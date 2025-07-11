using System;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Controller;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Dtos.ProgramDto;
using sport_app_backend.Interface;
using sport_app_backend.Mappers;
using sport_app_backend.Models;
using sport_app_backend.Models.Account;
using sport_app_backend.Models.Actions;
using sport_app_backend.Models.Challenge_Achievement;
using sport_app_backend.Models.Payments;
using sport_app_backend.Models.Program;
using Newtonsoft.Json;
using sport_app_backend.Dtos.ZarinPal;
using sport_app_backend.Dtos.ZarinPal.Verify;
using sport_app_backend.Models.TrainingPlan;


namespace sport_app_backend.Repository

{
    public class AthleteRepository(ApplicationDbContext context,IZarinPal zarinPal) : IAthleteRepository
    {
        public async Task<ApiResponse> GetFaq()
        {
            var getFaq = await context.AthleteFaq.ToListAsync();
            return new ApiResponse()
            {
                Action = true,
                Message = "get CoachFaq",
                Result = getFaq
            };

        }
        private async Task<ApiResponse> ConfirmTransactionId(Payment payment, long refId)
        {
            try
            {
                payment.CoachService.NumberOfSell += 1;
                payment.RefId = refId;

                var workoutProgram = new WorkoutProgram
                {
                    Title = payment.CoachService.Title,
                    Coach = payment.Coach,
                    CoachId = payment.Coach.Id,
                    Athlete = payment.Athlete,
                    AthleteId = payment.Athlete.Id,
                    Payment = payment,
                    PaymentId = payment.Id
                };
                payment.Coach.Amount += (payment.Amount - payment.Coach.ServiceFee);

                payment.WorkoutProgram = workoutProgram;
                payment.PaymentStatus = PaymentStatus.SUCCESS;

                await context.WorkoutPrograms.AddAsync(workoutProgram);
                await context.SaveChangesAsync();

                return new ApiResponse
                {
                    Message = "Payment confirmed successfully",
                    Action = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Message = $"Error confirming transaction: {ex.Message}",
                    Action = false
                };
            }
        }

        public async Task<ApiResponse> VerifyPaymentAsync(ZarinPalVerifyRequestDto request)
        {
            var payment = await context.Payments
                .Include(p => p.Coach)
                .Include(p => p.CoachService)
                .Include(p => p.Athlete)
                .Include(p => p.WorkoutProgram)
                .FirstOrDefaultAsync(x => x.Authority == request.Authority);

            if (payment == null)
            {
                return new ApiResponse
                {
                    Action = false,
                    Message = "تراکنشی یافت نشد",
                };
            }

            request.Amount = payment.Amount;
         
            try
            {
                var result = await zarinPal.VerifyPaymentAsync(request);
                switch (result?.Data)
                {
                    case { Code: 100 }:
                    {
                        var confirmResult = await ConfirmTransactionId(payment, result.Data.Ref_id);
                        if (!confirmResult.Action)
                        {
                            return confirmResult;
                        }

                        return new ApiResponse
                        {
                            Action = true,
                            Message = "پرداخت با موفقیت انجام شد ",
                            Result = result.Data.Ref_id
                        };
                    }
                    case { Code: 101 }:
                        return new ApiResponse
                        {
                            Action = true,
                            Message = "پرداخت با موفقیت انجام شد و قبلا تایید شده است  ",
                            Result = result.Data.Ref_id
                        };
                }

                if (result?.Errors is null)
                    return new ApiResponse
                    {
                        Action = false,
                        Message = "خطا ناشناخته",
                    };
                payment.PaymentStatus = PaymentStatus.FAILED;
                await context.SaveChangesAsync();

                var error = result?.Errors?.Select(e => e.Message).ToString() ??
                            "Unknown error from payment gateway.";
                return new ApiResponse
                {
                    Action = false,
                    Message = "پرداخت ناموفق",
                    Result = error
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying payment: {ex.Message}");
                return new ApiResponse
                {
                    Action = false,
                    Message = $"Error verifying payment: {ex.Message}"
                };
            }
        }
        
        public async Task<ApiResponse> BuyCoachingService(string phoneNumber, int coachingServiceId)
        {
            var athlete = await context.Athletes
                .Include(x => x.AthleteQuestions)
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

            if (athlete == null)
                return new ApiResponse { Message = "User is not an athlete", Action = false };

            var lastQuestion = athlete.AthleteQuestions.LastOrDefault();
            if (lastQuestion == null)
                return new ApiResponse { Message = "User has not completed the questions", Action = false };

            var coachService = await context.CoachServices
                .Include(x => x.Coach)
                .FirstOrDefaultAsync(x => x.Id == coachingServiceId && x.IsActive);

            if (coachService == null)
                return new ApiResponse { Message = "CoachingService not found", Action = false };

            if (coachService.IsDeleted)
                return new ApiResponse { Message = "CoachingService is deleted", Action = false };

            var zarinPalResponse = await zarinPal.RequestPaymentAsync(new ZarinPalPaymentRequestDto
            {
                amount = (long)coachService.Price,
                callback_url = "https://chaarset.ir/payment/verify",
                description = "خرید",
                Mobile = athlete.PhoneNumber
            });

            if (!zarinPalResponse.IsSuccessful)
            {
                return new ApiResponse
                {
                    Action = false,
                    Message = zarinPalResponse.ErrorMessage
                };
            }

            var payment = new Payment
            {
                Athlete = athlete,
                AthleteId = athlete.Id,
                CoachService = coachService,
                CoachServiceId = coachService.Id,
                CoachId = coachService.CoachId,
                Coach = coachService.Coach,
                Authority = zarinPalResponse.Authority,
                Amount = coachService.Price,
                AthleteQuestion = lastQuestion
            };

            await context.Payments.AddAsync(payment);
            await context.SaveChangesAsync();

            return new ApiResponse
            {
                Action = true,
                Message = "get url successfully",
                Result = zarinPalResponse
            };
        }

        public async Task<ApiResponse> GetMonthlyActivity(string phoneNumber, int year, int month)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(month);
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse() { Message = "User is not an athlete", Action = false };

            var persianCalendar = new System.Globalization.PersianCalendar();

            try
            {
                var startDate = persianCalendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
                var endDate = month == 12
                    ? persianCalendar.ToDateTime(year + 1, 1, 1, 0, 0, 0, 0)
                    : persianCalendar.ToDateTime(year, month + 1, 1, 0, 0, 0, 0);

                var activities = await context.Activities
                    .Where(x => x.AthleteId == athlete.Id && x.Date >= startDate && x.Date < endDate)
                    .ToListAsync();

                return new ApiResponse()
                {
                    Message = "Activities found",
                    Action = true,
                    Result = activities.Select(x => new ActivityDto()
                    {
                        Id = x.Id,
                        Date = x.Date.ToString("yyyy-MM-dd"),
                        CaloriesLost = x.CaloriesLost,
                        Duration = x.Duration,
                        ActivityCategory = x.ActivityCategory.ToString(),
                        Name = x.Name ?? ""
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    Message = $"Error converting date: {ex.Message}",
                    Action = false
                };
            }
        }

        public async Task<ApiResponse> GetLastWeekActivity(string phoneNumber)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse() { Message = "User is not an athlete", Action = false };

            var today = DateTime.Now.Date;
            var lastSaturday = GetLastSaturday(today);

            var activities = await context.Activities
                .Where(x => x.AthleteId == athlete.Id && x.Date >= lastSaturday)
                .ToListAsync();

            return new ApiResponse()
            {
                Message = "Activities found",
                Action = true,
                Result = activities.Select(x => new ActivityDto()
                {
                    Id = x.Id,
                    Date = x.Date.ToString("yyyy-MM-dd"),
                    CaloriesLost = x.CaloriesLost,
                    Duration = x.Duration,
                    ActivityCategory = x.ActivityCategory.ToString(),
                    Name = x.Name ?? ""
                }).ToList()
            };
        }

        public async Task<ApiResponse> TodayActivityReport(string phoneNumber)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse() { Message = "User is not an athlete", Action = false };

            var today = DateTime.Now.Date;
            var activities = await context.Activities
                .Where(x => x.AthleteId == athlete.Id && x.Date == today)
                .ToListAsync();

            return new ApiResponse()
            {
                Message = "Activities found",
                Action = true,
                Result = activities.Select(x => new ActivityDto()
                {
                    Id = x.Id,
                    Date = x.Date.ToString("yyyy-MM-dd"),
                    CaloriesLost = x.CaloriesLost,
                    Duration = x.Duration,
                    ActivityCategory = x.ActivityCategory.ToString(),
                    Name = x.Name ?? ""
                }).ToList()
            };
        }

        public async Task<ApiResponse> AddActivity(string phoneNumber, AddActivityDto addSportDto)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse()
                    { Message = "User is not an athlete", Action = false }; // Ensure the user is an athlete

            var activityCategory = Enum.Parse<ActivityCategory>(addSportDto.ActivityCategory!);

            var sport = new Activity()
            {
                AthleteId = athlete.Id,
                Athlete = athlete,
                ActivityCategory = activityCategory,
                CaloriesLost = addSportDto.CaloriesLost,
                Distance = addSportDto.Distance,
                Duration = addSportDto.Duration,
                Date = Convert.ToDateTime(addSportDto.Date)
            };

            await context.Activities.AddAsync(sport);
            await context.SaveChangesAsync();

            return new ApiResponse()
            {
                Message = "Sport added successfully",
                Action = true,
                Result = new
                {
                    sport.CaloriesLost,
                    sport.Duration,
                    sport.Distance,
                    ActivityCategory = sport.ActivityCategory.ToString()
                }
            };
        }

        public async Task<ApiResponse> AddWaterIntake(string phoneNumber, WaterInTakeDto waterInTakeDto)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse()
                    { Message = "User is not an athlete", Action = false }; // Ensure the user is an athlete
            if (waterInTakeDto.DailyCupOfWater < 0 || waterInTakeDto.Reminder < 0)
            {
                return new ApiResponse()
                    { Message = "your data input is negative", Action = false };
            }

            var waterIntake = new WaterInTake
            {
                AthleteId = athlete.Id,
                Athlete = athlete,
                DailyCupOfWater = waterInTakeDto.DailyCupOfWater,
                Reminder = waterInTakeDto.Reminder
            };
            athlete.WaterInTake = waterIntake;
            //edit last water intake if it exists
            var lastWaterIntake = await context.WaterInTakes
                .Where(w => w.AthleteId == athlete.Id)
                .FirstOrDefaultAsync();
            if (lastWaterIntake != null)
            {
                lastWaterIntake.DailyCupOfWater = waterInTakeDto.DailyCupOfWater;
                lastWaterIntake.Reminder = waterInTakeDto.Reminder;
                context.WaterInTakes.Update(lastWaterIntake);
                athlete.WaterInTake = lastWaterIntake; // Update the athlete's WaterInTake reference
            }
            else
            {
                athlete.WaterInTake = waterIntake;
                context.WaterInTakes.Add(waterIntake);
            }

            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "WaterIntake added successfully",
                Action = true
            };
        }

        public async Task<ApiResponse> SearchCoaches(CoachNameSearchDto coachNameSearchDto)
        {
            var coaches = await context.Users.Where(c =>
                (c.FirstName + " " + c.LastName).Contains(coachNameSearchDto.FullName) &&
                c.TypeOfUser == TypeOfUser.COACH).ToListAsync();
            return new ApiResponse()
            {
                Message = "Coaches found",
                Action = true,
                Result = coaches.Select(c => c.ToCoachForSearch()).ToList()
            };
        }

        public async Task<ApiResponse> GetLastQuestion(string phoneNumber)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
            {
                return new ApiResponse() { Message = "User is not an athlete", Action = false };
            }

            var lastQuestion = await context.AthleteQuestions
                .Where(q => q.AthleteId == athlete.Id).Include(i => i.InjuryArea)
                .OrderByDescending(q => q.CreatedAt)
                .FirstOrDefaultAsync();
            if (lastQuestion is null) return new ApiResponse() { Message = "Question not found", Action = false };

            return new ApiResponse()
            {
                Message = "Question found",
                Action = true,
                Result = lastQuestion.ToAthleteQuestionDto()
            };
        }

        public async Task<ApiResponse> DeleteActivity(string phoneNumber, int activityId)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse()
                    { Message = "User is not an athlete", Action = false }; // Ensure the user is an athlete
            var activity =
                await context.Activities.FirstOrDefaultAsync(x => x.Id == activityId && x.AthleteId == athlete.Id);
            if (activity is null) return new ApiResponse() { Message = "Activity not found", Action = false };
            context.Activities.Remove(activity);
            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "Activity deleted successfully",
                Action = true
            };
        }

        public async Task<ApiResponse> SubmitAthleteQuestions(string phoneNumber, AthleteQuestionDto athleteQuestionDto)
        {
            var athlete = await context.Athletes.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse()
                    { Message = "User is not an athlete", Action = false }; // Ensure the user is an athlete
            if (athlete.User is null) return new ApiResponse() { Message = "User not found", Action = false };
            athlete.User.BirthDate = Convert.ToDateTime(athleteQuestionDto.BirthDay);
            var athleteQuestion = athleteQuestionDto.ToAthleteQuestion(athlete);
            athlete.AthleteQuestions.Add(athleteQuestion);
            await context.AthleteQuestions.AddAsync(athleteQuestion);

            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "Athlete questions submitted successfully",
                Action = true
            };
        }

        public async Task<ApiResponse> AthleteFirstQuestions(string phoneNumber,
            AthleteFirstQuestionsDto athleteFirstQuestionsDto)
        {
            var user = await context.Users.Include(a => a.Athlete)
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (user is null) return new ApiResponse() { Message = "User not found", Action = false };
            var athlete = user.Athlete;
            if (athlete is null)
                return new ApiResponse()
                    { Message = "User is not an athlete", Action = false }; // Ensure the user is an athlete
            athlete.Height = athleteFirstQuestionsDto.Height;
            athlete.CurrentWeight = athleteFirstQuestionsDto.CurrentWeight;
            var weightEntry = new WeightEntry()
            {
                Athlete = athlete,
                AthleteId = athlete.Id,
                CurrentDate = DateTime.Now,
                Weight = athleteFirstQuestionsDto.CurrentWeight
            };
            await context.WeightEntries.AddAsync(weightEntry);
            user.LastName = athleteFirstQuestionsDto.LastName;
            user.FirstName = athleteFirstQuestionsDto.FirstName;
            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "Athlete first questions submitted successfully",
                Action = true,
                Result = new
                {
                    Questions = true
                }
            };
        }

        public async Task<ApiResponse> UpdateWaterInDay(string phoneNumber, int numberOfCup)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null) return new ApiResponse() { Message = "User is not athlete", Action = false };
            var waterInDay = await context.WaterInDays
                .Where(w => w.AthleteId == athlete.Id && w.Date.Date == DateTime.Now.Date)
                .FirstOrDefaultAsync();

            if (waterInDay is not null)
            {
                waterInDay.NumberOfCupsDrinked += numberOfCup;
                context.WaterInDays.Update(waterInDay);
                await context.SaveChangesAsync();
                return new ApiResponse() { Message = "WaterInDay updated successfully", Action = true };
            }

            if (numberOfCup > 0)
            {
                waterInDay = new WaterInDay
                {
                    AthleteId = athlete.Id,
                    Date = DateTime.Now.Date,
                    Athlete = athlete,
                    NumberOfCupsDrinked = numberOfCup // Initialize with 1 cup since it's the first entry for today
                };
                athlete.WaterInDays.Add(waterInDay); // Add the new WaterInDay to the athlete's collection
                await context.WaterInDays.AddAsync(waterInDay);
                await context.SaveChangesAsync();
                return new ApiResponse() { Message = "WaterInDay added successfully", Action = true };
            }

            return new ApiResponse() { Message = "WaterInDay is zero", Action = false };
        }


        public async Task<ApiResponse> UpdateGoalWeight(string phoneNumber, double goalWeight)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete == null) return new ApiResponse() { Action = false, Message = "User is not an athlete" };
            athlete.WeightGoal = goalWeight;
            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "Goal weight updated successfully",
                Action = true
            };
        }

        public async Task<ApiResponse> UpdateWeight(string phoneNumber, double weight)
        {
            var athlete = await context.Athletes
                .Include(a => a.WeightEntries) // برای اطمینان از بارگذاری لیست وزن‌ها
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

            if (athlete is null)
                return new ApiResponse() { Message = "User is not an athlete", Action = false };

            var today = DateTime.Today;

            athlete.CurrentWeight = weight;

            var weightEntry = athlete.WeightEntries
                .FirstOrDefault(x => x.CurrentDate.Date == today);

            if (weightEntry is null)
            {
                weightEntry = new WeightEntry()
                {
                    AthleteId = athlete.Id,
                    Athlete = athlete,
                    CurrentDate = today,
                    Weight = weight
                };
                await context.WeightEntries.AddAsync(weightEntry);
            }
            else
            {
                weightEntry.Weight = weight;
            }

            await context.SaveChangesAsync();

            return new ApiResponse()
            {
                Message = "Weight updated successfully",
                Action = true
            };
        }

        public async Task<ApiResponse> UpdateHightWeight(string phoneNumber, double weight, int hight)
        {
            var athlete = await context.Athletes
                .Include(a => a.WeightEntries) // برای اطمینان از بارگذاری لیست وزن‌ها
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

            if (athlete is null)
                return new ApiResponse() { Message = "User is not an athlete", Action = false };

            athlete.CurrentWeight = weight;
            athlete.Height = hight;

            var today = DateTime.Today;

            var weightEntry = athlete.WeightEntries
                .FirstOrDefault(x => x.CurrentDate.Date == today);

            if (weightEntry is null)
            {
                Console.WriteLine("**null");
                weightEntry = new WeightEntry()
                {
                    AthleteId = athlete.Id,
                    Athlete = athlete,
                    CurrentDate = today, // استفاده از متغیر today
                    Weight = weight
                };
                await context.WeightEntries.AddAsync(weightEntry);
            }
            else
            {
                weightEntry.Weight = weight;
            }

            await context.SaveChangesAsync();

            return new ApiResponse()
            {
                Message = "Weight and hight updated successfully",
                Action = true
            };
        }

        public async Task<ApiResponse> GetLastMonthWeightReport(string phoneNumber)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse() { Message = "User is not an athlete", Action = false };


            var pc = new PersianCalendar();
            var today = DateTime.Now;
            var persianYear = pc.GetYear(today);
            var persianMonth = pc.GetMonth(today);
            DateTime firstDayOfPersianMonth = pc.ToDateTime(persianYear, persianMonth, 1, 0, 0, 0, 0);

            // دریافت رکوردهای وزن از اول ماه شمسی تاکنون
            var weightEntries = await context.WeightEntries
                .Where(x => x.AthleteId == athlete.Id && x.CurrentDate >= firstDayOfPersianMonth)
                .OrderByDescending(x => x.CurrentDate)
                .ToListAsync();

            return new ApiResponse()
            {
                Message = "Weight report fetched successfully",
                Action = true,
                Result = weightEntries.Select(x => new WeightReportDto()
                {
                    Date = x.CurrentDate.ToString("yyyy-MM-dd"),
                    Weight = x.Weight
                }).ToList()
            };
        }


        public async Task<ApiResponse> CompleteNewChallenge(string phoneNumber, string challenge)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse() { Message = "User is not an athlete", Action = false };

            var challengeExists =
                context.Challenges.Any(x => x.AthleteId == athlete.Id && x.ChallengeType.ToString() == challenge);
            if (challengeExists)
                return new ApiResponse() { Message = "Challenge already completed", Action = false };

            var chal = new Challenge
            {
                AthleteId = athlete.Id,
                Athlete = athlete,
                ChallengeType = Enum.Parse<ChallengeType>(challenge),
                CompletedAt = DateTime.Now
            };
            await context.Challenges.AddAsync(chal);
            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "Challenge completed successfully",
                Action = true
            };
        }

        public async Task<ApiResponse> CompletedChallenge(string phoneNumber)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse() { Message = "User is not an athlete", Action = false };

            var challenges = await context.Challenges
                .Where(x => x.AthleteId == athlete.Id)
                .ToListAsync();

            return new ApiResponse()
            {
                Message = "Challenges found",
                Action = true,
                Result = challenges.Select(c => c.ChallengeType.ToString()).ToList()
            };
        }

        public async Task<ApiResponse> GetAchievements(string phoneNumber)
        {
            var athlete = await context.Athletes.Include(athlete => athlete.WorkoutPrograms)
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse() { Message = "User is not an athlete", Action = false };


            var challenges = await context.Challenges.Where(c => c.AthleteId == athlete.Id).ToListAsync();
            var firstChallenge = challenges.Count != 0;
            var challengeSeeker = challenges.Count >= 5;
            var challengeMaster = challenges.Count >= 12;

            var activities = await context.Activities.Where(a => a.AthleteId == athlete.Id).ToListAsync();
            var firstWorkout = activities.Count != 0;
            var workoutDays = activities.Select(a => a.Date.Date).Distinct().OrderBy(d => d).ToList();
            var consistentAthlete = await HasConsecutiveDaysAsync(workoutDays, 7);
            var oneMonthComplete = workoutDays.Count >= 30;
            var threeMonthsGolden = workoutDays.Count >= 90;
            var masterAthlete = workoutDays.Count >= 365;
            var firstProgramDone = athlete.WorkoutPrograms.Any(w => w.EndDate > DateTime.Now);

            var achievements = new List<AchievementType>();
            if (firstWorkout)
                achievements.Add(AchievementType.FirstWorkout);
            if (consistentAthlete)
                achievements.Add(AchievementType.ConsistentAthlete);
            if (firstProgramDone)
                achievements.Add(AchievementType.FirstProgramDone);
            if (oneMonthComplete)
                achievements.Add(AchievementType.OneMonthComplete);
            if (threeMonthsGolden)
                achievements.Add(AchievementType.ThreeMonthsGolden);
            if (masterAthlete)
                achievements.Add(AchievementType.MasterAthlete);
            if (firstChallenge)
                achievements.Add(AchievementType.FirstChallenge);
            if (challengeSeeker)
                achievements.Add(AchievementType.ChallengeSeeker);
            if (challengeMaster)
                achievements.Add(AchievementType.ChallengeMaster);

            return new ApiResponse()
            {
                Message = "Achievements found",
                Action = true,
                Result = achievements.Select(a => a.ToString()).ToList()
            };
        }

        public async Task<ApiResponse> GetAllPayments(string phoneNumber)
        {
            var athlete = await context.Athletes.Include(a => a.WorkoutPrograms)
                .ThenInclude(p => p.Payment)
                .ThenInclude(s => s.CoachService)
                .ThenInclude(x => x.Coach)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(z => z.PhoneNumber == phoneNumber);
            if (athlete is null)
                return new ApiResponse()
                    { Action = false, Message = "Athlete not found" };


            return new ApiResponse()
            {
                Action = true,
                Message = "Payments found",
                Result = athlete.WorkoutPrograms.Select(x => x.ToAllWorkoutProgramResponseDto()).ToList()
            };
        }

        public async Task<ApiResponse> GetPayment(string phoneNumber, int paymentId)
        {
            var payment = await context.Payments
                .Include(p => p.Athlete)
                .ThenInclude(a => a.User)
                .Include(x => x.Coach) // بارگذاری Coac
                .ThenInclude(a => a.User)
                .Include(a => a.AthleteQuestion) // بارگذاری User داخل Athlete
                .ThenInclude(I => I!.InjuryArea)
                .Include(w => w.WorkoutProgram)
                .ThenInclude(z => z.ProgramInDays)
                .ThenInclude(z => z.AllExerciseInDays)
                .ThenInclude(e => e.Exercise)
                .FirstOrDefaultAsync(p => p.Athlete.PhoneNumber == phoneNumber && p.Id == paymentId);
            if (payment is null) return new ApiResponse() { Message = "Payment not found", Action = false };
            return new ApiResponse()
            {
                Message = "Payment found",
                Action = true,
                Result = payment.ToAthletePaymentResponseDto()
            };
        }

        public Task<ApiResponse> GetAllPrograms(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetProgram(string phoneNumber, int programId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> ActiveProgram(string phoneNumber, int programId)
        {
            var athlete = await context.Athletes.Include(a => a.WorkoutPrograms)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);

            if (athlete == null)
            {
                return new ApiResponse { Action = false, Message = "Athlete not found" };
            }

            var targetProgram = athlete.WorkoutPrograms.FirstOrDefault(w => w.Id == programId);

            if (targetProgram == null)
            {
                return new ApiResponse { Action = false, Message = "Workout program not found" };
            }

            var allTrainingSessions = await context.TrainingSessions
                .Where(t => t.WorkoutProgramId == programId)
                .ToListAsync();

            Action<TrainingSession> resetTrainingSession = ts =>
            {
                ts.TrainingSessionStatus = TrainingSessionStatus.NOTSTARTED;
                var arrayBitMap = ts.ExerciseCompletionBitmap.ToArray();
                Array.Clear(arrayBitMap, 0, arrayBitMap.Length);
                ts.ExerciseCompletionBitmap = arrayBitMap;
            };

            if (targetProgram.Status == WorkoutProgramStatus.ACTIVE)
            {
                athlete.ActiveWorkoutProgramId = programId;
                allTrainingSessions.ForEach(resetTrainingSession);
                await context.SaveChangesAsync();
                return new ApiResponse { Action = true, Message = "Program already active and reset." };
            }

            foreach (var program in athlete.WorkoutPrograms.Where(x => x.Status == WorkoutProgramStatus.ACTIVE))
            {
                program.Status = WorkoutProgramStatus.STOPPED;
            }

            switch (targetProgram.Status)
            {
                case WorkoutProgramStatus.WRITING:
                case WorkoutProgramStatus.NOTSTARTED:
                    return new ApiResponse
                        { Action = false, Message = "Workout program status is not acceptable for activation." };

                case WorkoutProgramStatus.STOPPED:
                case WorkoutProgramStatus.FINISHED:
                    allTrainingSessions.ForEach(resetTrainingSession);
                    break;

                case WorkoutProgramStatus.NOTACTIVE:
                    await AddTrainingSession(targetProgram.PaymentId);
                    break;
            }

            targetProgram.Status = WorkoutProgramStatus.ACTIVE;
            athlete.ActiveWorkoutProgramId = programId;

            await context.SaveChangesAsync();
            return new ApiResponse { Action = true, Message = "Program activated successfully." };
        }

        private async Task AddTrainingSession(int paymentId)
        {
            var workoutProgram = await context.WorkoutPrograms
                .Include(p => p.Payment)
                .ThenInclude(p => p.AthleteQuestion)
                .Include(p => p.ProgramInDays)
                .ThenInclude(d => d.AllExerciseInDays)
                .FirstAsync(p => p.PaymentId == paymentId);

            var numberOfDay = workoutProgram.ProgramDuration *
                              workoutProgram.Payment.AthleteQuestion.DaysPerWeekToExercise;
            var programInDayList = workoutProgram.ProgramInDays;
            var programInDayCount = programInDayList.Count;

            var sessions = new List<TrainingSession>();

            for (var day = 1; day <= numberOfDay; day++)
            {
                var index = day % programInDayCount;
                sessions.Add(new TrainingSession
                {
                    ProgramInDayId = programInDayList[index].Id,
                    ProgramInDay = programInDayList[index],
                    ExerciseCompletionBitmap = new byte[programInDayList[index].AllExerciseInDays.Count],
                    TrainingSessionStatus = TrainingSessionStatus.NOTSTARTED,
                    DayNumber = day,
                    WorkoutProgram = workoutProgram,
                    WorkoutProgramId = workoutProgram.Id
                });
            }

            await context.TrainingSessions.AddRangeAsync(sessions);
            await context.SaveChangesAsync();
        }

        private static async Task<bool> HasConsecutiveDaysAsync(List<DateTime> dates, int requiredConsecutive)
        {
            return await Task.Run(() =>
            {
                if (dates.Count == 0)
                    return false;

                var consecutive = 1;
                for (var i = 1; i < dates.Count; i++)
                {
                    if ((dates[i] - dates[i - 1]).Days == 1)
                    {
                        consecutive++;
                        if (consecutive >= requiredConsecutive)
                            return true;
                    }
                    else if ((dates[i] - dates[i - 1]).Days > 1)
                    {
                        consecutive = 1;
                    }
                }

                return false;
            });
        }


        public async Task<ApiResponse> ExerciseFeedBack(string phoneNumber, ExerciseFeedbackDto feedbackDto)
        {
            var athlete = await context.Athletes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

            if (athlete is null)
            {
                return new ApiResponse() { Message = "ورزشکار یافت نشد!", Action = false };
            }


            var trainingSession = await context.TrainingSessions
                .AsNoTracking()
                .Include(ts => ts.WorkoutProgram)
                .Include(ts => ts.ProgramInDay)
                .ThenInclude(pid => pid.AllExerciseInDays)
                .FirstOrDefaultAsync(ts => ts.Id == feedbackDto.TrainingSessionId);


            if (trainingSession is null)
            {
                return new ApiResponse() { Message = "جلسه تمرینی یافت نشد!", Action = false };
            }

            if (trainingSession.WorkoutProgram?.AthleteId != athlete.Id)
            {
                return new ApiResponse() { Message = "شما به این جلسه تمرینی دسترسی ندارید.", Action = false };
            }

            if (trainingSession.ProgramInDay.AllExerciseInDays.All(se => se.Id != feedbackDto.SingleExerciseId))
            {
                return new ApiResponse() { Message = "تمرین مشخص شده در این جلسه وجود ندارد.", Action = false };
            }


            var feedback = feedbackDto.ToExerciseFeedback();
            feedback.AthleteId = athlete.Id;
            feedback.CoachId = trainingSession.WorkoutProgram.CoachId;

            await context.ExerciseFeedbacks.AddAsync(feedback);
            await context.SaveChangesAsync();

            return new ApiResponse
            {
                Action = true,
                Message = "فیدبک شما با موفقیت ثبت شد.",};
        }


        public async Task<ApiResponse> ChangeExercise(string phoneNumber, ExerciseChangeDto dto)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null) return new ApiResponse() { Message = "Athlete not found", Action = false };
            var singleExercise = await context.SingleExercises.Include(p => p.ProgramInDay)
                .ThenInclude(w => w.WorkoutProgram)
                .FirstOrDefaultAsync(s => s.Id == dto.SingleExerciseId);
            var exerciseChangeRequest = dto.ToExerciseChangeRequest();
            exerciseChangeRequest.AthleteId = athlete.Id;
            exerciseChangeRequest.SingleExerciseId = dto.SingleExerciseId;
            exerciseChangeRequest.TrainingSessionId = dto.TrainingSessionId;
            if (singleExercise?.ProgramInDay?.WorkoutProgram != null)
                exerciseChangeRequest.CoachId = singleExercise.ProgramInDay.WorkoutProgram.CoachId;

            await context.ExerciseChangeRequests.AddAsync(exerciseChangeRequest);
            await context.SaveChangesAsync();

            return new ApiResponse
            {
                Action = true,
                Message = "Exercise change request saved.", };
        }


        public async Task<ApiResponse> GetAllTrainingSession(string phoneNumber)
        {
            var athlete = await context.Athletes.Include(a => a.WorkoutPrograms)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);
            if (athlete is null) return new ApiResponse() { Message = "Athlete not found", Action = false };
            var workoutProgram = athlete.WorkoutPrograms.FirstOrDefault(x =>
                x.Status == WorkoutProgramStatus.ACTIVE);
            if (athlete.ActiveWorkoutProgramId == 0 || workoutProgram is null)
                return new ApiResponse() { Message = "workoutProgram not found", Action = true };

            var trainingSessions =
                await context.TrainingSessions.Include(T => T.ProgramInDay)
                    .ThenInclude(p => p.AllExerciseInDays)
                    .ThenInclude(z => z.Exercise)
                    .Where(t => t.WorkoutProgram.Id == athlete.ActiveWorkoutProgramId).ToListAsync();

            return new ApiResponse()
            {
                Action = true,
                Message = "get TrainingSession",
                Result = new
                {
                    ToAllTrainingSession = trainingSessions.Select(y => y.ToAllTrainingSessionDto()),
                    ProgramName = workoutProgram.Title
                }
            };
        }

        public async Task<ApiResponse> GetTrainingSession(string phoneNumber, int trainingSessionId)
        {  
            var athlete = await context.Athletes.FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);
            if (athlete is null) return new ApiResponse() { Message = "Athlete not found", Action = false };

            var trainingSession = await context.TrainingSessions
                .Include(ts => ts.WorkoutProgram)
                .Include(p => p.ProgramInDay)
                .ThenInclude(a => a.AllExerciseInDays).
                ThenInclude(e => e.Exercise)
                .FirstOrDefaultAsync(z => z.Id == trainingSessionId);
            
            if (trainingSession is null)
                return new ApiResponse() { Message = "trainingSession not found", Action = false };
            
            var finalCalories = _CalculateCaloriesInternal(trainingSession, athlete.CurrentWeight, false);

            
            return new ApiResponse()
            {
                Action = true,
                Message = "get TrainingSession",
                Result = trainingSession.ToTrainingSessionDto(finalCalories)
            };
        }

        public async Task<ApiResponse> DoTrainingSession(string phoneNumber, int trainingSessionId, int exerciseNumber)
        {
            var trainingSession = await context.TrainingSessions
                .FirstOrDefaultAsync(z => z.Id == trainingSessionId);
            if (trainingSession is null)
                return new ApiResponse() { Message = "trainingSession not found", Action = false };
            
            trainingSession.TrainingSessionStatus = TrainingSessionStatus.INPROGRESS;

            var bitmap = trainingSession.ExerciseCompletionBitmap.ToArray();
            bitmap[exerciseNumber] = 0xFF;
            trainingSession.ExerciseCompletionBitmap = bitmap;
            var allCompleted = trainingSession.ExerciseCompletionBitmap.All(b => b == 0xFF);
            trainingSession.TrainingSessionStatus =
                allCompleted ? TrainingSessionStatus.COMPLETED : TrainingSessionStatus.INPROGRESS;

            await context.SaveChangesAsync();

            return new ApiResponse()
            {
                Action = true,
                Message = "Do Training session",
                Result = trainingSession.ToAllTrainingSessionDto()
            };
        }

        public async Task<ApiResponse> FinishTrainingSession(string phoneNumber,
            FinishTrainingSessionDto finishTrainingSessionDto)
        {
            try
            {
                var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
                if (athlete is null) return new ApiResponse() { Message = "Athlete not found", Action = false };

                var trainingSession = await context.TrainingSessions
                    .Include(ts => ts.WorkoutProgram)
                    .Include(ts => ts.ProgramInDay)
                    .ThenInclude(pid => pid.AllExerciseInDays)
                    .ThenInclude(se => se.Exercise) // اطمینان از بارگذاری اطلاعات هر حرکت
                    .FirstOrDefaultAsync(z => z.Id == finishTrainingSessionDto.TrainingSessionId);

                if (trainingSession is null)
                    return new ApiResponse() { Message = "trainingSession not found", Action = false };
                var athleteWeight = athlete.CurrentWeight;
                
                var finalCalories = _CalculateCaloriesInternal(trainingSession, athleteWeight, true);


                trainingSession.TrainingSessionStatus = TrainingSessionStatus.COMPLETED;
                trainingSession.WorkoutProgram.LastExerciseDate = DateTime.Now.Date;
                trainingSession.WorkoutProgram.CompletedSessionCount++;

                var activity = new Activity()
                {
                    Athlete = athlete,
                    Duration = finishTrainingSessionDto.Duration,
                    CaloriesLost = finalCalories,
                    ActivityCategory = ActivityCategory.EXERCISE,
                    Name = finishTrainingSessionDto.TrainingSessionName,
                    Date = DateTime.Now.Date
                };

                await context.Activities.AddAsync(activity);
                await context.SaveChangesAsync();

                return new ApiResponse()
                {
                    Action = true,
                    Message = "Finish Training session",
                    Result = activity.ToActivityDto()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Action = false,
                    Message = $"Error finishing: {ex.Message}"
                };
            }
        }


        public async Task<ApiResponse> FeedbackTrainingSession(string phoneNumber,
            FeedbackTrainingSessionDto feedbackTrainingSessionDto)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null) return new ApiResponse() { Message = "Athlete not found", Action = false };
            var trainingSession = await context.TrainingSessions
                .FirstOrDefaultAsync(z => z.Id == feedbackTrainingSessionDto.TrainingSessionId);
            if (trainingSession is null)
                return new ApiResponse() { Message = "trainingSession not found", Action = false };
            if (trainingSession.TrainingSessionStatus != TrainingSessionStatus.COMPLETED)
                return new ApiResponse() { Message = "trainingSession not completed", Action = false };
            trainingSession.ExerciseFeeling =
                Enum.Parse<ExerciseFeeling>(feedbackTrainingSessionDto.ExerciseFeeling ?? string.Empty);
            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Action = true,
                Message = "Feedback Training session"
                
            };
        }


        public async Task<ApiResponse> ResetTrainingSession(string phoneNumber, int trainingSessionId)
        {
            var trainingSession = await context.TrainingSessions
                .FirstOrDefaultAsync(z => z.Id == trainingSessionId);
            if (trainingSession is null)
                return new ApiResponse() { Message = "trainingSession not found", Action = false };

            trainingSession.TrainingSessionStatus = TrainingSessionStatus.NOTSTARTED;

            var arrayBitMap = trainingSession.ExerciseCompletionBitmap.ToArray();
            Array.Clear(arrayBitMap, 0, arrayBitMap.Length);
            trainingSession.ExerciseCompletionBitmap = arrayBitMap;
            await context.SaveChangesAsync();

            return new ApiResponse()
            {
                Action = true,
                Message = "Training session Retested",
                Result = trainingSession.ToAllTrainingSessionDto()
            };
        }

        public async Task<ApiResponse> GetActivityPage(string phoneNumber)
        {
            var athlete = await context.Athletes
                .Include(a => a.WeightEntries)
                .Include(a => a.WaterInDays)
                .Include(a => a.Activities).Include(athlete => athlete.WaterInTake)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);

            if (athlete is null)
                return new ApiResponse { Message = "Athlete not found", Action = false };
            var waterInTake = athlete.WaterInTake ?? new WaterInTake()
            {
                DailyCupOfWater = 0,
                Reminder = 0
            };

            var today = DateTime.Today.Date;
            var lastSaturday = GetLastSaturday(today);
            var firstDayOfPersianMonth = GetFirstDayOfPersianMonth(today);

            var allActivities = athlete.Activities.ToList();

            var totalActivities = allActivities.Count;
            var totalTime = allActivities.Select(a => a.Duration).DefaultIfEmpty(0).Sum();
            var totalCalories = allActivities.Select(a => a.CaloriesLost).DefaultIfEmpty(0).Sum();

            var lastWeekActivities = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = lastSaturday.AddDays(offset).Date;
                    return athlete.Activities.Any(a => a.Date.Date == date) ? 1 : 0;
                })
                .ToList();

            var todayWater = athlete.WaterInDays.FirstOrDefault(w => w.Date == today);


            var todayActivities = athlete.Activities
                .Where(a => a.Date.Date == today)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Date = a.Date.ToString("yyyy-MM-dd"),
                    CaloriesLost = a.CaloriesLost,
                    Duration = a.Duration,
                    ActivityCategory = a.ActivityCategory.ToString(),
                    Name = a.Name ?? ""
                })
                .ToList();

            var currentWeight = athlete.CurrentWeight;
            var goalWeight = athlete.WeightGoal;

            var lastMonthWeights = athlete.WeightEntries
                .Where(w => w.CurrentDate >= firstDayOfPersianMonth)
                .OrderByDescending(w => w.CurrentDate)
                .Select(w => new WeightReportDto
                {
                    Date = w.CurrentDate.ToString("yyyy-MM-dd"),
                    Weight = w.Weight
                })
                .ToList();

            return new ApiResponse
            {
                Message = "Activities found",
                Action = true,
                Result = new ActivityPageDto()
                {
                    TotalActivities = totalActivities,
                    TotalTime = totalTime,
                    TotalCalories = totalCalories,
                    LastWeekActivities = lastWeekActivities,
                    NumberOfCupsDrinked = todayWater?.NumberOfCupsDrinked ?? 0,
                    DailyCupOfWater = waterInTake.DailyCupOfWater,
                    Reminder = waterInTake.Reminder,
                    TodayActivities = todayActivities,
                    CurrentWeight = currentWeight,
                    GoalWeight = goalWeight,
                    LastMonthWeights = lastMonthWeights,
                }
            };
        }

        public async Task<ApiResponse> CalculateCalories(string phoneNumber, int trainingSessionId)
        {
            var athlete = await context.Athletes.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (athlete is null) return new ApiResponse() { Message = "Athlete not found", Action = false };

    
            var trainingSession = await context.TrainingSessions
                .Include(ts => ts.WorkoutProgram)
                .Include(ts => ts.ProgramInDay)
                .ThenInclude(pid => pid.AllExerciseInDays)
                .ThenInclude(se => se.Exercise)
                .FirstOrDefaultAsync(z => z.Id == trainingSessionId);
            
            if (trainingSession is null)
                return new ApiResponse() { Message = "trainingSession not found", Action = false };
            
            var athleteWeight = athlete.CurrentWeight;
            
            var finalCalories = _CalculateCaloriesInternal(trainingSession, athleteWeight, true);
            return new ApiResponse()
            {
                Action = true,
                Message = "Calories calculated successfully for completed exercises.",
                Result = finalCalories
            };
        }
        private DateTime GetLastSaturday(DateTime today)
        {
            var diff = ((int)today.DayOfWeek - (int)DayOfWeek.Saturday + 7) % 7;
            return today.AddDays(-diff);
        }
        
        private DateTime GetFirstDayOfPersianMonth(DateTime date)
        {
            var pc = new PersianCalendar();
            var year = pc.GetYear(date);
            var month = pc.GetMonth(date);
            return pc.ToDateTime(year, month, 1, 0, 0, 0, 0);
        }

        private static double _CalculateCaloriesInternal(
            TrainingSession trainingSession ,double athleteWeight, bool completedExercisesOnly)
        {
            
            const double restMet = 1.3;


            // 1. میانگین‌گیری از پارامترها بر اساس اهداف برنامه
            var priorities = trainingSession.WorkoutProgram.ProgramPriorities;
            var avgParams = new TrainingGoalParameter
            {
                RestBetweenSetsSec = priorities.Average(p => TrainingGoalParameters.Parameters[p].RestBetweenSetsSec),
                RestBetweenMovesSec = priorities.Average(p => TrainingGoalParameters.Parameters[p].RestBetweenMovesSec),
                TimePerRepSec = priorities.Average(p => TrainingGoalParameters.Parameters[p].TimePerRepSec),
                EpocPercentage = priorities.Average(p => TrainingGoalParameters.Parameters[p].EpocPercentage)
            };

            double totalCaloriesActiveAndRestSets = 0;
            var exercisesCountInCalculation = 0;
            var allExercises = trainingSession.ProgramInDay.AllExerciseInDays;

            // ۲. محاسبه کالری
            for (var i = 0; i < allExercises.Count; i++)
            {
                // اگر فقط حرکات انجام شده مد نظر است، بیت‌مپ را چک کن
                if (completedExercisesOnly)
                {
                    if (i >= trainingSession.ExerciseCompletionBitmap.Length ||
                        trainingSession.ExerciseCompletionBitmap[i] != 0xFF)
                    {
                        continue; // اگر حرکت انجام نشده، از آن بگذر
                    }
                }

                var exercise = allExercises[i];
                if (exercise.Exercise == null) continue;

                exercisesCountInCalculation++;

                var workTimeSec = exercise.Set * exercise.Rep * avgParams.TimePerRepSec;
                var restBetweenSetsSec = (exercise.Set > 1) ? (exercise.Set - 1) * avgParams.RestBetweenSetsSec : 0;

                // اگر MET صفر بود، یک مقدار پیش‌فرض در نظر می‌گیریم
                var exerciseMet = (exercise.Exercise.Met == 0) ? 3.0 : exercise.Exercise.Met;
                var caloriesActive = (exerciseMet * athleteWeight * workTimeSec) / 3600.0;
                var caloriesRestSets = (restMet * athleteWeight * restBetweenSetsSec) / 3600.0;

                totalCaloriesActiveAndRestSets += caloriesActive + caloriesRestSets;
            }

            if (exercisesCountInCalculation == 0 && completedExercisesOnly)
            {
                return 0.0;
            }


            var totalRestBetweenMovesSec = (exercisesCountInCalculation > 1)
                ? (exercisesCountInCalculation - 1) * avgParams.RestBetweenMovesSec
                : 0;
            var caloriesRestMoves = (restMet * athleteWeight * totalRestBetweenMovesSec) / 3600.0;


            var totalCaloriesBeforeEpoc = totalCaloriesActiveAndRestSets + caloriesRestMoves;


            var finalCalories = totalCaloriesBeforeEpoc * (1 + avgParams.EpocPercentage);

            return Math.Round(finalCalories, 2);
        }
    }
}
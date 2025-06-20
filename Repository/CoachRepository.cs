﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Dtos.ProgramDto;
using sport_app_backend.Interface;
using sport_app_backend.Mappers;
using sport_app_backend.Models;
using sport_app_backend.Models.Payments;
using sport_app_backend.Models.Program;


namespace sport_app_backend.Repository
{
    public class CoachRepository(ApplicationDbContext context) : ICoachRepository
    {
        public async Task<ApiResponse> AddCoachingServices(string phoneNumber, AddCoachServiceDto addCoachingServiceDto)
        {
         
            var coach = await context.Coaches.Include(c => c.CoachingServices).FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
            if (coach is null) return new ApiResponse() { Message = "User is not a coach", Action = false };// Ensure the user is a coach
            var coachingService = addCoachingServiceDto.ToCoachService(coach);
            coach.CoachingServices ??= [];
            coach.CoachingServices.Add(coachingService);
            context.CoachServices.Add(coachingService);
            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "Coaching Service added successfully",
                Action = true
                
            };
        }
        public async Task<ApiResponse> SubmitCoachQuestions(string phoneNumber, CoachQuestionDto coachQuestionDto)
        {  
            var user = await context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (user is null) return new ApiResponse() { Message = "User not found", Action = false };
            var coach = user.Coach;
            if (coach == null) return new ApiResponse() { Message = "User is not a coach", Action = false };// Ensure the user is a coach
            user.FirstName=coachQuestionDto.FirstName;
            user.LastName=coachQuestionDto.LastName;
            var coachQuestion = coachQuestionDto.ToCoachQuestion(user);
            coach.CoachQuestion = coachQuestion;
            await context.CoachQuestions.AddAsync(coachQuestion);
            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "Coach questions submitted successfully",
                Action = true,
                Result=new
                {
                    Questions=true
                }
                
            };
        }

        public async Task<ApiResponse> UpdateCoachingService(string phoneNumber,int id, AddCoachServiceDto addCoachingServices)
        {

            var coach = await context.Coaches.Include(x=>x.CoachingServices).FirstOrDefaultAsync(x=>x.PhoneNumber==phoneNumber);
            if(coach is null) return new ApiResponse() { Message = "User is not a coach", Action = false };// Ensure the user is a coach
            var coachingService = coach.CoachingServices
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);

            if (coachingService is null)
            {
                return new ApiResponse() { Message = "سرویس کوچینگ یافت نشد یا قبلاً حذف شده است.", Action = false };
            }
            var hasActivePayments = await context.Payments
                .AnyAsync(p => p.CoachServiceId == coachingService.Id) ;

            if (hasActivePayments)
            {
                coachingService.IsDeleted = true;
                var newCoachService = addCoachingServices.ToCoachService(coach);
                newCoachService.NumberOfSell = coachingService.NumberOfSell;
                coach.CoachingServices.Add(newCoachService);
                 await context.CoachServices.AddAsync(newCoachService);
            }else{
                coachingService.UpdateCoachServices(addCoachingServices);
                
                
            }
        
            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "Coaching Service updated successfully",
                Action = true,
                Result = coachingService.ToCoachingServiceResponse()
            };

        }

        public async Task<ApiResponse> DeleteCoachingService(string phoneNumber, int id)
        {
            var coach = await context.Coaches.Include(x => x.CoachingServices).FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (coach is null) return new ApiResponse() { Message = "User is not a coach", Action = false };// Ensure the user is a coach
            var coachingService = coach.CoachingServices.FirstOrDefault(x => x.Id == id);
            if (coachingService is null) return new ApiResponse() { Message = "Coaching Service not found", Action = false };
            coachingService.IsDeleted = true;
            await context.SaveChangesAsync();
            return new ApiResponse()
            {
                Message = "Coaching Service deleted successfully",
                Action = true,
                Result = coachingService.ToCoachingServiceResponse()
            };
        }

        public async Task<ApiResponse> GetAllPayment(string phoneNumber)
        {
            var payments = await context.Payments
                .Include(p => p.Coach)
                .ThenInclude(c => c!.User)
                .Include(p => p.Athlete)
                .ThenInclude(a => a!.User)
                .Include(p => p.CoachService)
                .Include(p => p.WorkoutProgram)
                .Where(p => 
                    p.Coach != null &&
                    p.Coach.PhoneNumber == phoneNumber &&
                    p.PaymentStatus == PaymentStatus.SUCCESS &&
                    p.WorkoutProgram != null &&
                    (
                        p.WorkoutProgram.Status == WorkoutProgramStatus.NOTSTARTED ||
                        p.WorkoutProgram.Status == WorkoutProgramStatus.WRITING
                    )
                )
                .ToListAsync();
           
            
           
            return new ApiResponse()
            {
                Message = "Payments found",
                Action = true,
                Result = payments.Select(x=>x.ToCoachAllPaymentResponseDto())
            };
        }

        public async Task<ApiResponse> GetPayment(string phoneNumber, int paymentId)
        {
            var payment = await context.Payments
                .Include(p => p.Coach)  // بارگذاری Coach
                .Include(p => p.Athlete)  // بارگذاری Athlete
                .ThenInclude(a => a!.User)
                .Include(a=>a.AthleteQuestion)// بارگذاری User داخل Athlete
                .ThenInclude(I=> I!.InjuryArea)
                .Include(w=>w.WorkoutProgram)
                .ThenInclude(z=>z.ProgramInDays)
                .ThenInclude(z=>z.AllExerciseInDays)
                .ThenInclude(e=>e.Exercise)
                .FirstOrDefaultAsync(p => p.Coach.PhoneNumber == phoneNumber&& p.Id==paymentId);
            if(payment is null) return new ApiResponse() { Message = "Payment not found", Action = false };
           
            var result = payment.ToCoachPaymentResponseDto();
            if (result.WorkoutProgram!.ProgramInDays.Count == 0)
            {
                result.WorkoutProgram.ProgramInDays.Add(new ProgramInDayDto()
                {
                 
                    ForWhichDay = 1,
                    AllExerciseInDays = []

                });   
            }
            return new ApiResponse()
            {
                Message = "Payment found",
                Action = true,
                Result = result
            };

        }   



   

        public async Task<ApiResponse> GetProfile(string phoneNumber)
        {
            var user = await context.Users
                .Include(u => u.Coach)
                .ThenInclude(c=>c.CoachingServices)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user?.Coach == null) return new ApiResponse { Action = false, Message = "Coach not found" };
            var coachingService = user.Coach.CoachingServices.Where(x=>x.IsDeleted==false).ToList();
            var coachingServiceDto = coachingService.Select(x => x.ToCoachingServiceResponse()).ToList();
            var payments  = await context.Payments.Include(p=>p.Athlete).ThenInclude(u=>u.User).Include(p=>p.WorkoutProgram).
                Where(p => p.CoachId == user.Coach.Id && p.WorkoutProgram != null && p.WorkoutProgram.Status!=WorkoutProgramStatus.WRITING&&p.WorkoutProgram.Status!=WorkoutProgramStatus.NOTSTARTED).ToListAsync();
            return new ApiResponse
            {
                Action = true, Message = "Coach found",
                Result = user.ToCoachProfileResponseDto(coachingServiceDto, payments)
            };

        }

        public async Task<ApiResponse> SaveWorkoutProgram(string phoneNumber, int paymentId, WorkoutProgramDto workoutProgramDto)
        {
            try
            {
                var coach = await context.Coaches.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
                if (coach == null) return new ApiResponse { Action = false, Message = "Coach not found" };
                var workoutProgram = await context.WorkoutPrograms.Include(x=>x.ProgramInDays)
                    .ThenInclude(z=>z.AllExerciseInDays)
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
                if(workoutProgram is null) return new ApiResponse{ Action = false, Message = "Payment not found" };
                workoutProgram.ProgramInDays = workoutProgramDto.Days.ToListOfProgramInDays();
                workoutProgram.ProgramDuration = workoutProgramDto.Week;
                workoutProgram.GeneralWarmUp = workoutProgramDto.GeneralWarmUp?.Select(x => (GeneralWarmUp)Enum.Parse(typeof(GeneralWarmUp), x)).ToList()??[];
                workoutProgram.ProgramLevel = workoutProgramDto.ProgramLevel;
                if (workoutProgramDto.DedicatedWarmUp is not null)
                {
                    workoutProgram.DedicatedWarmUp =
                        (DedicatedWarmUp)Enum.Parse(typeof(DedicatedWarmUp), workoutProgramDto.DedicatedWarmUp);
                }

                workoutProgram.ProgramPriorities = workoutProgramDto.ProgramPriority.Select(x => (ProgramPriority)Enum.Parse(typeof(ProgramPriority), x.ToUpper())).ToList() ??[];
                if (workoutProgram.Status == WorkoutProgramStatus.NOTSTARTED)
                {
                    workoutProgram.Status = WorkoutProgramStatus.WRITING;
                }
                if (workoutProgramDto.Publish)
                {
                
                
                    workoutProgram.Status = WorkoutProgramStatus.NOTACTIVE;
                    var athlete = await context.Athletes.FirstOrDefaultAsync(a => a.Id == workoutProgram.AthleteId);
                    if (athlete is null)
                    {
                        return new ApiResponse()
                        {
                            Action = false,
                            Message = "athlete not found"
                        };
                    }

              
                    if (athlete.ActiveWorkoutProgramId == 0)
                    {                
                        await context.SaveChangesAsync();
                        await AddTrainingSession(paymentId);
                        workoutProgram.Status = WorkoutProgramStatus.ACTIVE;
                        athlete.ActiveWorkoutProgramId = workoutProgram.Id;


                    }

                }
                await context.SaveChangesAsync();
                return new ApiResponse()
                {
                    Action = true,
                    Message = "workout program saved"
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("*+*"+e);
                return new ApiResponse()
                {
                    Action = false,
                    Message = e.Message
                };
            }



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
            workoutProgram.TotalSessionCount = numberOfDay;


            for (var day = 1; day <= numberOfDay; day++)
            {
                var index = day % programInDayCount;
                await context.TrainingSessions.AddAsync(new TrainingSession
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


        }

        public async Task<ApiResponse> GetWorkoutProgram(string phoneNumber, int paymentId)
        {
            var coach = await context.Coaches.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
            if (coach == null) return new ApiResponse { Action = false, Message = "Coach not found" };
            var workoutProgram = await context.WorkoutPrograms.Include(x=>x.ProgramInDays)
                .ThenInclude(z=>z.AllExerciseInDays)
                .FirstOrDefaultAsync(p => p.Id == paymentId);
            if(workoutProgram is null) return new ApiResponse{ Action = false, Message = "Payment not found" };

            return new ApiResponse()
            {
                Action = true,
                Message = "workout program found",
                Result = workoutProgram.ProgramInDays.ToProgramInDayDto()
            };
            
        }

       public async Task<ApiResponse> GetCoachDashboard(string phoneNumber)
        {
            var coach = await context.Coaches.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
            if (coach == null)
            {
                return new ApiResponse { Action = false, Message = "مربی یافت نشد." };
            }

            var successfulPayments = await context.Payments
                .Where(p => p.CoachId == coach.Id && p.PaymentStatus == PaymentStatus.SUCCESS)
                .Include(p => p.WorkoutProgram) 
                .Include(p => p.Athlete) 
                .ToListAsync();

            if (successfulPayments.Count == 0)
            {
                return new ApiResponse { Action = true, Message = "گزارشی برای نمایش وجود ندارد.", Result = new CoachDashboardDto {
                    MonthlyIncome = new List<DailyIncomeDto>(),
                    AthleteStats = new AthleteStatsDto()
                }};
            }

            var totalSales = successfulPayments.Sum(p => p.Amount);
            var totalTransactions = successfulPayments.Count;
            var totalPrograms = successfulPayments.Count(p => p.WorkoutProgram != null);
            var totalAthletes = successfulPayments.Select(p => p.AthleteId).Distinct().Count();

            var activePrograms = successfulPayments
                .Where(p => p.WorkoutProgram?.Status == WorkoutProgramStatus.ACTIVE)
                .Select(p => p.WorkoutProgram)
                .ToList();
            
            var activeAthletesCount = activePrograms.Select(wp => wp.AthleteId).Distinct().Count();
            
            var athleteStats = new AthleteStatsDto
            {
                ActiveAthletes = activeAthletesCount,
                InactiveAthletes = totalAthletes - activeAthletesCount,
                NeedsFollowUp = activePrograms.Count(p => p.LastExerciseDate < DateTime.Now.Date.AddDays(-4) || p.LastExerciseDate == null),
                NearingCompletion = activePrograms.Count(p => 
                    (p.TotalSessionCount - p.CompletedSessionCount) < 5 && 
                    (p.TotalSessionCount - p.CompletedSessionCount) > 0)
            };

            var pc = new PersianCalendar();
            var today = DateTime.Now;
            var currentYear = pc.GetYear(today);
            var currentMonth = pc.GetMonth(today);
            var monthStartDate = pc.ToDateTime(currentYear, currentMonth, 1, 0, 0, 0, 0);
            var monthEndDate = monthStartDate.AddMonths(1);

            var monthlyIncome = successfulPayments
                .Where(p => p.PaymentDate >= monthStartDate && p.PaymentDate < monthEndDate)
                .GroupBy(p => p.PaymentDate.Date)
                .Select(g => new DailyIncomeDto
                {
                    Day = pc.GetDayOfMonth(g.Key).ToString(),
                    Amount = g.Sum(p => p.Amount)
                })
                .OrderBy(d => int.Parse(d.Day)) 
                .ToList();

            var dashboardDto = new CoachDashboardDto
            {
                TotalSales = totalSales,
                TotalTransactions = totalTransactions,
                MonthlyIncome = monthlyIncome,
                TotalPrograms = totalPrograms,
                TotalAthletes = totalAthletes,
                AthleteStats = athleteStats
            };

            return new ApiResponse { Action = true, Message = "گزارش با موفقیت دریافت شد.", Result = dashboardDto };
        }

        public async Task<ApiResponse> GetMonthlyIncomeChart(string phoneNumber, int year, int month)
        {
            var coach = await context.Coaches.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
            if (coach == null)
            {
                return new ApiResponse { Action = false, Message = "Coach not Found" };
            }

            var pc = new PersianCalendar();
            DateTime monthStartDate;
            try
            {
                monthStartDate = pc.ToDateTime(year, month, 1, 0, 0, 0, 0);
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ApiResponse { Action = false, Message = "سال یا ماه شمسی نامعتبر است." };
            }
            
            var monthEndDate = monthStartDate.AddMonths(1);

            // کوئری بهینه فقط برای دریافت اطلاعات مورد نیاز نمودار
            var monthlyIncome = await context.Payments
                .Where(p => p.CoachId == coach.Id && 
                            p.PaymentStatus == PaymentStatus.SUCCESS &&
                            p.PaymentDate >= monthStartDate && p.PaymentDate < monthEndDate)
                .GroupBy(p => p.PaymentDate.Date)
                .Select(g => new DailyIncomeDto
                {
                    Day = pc.GetDayOfMonth(g.Key).ToString(),
                    Amount = g.Sum(p => p.Amount)
                })
                .OrderBy(d => int.Parse(d.Day))
                .ToListAsync();

            return new ApiResponse { Action = true, Message = "نمودار درآمد ماهانه با موفقیت دریافت شد.", Result = monthlyIncome };
        }
    }
    }


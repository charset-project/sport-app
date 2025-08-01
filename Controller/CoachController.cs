﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Mappers;
using sport_app_backend.Models;
using System.Security.Claims;
using sport_app_backend.Dtos.ProgramDto;


namespace sport_app_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachController(ICoachRepository coachRepository, ApplicationDbContext dbContext) : ControllerBase
    {
        [HttpPost("CoachQuestions")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> SubmitCoachQuestions([FromBody] CoachQuestionDto coachQuestionDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");

            var result = await coachRepository.SubmitCoachQuestions(phoneNumber, coachQuestionDto);
            if (!result.Action) return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("CreatePayoutRequest")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> CreatePayoutRequest()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse { Action = false, Message = "PhoneNumber is null" });
            var result = await coachRepository.CreatePayoutRequest(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }


        [HttpGet("get_coach_profile")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetCoachProfile()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse { Action = false, Message = "PhoneNumber is null" });
            var result = await coachRepository.GetProfile(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("add_coaching_Service")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AddCoachingService(AddCoachServiceDto coachingServiceDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse { Action = false, Message = "PhoneNumber is null" });
            var result = await coachRepository.AddCoachingServices(phoneNumber, coachingServiceDto);

            if (result.Action) return Ok(result);
            else return BadRequest(result);
        }

        ///edit coaching Service
        [HttpPut("edit_coaching_Service/{id}")]
        [Authorize(Roles = "Coach")]

        public async Task<IActionResult> EditCoachingService([FromRoute] int id, AddCoachServiceDto coachingServiceDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse { Action = false, Message = "PhoneNumber is null" });
            var result = await coachRepository.UpdateCoachingService(phoneNumber, id, coachingServiceDto);
            if (result.Action != true) return BadRequest(result);
            return Ok(result);
        }
        
        //delete coaching Service
        [HttpDelete("delete_coaching_Service/{id}")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> DeleteCoachingService([FromRoute] int id)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse { Action = false, Message = "PhoneNumber is null" });
            var result = await coachRepository.DeleteCoachingService(phoneNumber, id);
            if (result.Action != true) return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("AthleteMonthlyActivityForCoach/{athleteId}/{year}/{month}")]
        [Authorize(Roles = "Coach")]

        public async Task<IActionResult> AthleteMonthlyActivityForCoach([FromRoute] int athleteId,[FromRoute] int year,[FromRoute] int month)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await coachRepository.AthleteMonthlyActivityForCoach(athleteId,year,month);
            if (!result.Action) return BadRequest(result);
            return Ok(result);

        }
        [HttpGet("AthleteReportForCoach/{athleteId}")]
        [Authorize(Roles = "Coach")]

        public async Task<IActionResult> AthleteReportForCoach([FromRoute] int athleteId)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await coachRepository.AthleteReportForCoach(athleteId);
            if (!result.Action) return BadRequest(result);
            return Ok(result);

        }

        /// get coaching Service
        [HttpGet("get_coaching_Service")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetCoachingService()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse { Action = false, Message = "PhoneNumber is null" });
            var coach = await dbContext.Coaches.Include(c => c.CoachingServices).FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
            if (coach == null) return NotFound(new ApiResponse { Action = false, Message = "Coach not found" });
            var coachingService = coach.CoachingServices.Where(x=>x.IsDeleted==false).ToList();
            var coachingServiceDto = coachingService.Select(x => x.ToCoachingServiceResponse()).ToList();
            return Ok(new ApiResponse { Action = true, Message = "Coaching Service found", Result = coachingServiceDto });

        }
       

        
        [HttpGet("get_all_payment")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetAllPayment()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse { Action = false, Message = "PhoneNumber is null" });
            var result = await coachRepository.GetAllPayment(phoneNumber);
            if (result.Action != true) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("get_payment/{paymentId}")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetPayment([FromRoute] int paymentId){
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse { Action = false, Message = "PhoneNumber is null" });
            var result = await coachRepository.GetPayment(phoneNumber, paymentId);
            if (result.Action != true) return BadRequest(result);
            return Ok(result);
            
        }

      

        [HttpPost("Save_workoutProgram/{paymentId}")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> SaveWorkoutProgram([FromRoute] int paymentId,[FromBody]WorkoutProgramDto  saveWorkoutProgramDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse { Action = false, Message = "PhoneNumber is null" });
            var result = await coachRepository.SaveWorkoutProgram(phoneNumber,paymentId, saveWorkoutProgramDto);
            if (result.Action != true) return BadRequest(result);
            return Ok(result);
            
        }
        [HttpGet("dashboard")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetDashboard()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) 
                return Unauthorized(new ApiResponse { Action = false, Message = "خطای احراز هویت." });

            var result = await coachRepository.GetCoachDashboard(phoneNumber);

            if (!result.Action) 
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("dashboard/monthly-chart/{year}/{month}")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetMonthlyChart([FromRoute] int year, [FromRoute] int month)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) 
                return Unauthorized(new ApiResponse { Action = false, Message = "خطای احراز هویت." });

            var result = await coachRepository.GetMonthlyIncomeChart(phoneNumber, year, month);

            if (!result.Action) 
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPut("UpdateSocialMediaLink")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> UpdateSocialMediaLink([FromBody] SocialMediaLinkDto socialMediaLinkDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) 
                return Unauthorized(new ApiResponse { Action = false, Message = "خطای احراز هویت." });

            var result = await coachRepository.UpdateSocialMediaLink(phoneNumber,socialMediaLinkDto);

            if (!result.Action) 
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("GetSocialMediaLink")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetSocialMediaLink()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) 
                return Unauthorized(new ApiResponse { Action = false, Message = "خطای احراز هویت." });

            var result = await coachRepository.GetSocialMediaLink(phoneNumber);

            if (!result.Action) 
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("GetStudentList")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetStudentList()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new ApiResponse { Action = false, Message = "خطای احراز هویت." });
            }

            var result = await coachRepository.GetAthletesWithStatus(phoneNumber);

            if (!result.Action)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpGet("GetTransactionList")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetTransactionList()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new ApiResponse { Action = false, Message = "خطای احراز هویت." });
            }

            var result = await coachRepository.GetTransactions(phoneNumber);

            if (!result.Action)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpGet("GetFaq")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> GetFaq()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Unauthorized(new ApiResponse { Action = false, Message = "خطای احراز هویت." });
            }

            var result = await coachRepository.GetFaq();

            if (!result.Action)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
      
    }

   
}

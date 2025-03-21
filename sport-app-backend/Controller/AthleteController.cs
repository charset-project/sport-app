using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Mappers;
using sport_app_backend.Models;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AthleteController : ControllerBase
    {
        private readonly IAthleteRepository _athleteRepository;
        private readonly ApplicationDbContext _context;


        public AthleteController(IAthleteRepository athleteRepository, ApplicationDbContext context)
        {
            _athleteRepository = athleteRepository;
            _context = context;
        }
        [HttpPost("add athlete question")]
        [Authorize(Roles = "Athlete")]

        public async Task<IActionResult> AddAthleteQuestion([FromBody] AthleteQuestionDto athleteQuestionDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");

            var result = await _athleteRepository.SubmitAthleteQuestions(phoneNumber, athleteQuestionDto);
            if (!result.Action) return BadRequest(result.Message);
            return Ok();
        }

        [HttpPost("add_water_intake")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> AddWaterIntake([FromBody] WaterInTakeDto waterInTakeDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");

            var result = await _athleteRepository.AddWaterIntake(phoneNumber, waterInTakeDto);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }


        [HttpGet("get_water_intake")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetWaterIntake()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var user = await _context.Users
                .Include(u => u.Athlete)
                .ThenInclude(a => a.WaterInTake)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user is null || user.Athlete == null || user.Athlete.WaterInTake == null)
            {
                return NotFound("Water intake not found");
            }
            else
            {
                return Ok(new ApiResponse()
                {
                    Action = true,
                    Message = "Water intake found",
                    Result = new WaterInTakeDto()
                    {
                        DailyCupOfWater = user.Athlete.WaterInTake.DailyCupOfWater,
                        Reminder = user.Athlete.WaterInTake.Reminder
                    }
                });
            }
        }



        [HttpPut("update_water_inDay")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> UpdateWaterInDay()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await _athleteRepository.UpdateWaterInDay(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }


        [HttpGet("get_water_in_day")]
        [Authorize(Roles = "Athlete")]
        // This endpoint retrieves the water intake records for the current day for the authenticated athlete.
        //this data for last30 days but in Hijri solar calendar
        public async Task<IActionResult> GetWaterInDayForLast7Days()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var athlete = await _context.Users
                .Include(u => u.Athlete)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (athlete == null || athlete.Athlete == null) return NotFound("Athlete not found");
            var ListOfWaterInDay = await _context.WaterInDays
                .Where(w => w.AthleteId == athlete.Athlete.Id && w.Date.Date >= DateTime.UtcNow.Date.AddDays(-70))
                .ToListAsync();

            return Ok(new ApiResponse()
            {
                Action = true,
                Message = "Water intake found",
                Result = ListOfWaterInDay.Select(w => new WaterInDayDto()
                {

                    NumberOfCupsDrinked = w.NumberOfCupsDrinked,
                    Date = w.Date.ToString("yyyy-MM-dd") // Format the date as needed
                })
            });

        }

        [HttpGet("get_Athlete_profile")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetAthleteProfile()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse() { Action = false, Message = "PhoneNumber is null" });
            var user = await _context.Users
                .Include(u => u.Athlete).FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user is null) return BadRequest(new ApiResponse() { Action = false, Message = "User not found" });

            return Ok(new ApiResponse() { Action = true, Message = "User found", Result = user.ToAthleteProfileResponseDto() });

        }

        [HttpGet("search_Coaches")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> SearchCoaches([FromBody] string FullName)
        {
           var resualt = await _context.Users.Where(c => (c.FirstName+" "+c.LastName).Contains(FullName)&& 
           c.TypeOfUser == TypeOfUser.COACH).ToListAsync();
            return Ok(new ApiResponse() { Action = true, Message = "Coaches found", Result = resualt.Select(c => c.ToCoachForSearch()).ToList() });

           
        }
        [HttpGet("Get-Coaches")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetCoaches()
        {
            var resualt = await _context.Users.Where(c => c.TypeOfUser == TypeOfUser.COACH).ToListAsync();
            return Ok(new ApiResponse() { Action = true, Message = "Coaches found", Result = resualt.Select(c => c.ToCoachForSearch()).ToList() });
        }


    }

}
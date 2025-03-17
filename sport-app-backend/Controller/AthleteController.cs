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
            if (!result) return BadRequest("Failed to add athlete question");
            return Ok(new { Message = "Athlete question added successfully" });
        }

        [HttpPost("add_water_intake")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> AddWaterIntake([FromBody] WaterInTakeDto waterInTakeDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");

            var result = await _athleteRepository.AddWaterIntake(phoneNumber, waterInTakeDto);
            if (!result) return BadRequest("Failed to add water intake");
            return Ok(new { Message = "Water intake added successfully" });
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
                  if (user is null || user.Athlete == null || user.Athlete.WaterInTake == null) {
                      return NotFound("Water intake not found");
                  }else{
                      return Ok(new WaterInTakeDto{
                          DailyCupOfWater = user.Athlete.WaterInTake.DailyCupOfWater,
                          Reminder = user.Athlete.WaterInTake.Reminder});
                  }
        }



        [HttpPut("update_water_inDay")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> UpdateWaterInDay()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await _athleteRepository.UpdateWaterInDay(phoneNumber);
            if (!result) return BadRequest("Failed to update water in day");
            return Ok(new { Message = "Water in day updated successfully" });
        }  


        [HttpGet("get_water_in_day")]
        [Authorize(Roles = "Athlete")]
        // This endpoint retrieves the water intake records for the current day for the authenticated athlete.
        //this data for last30 days but in Hijri solar calendar
        public async Task<IActionResult> GetWaterInDay()
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

            return Ok(ListOfWaterInDay.Select(w => new WaterInDayDto()
            {
            
                NumberOfCupsDrinked = w.NumberOfCupsDrinked,
                Date = w.Date.ToString("yyyy-MM-dd") // Format the date as needed
            }));
                
        }


    }
}
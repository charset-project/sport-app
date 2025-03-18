using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;

namespace sport_app_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AthleteController : ControllerBase
    {
        private readonly IAthleteRepository _athleteRepository;

        public AthleteController(IAthleteRepository athleteRepository)
        {
            _athleteRepository = athleteRepository;
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
    }
}
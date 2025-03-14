using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Models;
using sport_app_backend.Models.Account;
using sport_app_backend.Repository;
using System.Security.Claims;

namespace sport_app_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoacheController(ICoachRepository coachRepository) : ControllerBase
    {
        private readonly ICoachRepository _coachRepository = coachRepository;


        [HttpPost("CoachQuastions")]
        [Authorize(Roles="Coach")]
        public async Task<IActionResult> SubmitCoachQuastions([FromBody] CoachQuestionDto coachQuestionDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");

            var result = await _coachRepository.SubmitCoachQuestions(phoneNumber, coachQuestionDto);
            if (!result) return BadRequest("Failed to submit coach questions.");

            return Ok(new { Message = "Coach questions submitted successfully" });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;

namespace sport_app_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        [HttpPost("add_Exercises")]
        public async Task<IActionResult> AddExercises([FromBody] List<AddExercisesRequestDto> exercises)
        {
            return Ok(await _adminRepository.AddExercises(exercises));
        }
       
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sport_app_backend.Dtos;

namespace sport_app_backend.Interface
{
    public interface IAdminRepository
    {
        Task<bool> AddExercises(List<AddExercisesRequestDto> exercises);
        
    }
}
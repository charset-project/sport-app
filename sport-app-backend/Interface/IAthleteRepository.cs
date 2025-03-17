using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sport_app_backend.Dtos;

namespace sport_app_backend.Interface
{
    public interface IAthleteRepository
    {
        public Task<bool> SubmitAthleteQuestions(string phoneNumber, AthleteQuestionDto AthleteQuestionDto);
        public Task<bool> AddWaterIntake(string phoneNumber, WaterInTakeDto waterInTakeDto);
        public Task<bool> UpdateWaterInDay(string phoneNumber);
    }
}
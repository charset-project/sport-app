using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sport_app_backend.Dtos;
using sport_app_backend.Models;

namespace sport_app_backend.Interface
{
    public interface IAthleteRepository
    {
        public Task<ApiResponse> SubmitAthleteQuestions(string phoneNumber, AthleteQuestionDto AthleteQuestionDto);
        public Task<ApiResponse> AddWaterIntake(string phoneNumber, WaterInTakeDto waterInTakeDto);
        public Task<ApiResponse> UpdateWaterInDay(string phoneNumber);
        public Task<ApiResponse> UpdateWeight(string phoneNumber, double weight);
        public Task<ApiResponse> WeightReport(string phoneNumber);
        
    }
}
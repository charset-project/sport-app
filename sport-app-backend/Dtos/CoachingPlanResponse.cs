using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sport_app_backend.Dtos
{
    public class CoachingPlanResponse
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required double Price { get; set; }
        public required int DurationByDay { get; set; }
        public bool IsActive { get; set; }

        public string TypeOfCoachingPlan { get; set; }

    }
}
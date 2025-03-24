using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sport_app_backend.Dtos;
using sport_app_backend.Models.Actions;

namespace sport_app_backend.Mappers
{
    public static class ExercisesMappers
    {
        public static Exercise ToExercisesDto(this AddExercisesRequestDto addExercisesRequestDto)
        {
            return new Exercise
            {
                EnglishName = addExercisesRequestDto.EnglishName,
                PersianName = addExercisesRequestDto.PersianName,
                MainImage = addExercisesRequestDto.MainImage,
                AnatomyImage = addExercisesRequestDto.AnatomyImage,
                Description = addExercisesRequestDto.Description,
                Calories = addExercisesRequestDto.Calories,
                Image = addExercisesRequestDto.Image,
                BaseCategory = (ExerciseEnum)Enum.Parse(typeof(ExerciseEnum), addExercisesRequestDto.BaseCategory.ToUpper()),
                ActionsTage = addExercisesRequestDto.ActionsTage.Select(x => (ExerciseEnum)Enum.Parse(typeof(ExerciseEnum), x.ToUpper())).ToList()};
        }
    }
}
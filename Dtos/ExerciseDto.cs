﻿namespace sport_app_backend.Dtos;

public class ExerciseDto
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string ExerciseCategories { get; set; }
    public string Equipment { get; set; }
    public List<string> Muscles { get; set; }
    public string EnglishName { get; set; }
    public string PersianName { get; set; }
    public string ImageLink { get; set; }
    public string VideoLink { get; set; }
    public string BaseCategory { get; set; }
    public string ExerciseLevel { get; set; }
    public string Mechanics { get; set; }
   


}
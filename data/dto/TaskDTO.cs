using System;

namespace p48_lessons_asp.data.dto;

public class TaskDTO
{
    public bool completed{get;set;}
    public string Title{get;set;}
    public string? description{get;set;}
    public int day_deadline_period{get;set;}
}

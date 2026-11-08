using System;
using p48_lessons_asp.data.dto;

namespace p48_lessons_asp.data;

/*
id-GUID
completed-BOOL
title-STRING
description?-STRING
created_at-DATETIME
deadline_date-DATETIME
*/
public class UserTask
{
    public Guid Id {get;set;}
    public bool completed{get;set;}
    public string Title{get;set;}
    public string? description{get;set;}
    public DateTime CreatedAt{get;set;}
    public DateTime DeadlineDate{get;set;}

    public UserTask(TaskDTO DTOTask)
    {
        Id = Guid.NewGuid();
        completed = DTOTask.completed;
        Title = DTOTask.Title;
        description = DTOTask.description;
        CreatedAt = DateTime.Now;
        DeadlineDate = new DateTime(CreatedAt.Year, CreatedAt.Month, CreatedAt.Day+DTOTask.day_deadline_period, CreatedAt.Hour, CreatedAt.Minute, CreatedAt.Second, DateTimeKind.Local);
    }
}

using System;

namespace p48_lessons_asp.data.dto;

public class ReturnObject
{
    public int status {get;set;}
    public string message {get;set;}
    public dynamic? data {get;set;}
    public string? error{get;set;}
}

using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using p48_lessons_asp.data;
using p48_lessons_asp.data.dto;

// Файл старту, тут запускається сервер

// як запустити ASP проєкт
// # запуск проєкту 
// dotnet run 
namespace p48_lessons_asp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            List<UserTask> userTasks = new()
            {
              new(new TaskDTO{completed = true, Title = "Completed Task", description = "A completed Task.", day_deadline_period = 7}),
              new(new TaskDTO{completed = false, Title = "Expired Task", description = "An expired Task.", day_deadline_period = -1}),
              new(new TaskDTO{completed = false, Title = "Uncompleted Task", description = "An uncompleted Task.", day_deadline_period = 7})
            };

            string baseToken = "USER APPROVED";
            byte[] bytes = Encoding.UTF8.GetBytes(baseToken);
            string authenticationToken = Convert.ToBase64String(bytes);

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.MapGet("/api/tasks/{typeOfTasks?}", (string? typeOfTasks) =>
            {
                if (string.IsNullOrEmpty(typeOfTasks))
                {
                    return Results.Ok(new ReturnObject
                    {
                        status = 200,
                        message = "Ok",
                        data = userTasks
                    });
                }
                else
                {
                    if (typeOfTasks == "completed")
                    {
                        return Results.Ok(new ReturnObject
                        {
                            status = 200,
                            message = "Ok",
                            data = userTasks.Where(ut => ut.completed).ToList()
                        });
                    }
                    else if (typeOfTasks == "expired")
                    {
                        DateTime rightNow = DateTime.Now;
                        return Results.Ok(new ReturnObject
                        {
                            status = 200,
                            message = "Ok",
                            data = userTasks.Where(ut => ut.DeadlineDate < rightNow).ToList()
                        });
                    }
                    else if (typeOfTasks == "current")
                    {
                        return Results.Ok(new ReturnObject
                        {
                            status = 200,
                            message = "Ok",
                            data = userTasks.Where(ut => !ut.completed).ToList()
                        });
                    }
                    else
                    {
                        return Results.BadRequest(new ReturnObject
                        {
                            status = 200,
                            message = "Go to a valid end point",
                            error = "Invalid endpoint"
                        });
                    }
                }
            });

            app.MapGet("/api/task/{id}", (string id) =>
            {
                try
                {
                    Guid true_id = Guid.Parse(id);
                    UserTask? userTask = userTasks.FirstOrDefault(ut => ut.Id == true_id);
                    if (userTask is not null)
                    {
                        return new ReturnObject()
                        {
                            status = StatusCodes.Status200OK,
                            message = "User found",
                            data = userTask
                        };
                    }
                    else
                    {
                        return new ReturnObject()
                        {
                            status = StatusCodes.Status400BadRequest,
                            message = "Input a valid id",
                            error = "Non-existant object"
                        };
                    }
                }
                catch (FormatException)
                {
                    return new ReturnObject()
                    {
                        status = StatusCodes.Status400BadRequest,
                        message = "Wrong id format",
                        error = "Wrong id format"
                    };
                }
            });

            app.MapPost("/api/tasks/create", (TaskDTO? dTO, HttpContext context) =>
            {
                string sentToken = "";

                if (context.Request.Headers.TryGetValue("authenticationKey", out var token))
                {
                    sentToken = token.ToString();
                }
                if (sentToken != authenticationToken)
                {
                    return Results.Json(
                        new ReturnObject
                        {
                            status = StatusCodes.Status403Forbidden,
                            message = "Authentication failed",
                            error = "Invalid authentication key"
                        });
                }
                if (dTO is null)
                {
                    return Results.BadRequest(
                        new ReturnObject
                        {
                            status = StatusCodes.Status400BadRequest,
                            message = "Send a correct object type",
                            error = "no object sent"
                        }
                    );
                }
                else
                {
                    UserTask userTask = new(dTO);
                    userTasks.Add(userTask);
                    return Results.Ok(
                        new ReturnObject
                        {
                            status = StatusCodes.Status200OK,
                            message = "Object added",
                            data = userTask
                        }
                    );
                }
            });

            app.MapDelete("/api/task/{id}", (string id, HttpContext context) =>
            {
                string sentToken = "";

                if (context.Request.Headers.TryGetValue("authenticationKey", out var token))
                {
                    sentToken = token.ToString();
                }
                if (sentToken != authenticationToken)
                {
                    return Results.Json(
                        new ReturnObject
                        {
                            status = StatusCodes.Status403Forbidden,
                            message = "Authentication failed",
                            error = "Invalid authentication key"
                        });
                }
                try
                {
                    Guid true_id = Guid.Parse(id);
                    UserTask? userTask = userTasks.FirstOrDefault(ut => ut.Id == true_id);
                    if (userTask is not null)
                    {
                        List<UserTask> newList = userTasks.Where(ut => ut.Id != true_id).ToList();
                        userTasks = newList;
                        return Results.Ok(new ReturnObject()
                        {
                            status = StatusCodes.Status200OK,
                            message = "User deleted"
                        });
                    }
                    else
                    {
                        return Results.BadRequest(new ReturnObject()
                        {
                            status = StatusCodes.Status400BadRequest,
                            message = "Input a valid id",
                            error = "Non-existant object"
                        });
                    }
                }
                catch (FormatException)
                {
                    return Results.BadRequest(new ReturnObject()
                    {
                        status = StatusCodes.Status400BadRequest,
                        message = "Wrong id format",
                        error = "Wrong id format"
                    });
                }
            });

            app.Run();

        }
    }
}
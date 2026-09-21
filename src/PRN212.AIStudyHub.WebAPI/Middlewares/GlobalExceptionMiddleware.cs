using System.Net;
using System.Text.Json;
using PRN212.AIStudyHub.Application.DTOs.Common;
using PRN212.AIStudyHub.Application.Exceptions;

namespace PRN212.AIStudyHub.WebAPI.Middlewares;

public class GlobalExceptionMiddleware(
	RequestDelegate next,
	ILogger<GlobalExceptionMiddleware> logger,
	IHostEnvironment env)
{
  public async Task InvokeAsync(HttpContext context)
  {
	try
	{
	  await next(context);
	}
	catch (Exception ex)
	{
	  await HandleExceptionAsync(context, ex);
	}
  }

  private async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
	var (statusCode, message) = exception switch
	{
	  BadRequestException ex => (HttpStatusCode.BadRequest, ex.Message),
	  ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),
	  UnauthorizedException ex => (HttpStatusCode.Unauthorized, ex.Message),
	  ForbiddenException ex => (HttpStatusCode.Forbidden, ex.Message),
	  UnauthorizedAccessException ex => (HttpStatusCode.Forbidden, ex.Message),
	  NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
	  KeyNotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
	  ConflictException ex => (HttpStatusCode.Conflict, ex.Message),
	  CloudStorageException ex => (HttpStatusCode.BadGateway, ex.Message),
	  InvalidOperationException ex => (HttpStatusCode.BadRequest, ex.Message),
	  _ => (HttpStatusCode.InternalServerError, "An unexpected internal server error occurred.")
	};

	if (statusCode == HttpStatusCode.InternalServerError)
	{
	  logger.LogError(exception, "Unhandled server error processing {Method} {Path}: {Message}",
		  context.Request.Method, context.Request.Path, exception.Message);
	}
	else
	{
	  logger.LogWarning("Handled business exception on {Method} {Path} -> {StatusCode}: {Message}",
		  context.Request.Method, context.Request.Path, (int)statusCode, message);
	}

	context.Response.ContentType = "application/json";
	context.Response.StatusCode = (int)statusCode;

	List<string>? errors = null;
	if (env.IsDevelopment() && statusCode == HttpStatusCode.InternalServerError)
	{
	  errors = new List<string>
	  {
		exception.Message,
		exception.StackTrace ?? string.Empty
	  };
	}

	var response = ApiResponse.FailureResponse(message, errors);
	var jsonOptions = new JsonSerializerOptions
	{
	  PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
  }
}

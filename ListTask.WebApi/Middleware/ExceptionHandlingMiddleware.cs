using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ListTask.WebApi.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            var status = MapToStatusCode(ex);
            var problem = BuildProblemDetails(ctx, ex, status);

            var logger = Log.ForContext("TraceId", ctx.TraceIdentifier)
                            .ForContext("Path", ctx.Request.Path.Value)
                            .ForContext("Method", ctx.Request.Method)
                            .ForContext("StatusCode", status);

            if ((int)status >= 500)
                logger.Error(ex, "Unhandled exception");
            else
                logger.Warning(ex, "Handled client error");

            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = (int)status;

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            await ctx.Response.WriteAsync(json);
        }
    }

    private static HttpStatusCode MapToStatusCode(Exception ex) => ex switch
    {
        ArgumentException or ArgumentNullException or ArgumentOutOfRangeException or FormatException
            => HttpStatusCode.BadRequest,

        UnauthorizedAccessException
            => HttpStatusCode.Forbidden,

        KeyNotFoundException
            => HttpStatusCode.NotFound,

        NotSupportedException
            => HttpStatusCode.MethodNotAllowed,

        InvalidOperationException or DbUpdateConcurrencyException or DbUpdateException
            => HttpStatusCode.Conflict,

        _ => HttpStatusCode.InternalServerError
    };

    private static object BuildProblemDetails(HttpContext ctx, Exception ex, HttpStatusCode status) => new
    {
        type = $"https://httpstatuses.io/{(int)status}",
        title = GetTitle(status),
        status = (int)status,
        detail = GetClientSafeDetail(ex, status),
        traceId = ctx.TraceIdentifier,
        path = ctx.Request.Path.Value
    };

    private static string GetTitle(HttpStatusCode status) => status switch
    {
        HttpStatusCode.BadRequest => "Bad Request",
        HttpStatusCode.Unauthorized => "Unauthorized",
        HttpStatusCode.Forbidden => "Forbidden",
        HttpStatusCode.NotFound => "Not Found",
        HttpStatusCode.MethodNotAllowed => "Method Not Allowed",
        HttpStatusCode.Conflict => "Conflict",
        _ => "Internal Server Error"
    };

    private static string GetClientSafeDetail(Exception ex, HttpStatusCode status)
    {
        if ((int)status is >= 400 and < 500)
            return string.IsNullOrWhiteSpace(ex.Message) ? null : ex.Message;

        return "An unexpected error occurred. Please contact support if the problem persists.";
    }
}
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zuhid.Notification.Shared;

public class ResultFilter : IResultFilter
{
    /// <summary>
    /// It changes the status code from 200 OK to 202 Accepted to indicate that the request 
    /// has been accepted for asynchronous processing, but the processing has not yet been completed.
    /// </summary>
    /// <param name="context">The result executing context.</param>
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.HttpContext.Response.StatusCode == StatusCodes.Status200OK)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}

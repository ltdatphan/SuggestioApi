using System.Net;
using System.Text;
using Ganss.Xss;
using Microsoft.Extensions.Primitives;
using SuggestioApi.Helpers.Sanitize;

namespace SuggestioApi.RequestPipeline;

public static class WebApplicationExtensions
{
    public static WebApplication UseCsrfMiddleware(this WebApplication app)
    {
        app.Use(next => async context =>
        {
            if (HttpMethods.IsPost(context.Request.Method) ||
                HttpMethods.IsPut(context.Request.Method) ||
                HttpMethods.IsDelete(context.Request.Method))
            {
                if (context.Request.Path == "/api/account/login" ||
                    context.Request.Path == "/api/account/register" ||
                    context.Request.Path == "/api/account/refresh-token" ||
                    context.Request.Path == "/api/account/revoke-token")
                {
                    // Bypass CSRF for these endpoints
                    await next(context);
                    return;
                }

                var csrfTokenFromHeader = context.Request.Headers["X-CSRF-TOKEN"].ToString();
                var csrfTokenFromCookie = context.Request.Cookies["CSRF-TOKEN"];

                // Decode the CSRF token from the header
                var decodedCsrfTokenFromHeader = Uri.UnescapeDataString(csrfTokenFromHeader);

                if (decodedCsrfTokenFromHeader != csrfTokenFromCookie)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync("CSRF token validation failed");
                    return;
                }
            }

            await next(context);
        });
        return app;
    }

    public static WebApplication UseSanitizeMiddleware(this WebApplication app)
    {
        app.Use(next => async context =>
        {
            var httpContext = context.Request.HttpContext;

            httpContext.Request.EnableBuffering();

            foreach (var key in context.Request.RouteValues.Keys.ToList())
            {
                context.Request.RouteValues[key] = Sanitize.SanitizeInput(context.Request.RouteValues[key]?.ToString());
            }

            // Sanitize query parameters
            var queryCollection = context.Request.Query;
            var sanitizedQuery = new QueryCollection(queryCollection.ToDictionary(
                kvp => kvp.Key,
                kvp => new StringValues(kvp.Value.Select(value => value).ToArray())
            ));
            context.Request.Query = sanitizedQuery;

            // Enable buffering to read the request body
            context.Request.EnableBuffering();

            // Sanitize request body
            using (var streamReader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var raw = await streamReader.ReadToEndAsync();
                var sanitiser = new HtmlSanitizer();
                var sanitised = sanitiser.Sanitize(raw);

                if (raw != sanitised)
                {
                    throw new BadHttpRequestException("XSS injection detected from middleware.");
                }

                // Reset the body stream with sanitized data
                var sanitizedBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(sanitised));
                context.Request.Body = sanitizedBodyStream;
            }
            await next(context);
        });

        return app;
    }
}
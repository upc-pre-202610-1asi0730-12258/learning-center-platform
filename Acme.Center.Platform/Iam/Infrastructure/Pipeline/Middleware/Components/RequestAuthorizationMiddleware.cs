using Microsoft.AspNetCore.Http;
using Acme.Center.Platform.Iam.Application.Internal.OutboundServices;
using Acme.Center.Platform.Iam.Application.QueryServices;
using Acme.Center.Platform.Iam.Domain.Model.Queries;
using Acme.Center.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace Acme.Center.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

/**
 * RequestAuthorizationMiddleware is custom middleware.
 * This middleware is used to authorize requests.
 * It validates a token is included in the request header and that the token is valid.
 * If the token is valid, then it sets the user in `HttpContext.Items["User"]`.
 */
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    /**
     * InvokeAsync is called by the ASP.NET Core runtime.
     * It is used to authorize requests.
     * It validates a token is included in the request header and that the token is valid.
     * If the token is valid, then it sets the user in HttpContext.Items["User"].
     */
    public async Task InvokeAsync(
        HttpContext context,
        IUserQueryService userQueryService,
        ITokenService tokenService)
    {
        CancellationToken cancellationToken = context.RequestAborted;
        Console.WriteLine("Entering InvokeAsync");
        // skip authorization if endpoint is decorated with [AllowAnonymous] attribute
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata.Any(m => m.GetType() == typeof(AllowAnonymousAttribute)) ?? false;
        Console.WriteLine($"Allow Anonymous is {allowAnonymous}");
        if (allowAnonymous)
        {
            Console.WriteLine("Skipping authorization");
            // [AllowAnonymous] attribute is set, so skip authorization
            await next(context);
            return;
        }

        Console.WriteLine("Entering authorization");
        // get token from request header
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        Console.WriteLine($"Authorization header: {authHeader}");
        var token = authHeader?.Split(" ").Last();
        Console.WriteLine($"Extracted token: {token}");


        // if token is null then return unauthorized
        if (token == null)
        {
            Console.WriteLine("Token is null");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // validate token
        var userId = await tokenService.ValidateToken(token);
        Console.WriteLine($"Token validation result: {userId}");

        // if token is invalid then return unauthorized
        if (userId == null)
        {
            Console.WriteLine("Token is invalid");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // get user by id
        var getUserByIdQuery = new GetUserByIdQuery(userId.Value);

        // set user in HttpContext.Items["User"]
        var user = await userQueryService.Handle(getUserByIdQuery, cancellationToken);
        context.Items["User"] = user;
        
        // call next middleware
        await next(context);
    }
}
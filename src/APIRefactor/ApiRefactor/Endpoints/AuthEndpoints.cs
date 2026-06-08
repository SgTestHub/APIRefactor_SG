using ApiRefactor.Data;
using ApiRefactor.Models;
using ApiRefactor.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ApiRefactor.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/auth")
                .WithTags("Authentication");

            group.MapPost("/login", Login)
                .WithName("Login")
                .WithOpenApi()
                .Produces<APIResponse>(StatusCodes.Status200OK)
                .Produces<APIResponse>(StatusCodes.Status401Unauthorized)
                .Accepts<LoginRequest>("application/json");
        }

        private static async Task<IResult> Login(LoginRequest request, IJwtTokenService tokenService, WavesDbContext db, ILogger<Program> logger)
        {
            try
            {
                // Validate request
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    logger.LogWarning("Login attempt with missing credentials");
                    return Results.Unauthorized();
                }

                // Validate credentials against demo users
                var user = await db.LocalUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
                if (user == null || user.Password != request.Password)
                {
                    logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                    return Results.Unauthorized();
                }

                // Generate JWT token
                var token = tokenService.GenerateToken(user);

                logger.LogInformation("User {Username} logged in successfully", request.Email);

                return Results.Ok(new APIResponse
                {
                    IsSuccess = true,
                    Result = new LoginResponse
                    {
                        Token = token.ToString(),
                        Email = user.Email,
                        Role = user.Role
                    },
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login");
                return Results.Problem("An error occurred during login");
            }
        }
    }
}

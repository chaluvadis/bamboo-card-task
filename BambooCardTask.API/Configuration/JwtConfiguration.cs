namespace BambooCardTask.Configuration;

public static class JwtConfiguration
{
    public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT secret key is not configured.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero, // Reduce clock skew for token expiration
                    RequireSignedTokens = true, // Ensure tokens are signed
                    ValidAlgorithms = ["HS512"], // Explicitly enforce HS512 algorithm
                    RoleClaimType = ClaimTypes.Role, // Map role claim
                    NameClaimType = ClaimTypes.Email // Map email claim
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log.Warning("Authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Log.Warning("Authentication challenge: {Error}", context.ErrorDescription);
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        Log.Warning("Access forbidden: {Path}", context.HttpContext.Request.Path);
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
            .AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    }
}

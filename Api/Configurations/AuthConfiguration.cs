using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Configurations;

public static class AuthConfiguration
{
    public static void AddUserAuthentication(this WebApplicationBuilder builder)
    {
        var authUserSettingsConfiguration = builder.Configuration.GetSection(nameof(AuthUserSettings));
        var authUserSettings = authUserSettingsConfiguration.Get<AuthUserSettings>()!;

        builder.Services.Configure<AuthUserSettings>(authUserSettingsConfiguration);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = authUserSettings.Issuer,
                ValidAudience = authUserSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authUserSettings.Key)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });
        builder.Services.AddAuthentication();
    }
}
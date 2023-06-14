//Created by IdentityInstallerClassCreator at 8/5/2022 7:36:07 AM

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebInstallers;

namespace IdentityTools;

// ReSharper disable once UnusedType.Global
public class IdentityInstaller : IInstaller
{
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public void InstallServices(WebApplicationBuilder builder, string[] args)
    {
        Console.WriteLine("identityInstaller.InstallServices Started");

        //builder.Services.AddScoped<IUserStore<AppUser>, MyUserStore>();
        //builder.Services.AddScoped<IUserPasswordStore<AppUser>, MyUserStore>();
        //builder.Services.AddScoped<IUserEmailStore<AppUser>, MyUserStore>();
        //builder.Services.AddScoped<IUserRoleStore<AppUser>, MyUserStore>();
        //builder.Services.AddScoped<IRoleStore<AppRole>, MyUserStore>();

        builder.Services.AddIdentity<IdentityUser<int>, IdentityRole<int>>(options =>
        {
            options.Password.RequiredLength = 3;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
        }).AddDefaultTokenProviders();

        string? jwtSecret = builder.Configuration["IdentitySettings:JwtSecret"];
        if (jwtSecret is null)
        {
            throw new Exception("jwtSecret is null");
        }

        byte[] key = Encoding.ASCII.GetBytes(jwtSecret);
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, ValidateAudience = false
            };
        });

        Console.WriteLine("identityInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
        Console.WriteLine("identityInstaller.UseServices Started");
        app.UseAuthentication();
        app.UseAuthorization();
        Console.WriteLine("identityInstaller.UseServices Finished");
    }
}
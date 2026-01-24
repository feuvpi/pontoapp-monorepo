using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PontoAPP.Application.Behaviours;
using PontoAPP.Application.Queries.Tenants;
using PontoAPP.Domain.Repositories;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Data.Context;
using PontoAPP.Infrastructure.Data.Interceptors;
using PontoAPP.Infrastructure.Data.Repositories;
using PontoAPP.Infrastructure.Identity;
using PontoAPP.Infrastructure.Multitenancy;
using PontoAPP.Infrastructure.Services;
using PontoAPP.Application.Commands.Tenants; 

namespace PontoAPP.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPontoAppDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

        // Interceptors
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<TenantInterceptor>();

        // Único DbContext unificado
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
            var tenantInterceptor = serviceProvider.GetRequiredService<TenantInterceptor>();

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                npgsqlOptions.CommandTimeout(30);
            });

            options.AddInterceptors(auditInterceptor, tenantInterceptor);

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        return services;
    }

    public static IServiceCollection AddPontoAppRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITimeRecordRepository, TimeRecordRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<ITimeRecordAdjustmentRepository, TimeRecordAdjustmentRepository>();
        return services;
    }

    public static IServiceCollection AddPontoAppServices(this IServiceCollection services)
    {
        // Domain Services - Portaria 671
        services.AddScoped<INSRGenerator, NSRGenerator>();
        services.AddScoped<ISignatureGenerator, SignatureGenerator>();
        // Removido: ITenantSchemaService
        services.AddScoped<IDomainEventService, DomainEventService>();
        
        services.AddScoped<IAFDGenerator, AFDGenerator>();
        services.AddScoped<IACJEFGenerator, ACJEFGenerator>();
        services.AddScoped<IEspelhoPontoGenerator, EspelhoPontoGenerator>();
        services.AddScoped<ICRPGenerator, CRPGenerator>();

        return services;
    }
    
    public static IServiceCollection AddPontoAppIdentity(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IClaimsService, ClaimsService>();
        
        return services;
    }

    public static IServiceCollection AddPontoAppMultitenancy(this IServiceCollection services)
    {
        services.AddScoped<ITenantAccessor, TenantAccessor>();
        services.AddScoped<ITenantResolver, TenantResolver>();

        return services;
    }

    public static IServiceCollection AddPontoAppMediatr(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<GetTenantByIdQuery>();

            cfg.RegisterServicesFromAssembly(typeof(ValidationBehavior<,>).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        return services;
    }
    
    public static IServiceCollection AddPontoAppValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ValidationBehavior<,>).Assembly);
        return services;
    }

    public static IServiceCollection AddPontoAppAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSecret = configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret not configured");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }
}
﻿using App.Models.Dbcontexts;
using App.Models.DTOs;
using App.Models.Entities.Identities;
using MaxV.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Infrastructures.Startups.ServicesExtensions
{
    public static class GeneralServiceExtension
    {
        public static void AddGeneralConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSection = configuration.GetSection("CorsOptions");
            if (corsSection == null)
            {
                throw new NullReferenceException(nameof(corsSection));
            }
            var corsOption = corsSection.Get<CorsOptions>();
            var policyName = corsOption.PolicyName.Nullify("AppCorsPolicy");
            services.AddCors(c =>
            {
                c.AddPolicy(policyName, options =>
                {
                    options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    if (corsOption.AllowedOrigins.IsAllowedAll())
                    {
                        options.AllowAnyOrigin();
                    }
                    else
                    {
                        options.WithOrigins(corsOption.AllowedOrigins);
                    }

                    if (corsOption.AllowedMethods.IsAllowedAll())
                    {
                        options.AllowAnyMethod();
                    }
                    else
                    {
                        options.WithMethods(corsOption.AllowedMethods);
                    }

                    if (corsOption.ExposedHeaders.IsAllowedAll())
                    {
                        options.AllowAnyHeader();
                    }
                    else
                    {
                        options.WithHeaders(corsOption.ExposedHeaders);
                    }
                });
            });

            services.Configure<ConnectionString>(configuration.GetSection("ConnectionStrings"));
            var connectionStrings = configuration.GetSection("ConnectionStrings").Get<ConnectionString>();
            services.Configure<JwtOptions>(configuration.GetSection("JWT"));
            var jwtOptions = configuration.GetSection("JWT").Get<JwtOptions>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.EnableDetailedErrors(true);

                options.UseSqlServer(connectionStrings.DefaultConnection);
                options.UseSnakeCaseNamingConvention();
            });
            services.AddIdentity<User, Role>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDistributedMemoryCache();

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.ValidAudience,
                    ValidIssuer = jwtOptions.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "App Api", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"Example Token: 'Bearer {Token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oath2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }
        private static bool IsAllowedAll(this string[] values)
        {
            return values == null || values.Length == 0 || values.Contains("*");
        }
    }
}
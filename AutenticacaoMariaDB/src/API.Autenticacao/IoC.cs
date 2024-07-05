using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text.Json.Serialization;
using Dominio;
using Infraestrutura.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System;
using Infraestrutura.Data.EF;
using Core.Interfaces;
using Core.Services;
using Middlewares;
using Microsoft.AspNetCore.Http;
using NetDevPack.Security.Jwt.Core.Jwa;

namespace API.Autenticacao
{
    public static class IoC
    {

        public static void AddCustomServices(this IServiceCollection app, IConfiguration configuration)
        {
            app.AddControllers();

            app.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            app.AddDbContext<APIDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("BancoDados"), serverVersion,
                mysqlOptions => mysqlOptions.EnableRetryOnFailure()));
            app.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            app.AddScoped<IUsuarioService, UsuarioService>();
            app.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            app.AddMemoryCache();

        }

        public static void AddSecurity(this IServiceCollection app, IConfiguration configuration)
        {
            app.AddJwksManager(o =>
            {
                o.Jws = Algorithm.Create(DigitalSignaturesAlgorithm.RsaSha256);
                o.Jwe = Algorithm.Create(EncryptionAlgorithmKey.RsaOAEP).WithContentEncryption(EncryptionAlgorithmContent.Aes128CbcHmacSha256);

            })
            .UseJwtValidation()
            .PersistKeysToDatabaseStore<APIDbContext>();

            app.AddDefaultIdentity<Usuario>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<APIDbContext>()
            .AddDefaultTokenProviders();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

            app.AddAuthorization();
            app.AddHealthChecks();
            app.AddHttpClient();
        }

        public static void AddCustomSwagger(this IServiceCollection app)
        {
            app.AddEndpointsApiExplorer();
            app.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "DribionPay.API",
                        Description = "API de Pagamento",
                        Version = "v1"
                    });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                options.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
            });
        }

        public static void UseCustomEndpoints(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./v1/swagger.json", "DribionPay.API v1");
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseJwksDiscovery();

            app.UseMiddleware<AutenticacaoAPIMiddlewareException>();

            app.MapGet("/", async context =>
            {
                await context.Response.WriteAsync($"Exemplo API ({DateTime.Now}) - Update: {AssemblyBuildDate()}");
            });
        }

        private static DateTime AssemblyBuildDate()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var fileInfo = new FileInfo(entryAssembly.Location);
            return fileInfo.LastWriteTime;
        }
    }
}

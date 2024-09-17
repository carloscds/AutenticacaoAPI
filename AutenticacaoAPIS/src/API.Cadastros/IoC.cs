using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetDevPack.Security.JwtExtensions;
using System.IO;
using System.Reflection;
using System;
using System.Threading.Tasks;
using Middlewares;

namespace API.Cadastros
{
    public static class IoC
    {
        public static void AddCustomServices(this IServiceCollection app, IConfiguration configuration)
        {
            app.AddControllers();

        }

        public static void AddSecurity(this IServiceCollection app, IConfiguration configuration)
        {
            app.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.SetJwksOptions(new JwkOptions(configuration["ApplicationSettings:Authority"]));
                //x.TokenValidationParameters.ValidateAudience = true;
            });
            app.AddAuthorization(options =>
            {
                options.AddPolicy("acesso_leitura",
                    policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                    (c.Type == "scope" && (c.Value == "usuario" || c.Value == "API_CADASTRO.leitura")))));
                options.AddPolicy("acesso_escrita",
                    policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                    (c.Type == "scope" && (c.Value == "usuario" || c.Value == "API_CADASTRO.escrita")))));
            });
        }

        public static void AddCustomSwagger(this IServiceCollection app)
        { 
            app.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API.Cadastros", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Input token like: Bearer {token}",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] {}
                    }
                });
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
            app.UseMiddleware<AutenticacaoAPIMiddlewareException>();

            app.MapGet("/", async context =>
            {
                await context.Response.WriteAsync($"Exemplo API Cadastros ({DateTime.Now}) - Update: {AssemblyBuildDate()}");
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

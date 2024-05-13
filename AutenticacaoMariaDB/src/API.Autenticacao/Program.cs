using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using API.Autenticacao;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddCustomSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomEndpoints();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
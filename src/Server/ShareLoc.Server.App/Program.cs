using ShareLoc.Server.App.Endpoints;
using ShareLoc.Server.App.Extensions;
using ShareLoc.Server.App.Middlewares;
using ShareLoc.Server.App.Pages;
using ShareLoc.Server.DAL.Extensions;
using ShareLoc.Shared.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.AddEnvironmentVariables(prefix: "SHARELOC_")
	.Build();

// Add services to the container.
builder.Services.AddCommon();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCommon();
builder.Services.AddDAL();
builder.Services.AddDBContext(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	await app.Services.EnsureTablesCreatedAsync();

	app.UseSwagger();
	app.UseSwaggerUI();

	app.UseMiddleware<RequestLoggingMiddleware>();
	app.UseMiddleware<ResponseLoggingMiddleware>();
}
else
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapPage<GuessingPage>();
app.MapEndpoints<Endpoints>();

app.Run();

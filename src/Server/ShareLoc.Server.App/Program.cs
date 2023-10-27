using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using ShareLoc.Server.App.Endpoints;
using ShareLoc.Server.App.Extensions;
using ShareLoc.Server.DAL.Repositories;
using ShareLoc.Server.DAL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<GuessRepository>();
builder.Services.AddSingleton<PlaceRepository>();
builder.Services.AddSingleton<GuessService>();
builder.Services.AddSingleton<PlaceService>();

// Create DynamoDB context
AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
clientConfig.ServiceURL = "http://localhost:8000";
clientConfig.AuthenticationRegion = "eu-central-1";
AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);
builder.Services.AddSingleton(new DynamoDBContext(client));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapEndpoints<Endpoints>();

app.Run();

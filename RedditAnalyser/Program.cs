using RedditAnalyzer.Service;
using RedditAnalyzer.Api.Extensions;
using RedditAnalyzer.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddServicesDependency();
builder.Services.AddDatabase(configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.AddControllers();

app.MapGet("/", () => "Hello world!");

app.Run();

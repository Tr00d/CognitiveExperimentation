using CognitiveServices.Api;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Vonage.Extensions;
using Vonage.Request;

var vonageCredentials =
    Credentials.FromAppIdAndPrivateKeyPath(GetOptions().VonageApplicationId, GetOptions().VonagePrivateKeyPath);
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CognitiveOptions>(_ => GetOptions());
builder.Services.AddScoped<ApiKeyServiceClientCredentials>(service =>
    new ApiKeyServiceClientCredentials(service.GetRequiredService<CognitiveOptions>().ApiKey));
builder.Services.AddVonageClientScoped(vonageCredentials);
builder.Services.AddScoped<ContentModeratorClient>(service =>
    new ContentModeratorClient(service.GetRequiredService<ApiKeyServiceClientCredentials>())
    {
        Endpoint = service.GetRequiredService<CognitiveOptions>().ApiEndpoint,
    });
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();

CognitiveOptions GetOptions() =>
    new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false, true)
        .Build()
        .GetSection(nameof(CognitiveOptions))
        .Get<CognitiveOptions>() ??
    throw new InvalidOperationException("Cannot load CognitiveOptions from configuration file.");
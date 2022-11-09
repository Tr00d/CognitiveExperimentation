using System.Text;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using Newtonsoft.Json;

namespace CognitiveServices.Application;

public static class HarassmentEvaluator
{
    private static IEnumerable<string> LoadInputs(string filePath) =>
        JsonConvert.DeserializeObject<TextInput>(File.ReadAllText(filePath)).Values;

    public static async Task<IEnumerable<string>> EvaluateAsync(CognitiveOptions options)
    {
        var inputs = LoadInputs(options.InputFilePath);
        var tasks = inputs.Select(value => GetScreeningResultAsync(value, options));
        return await Task.WhenAll(tasks);
    }

    private static async Task<string> GetScreeningResultAsync(string text, CognitiveOptions options)
    {
        using var client = CreateClient(options);
        try
        {
            Console.WriteLine($"Evaluating: '{text}'");
            var screeningResult = await ScreenTextAsync(text, client);
            var serializedResult = JsonConvert.SerializeObject(screeningResult, Formatting.Indented);
            Console.WriteLine($"Evaluation result for '{text}': {serializedResult}");
            return serializedResult;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Evaluation error for {text}: {exception.Message}");
            return exception.Message;
        }
    }

    private static async Task<Screen> ScreenTextAsync(string text, IContentModeratorClient client) =>
        await client.TextModeration.ScreenTextAsync("text/plain", new MemoryStream(Encoding.UTF8.GetBytes(text)), "eng",
            true, true, null, true);

    private static IContentModeratorClient CreateClient(CognitiveOptions options) =>
        new ContentModeratorClient(new ApiKeyServiceClientCredentials(options.ApiKey))
        {
            Endpoint = options.ApiEndpoint,
        };
}
using System.Text;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using Newtonsoft.Json;
using static LanguageExt.Prelude;

namespace CognitiveServices.Application;

public static class HarassmentEvaluator
{
    private static IEnumerable<ScreeningInput> LoadInputs(CognitiveOptions options) =>
        JsonConvert.DeserializeObject<TextInput>(File.ReadAllText(options.InputFilePath))
            .Values
            .Select(value => new ScreeningInput(options.ApiKey, options.ApiEndpoint, value));

    public static async Task<IEnumerable<string>> EvaluateAsync(CognitiveOptions options) =>
        await Task.WhenAll(LoadInputs(options).Select(GetScreeningResultAsync));

    private static async Task<string> GetScreeningResultAsync(ScreeningInput screening) =>
        await TryAsync(screening)
            .Do(LogEvaluateBeginning)
            .MapAsync(ScreenTextAsync)
            .Map(SerializeScreening)
            .Do(LogEvaluateSuccess)
            .IfFail(ProcessFailure);

    private static string ProcessFailure(Exception exception)
    {
        LogEvaluateFailure(exception);
        return exception.Message;
    }

    private static string SerializeScreening(Screen result) => JsonConvert.SerializeObject(result, Formatting.Indented);

    private static void LogEvaluateFailure(Exception exception) =>
        Console.WriteLine($"Evaluation error: {exception.Message}");

    private static void LogEvaluateSuccess(string serialized) =>
        Console.WriteLine($"Evaluation result: {serialized}");

    private static void LogEvaluateBeginning(ScreeningInput screening) =>
        Console.WriteLine($"Evaluating: '{screening.Text}'");

    private static async Task<Screen> ScreenTextAsync(ScreeningInput screening) =>
        await CreateClient(screening).TextModeration.ScreenTextAsync("text/plain",
            new MemoryStream(Encoding.UTF8.GetBytes(screening.Text)), "eng",
            true, true, null, true);

    private static IContentModeratorClient CreateClient(ScreeningInput screening) =>
        new ContentModeratorClient(new ApiKeyServiceClientCredentials(screening.ApiKey))
        {
            Endpoint = screening.ApiEndpoint,
        };

    private record ScreeningInput(string ApiKey, string ApiEndpoint, string Text);
}
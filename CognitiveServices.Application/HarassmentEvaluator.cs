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

    private static string SerializeScreening(Screen result) =>
        result.Classification.Category1.Score > 0.3d
        || result.Classification.Category2.Score > 0.3d
        || result.Classification.Category3.Score > 0.3d
            ? JsonConvert.SerializeObject(result, Formatting.Indented)
            : $"'{result.OriginalText}' got scores: {result.Classification.Category1.Score}, {result.Classification.Category2.Score}, {result.Classification.Category3.Score}";

    private static void LogEvaluateFailure(Exception exception) =>
        Console.WriteLine($"Evaluation error: {exception.Message}");

    private static void LogEvaluateSuccess(string serialized) =>
        Console.WriteLine($"Evaluation result: {serialized}");

    private static void LogEvaluateBeginning(ScreeningInput screening) =>
        Console.WriteLine($"Evaluating: '{screening.Text}'");

    private static Screen FilterScreening(Screen screen)
    {
        if (screen.Classification.Category1.Score > 0.3d
            || screen.Classification.Category2.Score > 0.3d
            || screen.Classification.Category3.Score > 0.3d)
        {
            return screen;
        }

        throw new Exception(
            $"'{screen.OriginalText}' got scores: {screen.Classification.Category1}, {screen.Classification.Category2}, {screen.Classification.Category3}");
    }

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
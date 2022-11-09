using System.Text;
using CognitiveServices.Application;
using LanguageExt;
using Microsoft.Extensions.Configuration;

GetOptions()
    .Do(_ => Console.WriteLine("Starting process..."))
    .Map(Evaluate)
    .Map(ConcatenateResults)
    .Do(WriteOutput)
    .Do(_ => Console.WriteLine("Ending process..."));

string ConcatenateResults(IEnumerable<string> results)
{
    var builder = new StringBuilder();
    results.ToList().ForEach(value => builder.AppendLine(value));
    return builder.ToString();
}

void WriteOutput(string content) => GetOptions()
    .Map(options => options.OutputFilePath)
    .Do(path => File.WriteAllText(path, content));

IEnumerable<string> Evaluate(CognitiveOptions options) => HarassmentEvaluator.EvaluateAsync(options).Result;

IConfiguration GetConfiguration() => new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .Build();

Option<CognitiveOptions> GetOptions() => GetConfiguration()
    .GetSection(nameof(CognitiveOptions))
    .Get<CognitiveOptions>() ?? Option<CognitiveOptions>.None;
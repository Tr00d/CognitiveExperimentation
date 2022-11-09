namespace CognitiveServices.Application;

public class CognitiveOptions
{
    public CognitiveOptions(string apiKey, string apiEndpoint, string inputFilePath, string outputFilePath)
    {
        this.ApiKey = apiKey;
        this.ApiEndpoint = apiEndpoint;
        this.InputFilePath = inputFilePath;
        this.OutputFilePath = outputFilePath;
    }

    public string ApiKey { get; set; }
    public string ApiEndpoint { get; set; }
    public string InputFilePath { get; set; }
    public string OutputFilePath { get; set; }
}
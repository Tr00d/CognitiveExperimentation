# CognitiveExperimentation

Experimentation for harassment detection with Azure Cognitive Services

## How to setup the project

Set your Azure key and endpoint in `appsettings.json`.

```json
{
  "CognitiveOptions": {
    "ApiKey": "...",
    "ApiEndpoint": "..."
  }
}
```

Add sentences you want to evaluate in `input.json`.

```json
{
  "Values": [
    "Sentence 1",
    "Sentence 2",
    "Sentence 3"
  ]
}
```

## Run the project

Simply run the project and observe the console output.
You should see each sentence being evaluated by Cognitive Services.

When the process finished, the file `output.json` containing detailed results is created in `\bin\Debug\net6.0`.

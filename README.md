# CognitiveExperimentation

Experimentation for harassment detection with Azure Cognitive Services.

There are two ways to try it out:

- A console app
- An API with a Vonage API integration (Messages)

## Console Application

This application reads data from `input.json`, and outputs results into `output.json`.

### How to setup the project

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

### Run the project

Simply run the project and observe the console output.
You should see each sentence being evaluated by Cognitive Services.

When the process finished, the file `output.json` containing detailed results is created in `\bin\Debug\net6.0`.

## API - Vonage integration with Messages

This is a local API working with Vonage Messages API.

Here's the workflow:

- The client sends an SMS to your Vonage number
- Our API receives the message content via a callback
- Our API sends the text to Azure Cognitive Services
- Our API formats the data
- Our API sends an SMS back to the client, with the classification details

### How to setup the project

Fill your credentials in `appsettings.json`.

```json
{
  "CognitiveOptions": {
    "ApiKey": "Your Azure Key",
    "ApiEndpoint": "Your Azure Endpoint",
    "VonageApplicationId": "Your Vonage ApplicationId"
  }
}
```

Fill your Vonage private key in `PrivateKey.txt`.

```
-----BEGIN PRIVATE KEY-----
...
-----END PRIVATE KEY-----
```

By then, you should be able to run the API locally with SwaggerUI.

### How to expose your API with ngrok

By default, the API runs over `http://localhost:5000`

Run the following to expose this port through tunneling on ngrok:

```
ngrok http 5000
```

Your local APi now has a public url, like `https://4a8698326dae.ngrok.app`.

### How to setup the Callback Url

On Vonage Dashboard, navigate to your application - make sure to have Messages enabled.

You need to set Messages inbound url with the following value: `{ngrok-url}/harassment` with `ngrok-url` being your
public url.

Finally, you need to link a Vonage phone number to this Application.

### Run the project

Simply run the project, and send a text message to your Vonage phone number.

You will receive a text message with the following structure:

```text
Sexually explicit: 1%
Sexually suggestive: 25%
Offensive: 18%
```

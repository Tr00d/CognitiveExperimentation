using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Vonage.Messages;
using Vonage.Messages.Sms;

namespace CognitiveServices.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HarrassmentController : ControllerBase
{
    private readonly IMessagesClient client;

    public HarrassmentController(IMessagesClient client) => this.client = client;

    [HttpPost]
    public async Task<IActionResult> GetCallBack(SmsInput input)
    {
        var contentClient =
            new ContentModeratorClient(new ApiKeyServiceClientCredentials("3d4c0a3afddb47d49b6046f8ef30df04"))
            {
                Endpoint = "https://howtotalk.cognitiveservices.azure.com/",
            };
        var result = await contentClient.TextModeration.ScreenTextAsync("text/plain",
            new MemoryStream(Encoding.UTF8.GetBytes(input.Text)), "eng",
            true, true, null, true);
        await this.client.SendAsync(new SmsRequest
        {
            From = input.To,
            To = input.From,
            Text = result.Classification.Category3.Score.ToString(),
        });
        return this.Ok();
    }

    public record SmsInput(string To, string From, string Text);
}
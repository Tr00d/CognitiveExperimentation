using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using Vonage.Messages;
using Vonage.Messages.Sms;

namespace CognitiveServices.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HarassmentController : ControllerBase
{
    private readonly ContentModeratorClient moderationClient;
    private readonly IMessagesClient messagesClient;

    public HarassmentController(IMessagesClient messagesClient, ContentModeratorClient moderationClient)
    {
        this.messagesClient = messagesClient;
        this.moderationClient = moderationClient;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveSmsCallBack(SmsInput input)
    {
        var result = await this.ScreenUserInputAsync(input.Text);
        await this.messagesClient.SendAsync(new SmsRequest
        {
            From = input.To,
            To = input.From,
            Text = BuildOutputMessage(result.Classification),
        });
        return this.Ok();
    }

    private static string BuildOutputMessage(Classification classification)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Sexually explicit: {FormatPercentageOutput(classification.Category1.Score)}");
        stringBuilder.AppendLine($"Sexually suggestive: {FormatPercentageOutput(classification.Category2.Score)}");
        stringBuilder.AppendLine($"Offensive: {FormatPercentageOutput(classification.Category3.Score)}");
        return stringBuilder.ToString();
    }

    private static string FormatPercentageOutput(double? value) => value.HasValue
        ? (value.Value * 100).ToString("0'%'")
        : string.Empty;

    private Task<Screen> ScreenUserInputAsync(string input) =>
        this.moderationClient.TextModeration.ScreenTextAsync("text/plain",
            new MemoryStream(Encoding.UTF8.GetBytes(input)), "eng",
            true, true, null, true);

    public record SmsInput(string To, string From, string Text);
}
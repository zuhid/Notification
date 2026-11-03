using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Zuhid.Notification.Tests.Integration;

// End-to-end tests through the real HTTP pipeline (routing, model binding). Each endpoint only
// queues a message for the background service to process, so the HTTP response body is always
// empty — assertions instead poll MailHog (localhost:8025), which the dev SMTP setting
// (localhost:1025) delivers to.
public class AuthControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly HttpClient MailHogClient = new() { BaseAddress = new Uri("http://localhost:8025") };
    private readonly HttpClient _client = factory.CreateClient();

    // Polls MailHog for a message sent to `to` with the given subject and returns its decoded body.
    private static async Task<string> WaitForMessage(string to, string subject)
    {
        for (var i = 0; i < 20; i++)
        {
            await Task.Delay(500);
            var response = await MailHogClient.GetAsync($"/api/v2/search?kind=to&query={Uri.EscapeDataString(to)}");
            if (!response.IsSuccessStatusCode)
            {
                continue;
            }

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            foreach (var item in doc.RootElement.GetProperty("items").EnumerateArray())
            {
                var content = item.GetProperty("Content");
                var headers = content.GetProperty("Headers");
                if (headers.GetProperty("Subject")[0].GetString() != subject)
                {
                    continue;
                }

                var body = content.GetProperty("Body").GetString() ?? string.Empty;
                var encoding = headers.TryGetProperty("Content-Transfer-Encoding", out var cte) ? cte[0].GetString() : null;
                return encoding switch
                {
                    "base64" => Encoding.UTF8.GetString(Convert.FromBase64String(body)),
                    "quoted-printable" => DecodeQuotedPrintable(body),
                    _ => body,
                };
            }
        }

        throw new TimeoutException($"No MailHog message found for to:{to} subject:{subject}");
    }

    private static string DecodeQuotedPrintable(string input)
    {
        input = input.Replace("=\r\n", "").Replace("=\n", "");
        var bytes = new List<byte>();
        for (var i = 0; i < input.Length; i++)
        {
            if (input[i] == '=' && i + 2 < input.Length && Uri.IsHexDigit(input[i + 1]) && Uri.IsHexDigit(input[i + 2]))
            {
                bytes.Add(Convert.ToByte(input.Substring(i + 1, 2), 16));
                i += 2;
            }
            else
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(input[i].ToString()));
            }
        }
        return Encoding.UTF8.GetString([.. bytes]);
    }

    [Fact]
    public async Task VerifyEmail_QueuesMessage_SendsVerificationEmail()
    {
        // Arrange
        var email = $"verify_{Guid.NewGuid():N}@example.com";
        var token = Guid.NewGuid().ToString("N");
        var request = new { Email = email, AppUrl = "https://example.com", Token = token };
        // Act
        var response = await _client.PostAsJsonAsync("/Auth/VerifyEmail", request);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await WaitForMessage(email, "Verify Email");
        Assert.Contains(token, html);
        Assert.Contains($"https://example.com/account/verify-email/{Uri.EscapeDataString(email)}/{token}", html);
    }

    [Fact]
    public async Task VerifyPhone_QueuesMessage_SendsCodeToPhoneTestInbox()
    {
        // Arrange
        var phone = Random.Shared.Next(1000000000, int.MaxValue).ToString();
        var token = Guid.NewGuid().ToString("N")[..6];
        var request = new { Phone = phone, Token = token };
        // Act
        var response = await _client.PostAsJsonAsync("/Auth/VerifyPhone", request);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await WaitForMessage("phone@test.com", phone);
        Assert.Contains($"Your verification code is: {token}", body);
    }

    [Fact]
    public async Task SendTwoFactorEmail_QueuesMessage_SendsCodeEmail()
    {
        // Arrange
        var email = $"twofactor_{Guid.NewGuid():N}@example.com";
        var token = Random.Shared.Next(100000, 999999).ToString();
        var request = new { Email = email, Token = token };
        // Act
        var response = await _client.PostAsJsonAsync("/Auth/SendTwoFactorEmail", request);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await WaitForMessage(email, "Your verification code");
        Assert.Contains(token, html);
    }

    [Fact]
    public async Task SendTwoFactorSms_QueuesMessage_SendsCodeToPhoneTestInbox()
    {
        // Arrange
        var phone = Random.Shared.Next(1000000000, int.MaxValue).ToString();
        var token = Random.Shared.Next(100000, 999999).ToString();
        var request = new { Phone = phone, Token = token };
        // Act
        var response = await _client.PostAsJsonAsync("/Auth/SendTwoFactorSms", request);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await WaitForMessage("phone@test.com", phone);
        Assert.Contains($"Your two-factor authentication code is: {token}", body);
    }

    [Theory]
    [InlineData("/Auth/VerifyEmail")]
    [InlineData("/Auth/VerifyPhone")]
    [InlineData("/Auth/SendTwoFactorEmail")]
    [InlineData("/Auth/SendTwoFactorSms")]
    public async Task MissingRequiredFields_Returns400(string endpoint)
    {
        // Act
        var response = await _client.PostAsJsonAsync(endpoint, new { });
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

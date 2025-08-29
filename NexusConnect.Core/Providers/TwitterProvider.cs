using NexusConnect.Core.Exceptions;
using NexusConnect.Core.Providers.Twitter;
using NexusConnect.Core.Providers.Twitter.Models;
using System.Text.Json;
using OAuth;
using System.Text;

namespace NexusConnect.Core.Providers;

public class TwitterProvider : IProvider, ITwitterActions
{
    // Kendi HttpClient'ýmýzý tekrar kullanýma alýyoruz
    private static readonly HttpClient httpClient = new HttpClient();

    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _accessToken;
    private readonly string _accessTokenSecret;

    public string Name => "Twitter";

    public TwitterProvider(string apiKey, string apiSecret, string accessToken, string accessTokenSecret)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _apiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
        _accessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        _accessTokenSecret = accessTokenSecret ?? throw new ArgumentNullException(nameof(accessTokenSecret));
    }

    public async Task<Tweet> PostTweet(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Tweet text cannot be null or whitespace.", nameof(text));

        var requestUri = "https://api.twitter.com/2/tweets";

        // 1. OAuth imza makinesini yapýlandýr
        var oAuth = new OAuthRequest
        {
            ConsumerKey = _apiKey,
            ConsumerSecret = _apiSecret,
            Token = _accessToken,
            TokenSecret = _accessTokenSecret,
            Method = "POST",
            RequestUrl = requestUri,
            Version = "1.0a",
            SignatureMethod = OAuthSignatureMethod.HmacSha1
        };

        // 2. Ýmzalý Authorization baþlýðýný oluþtur
        var authHeader = oAuth.GetAuthorizationHeader();

        // 3. Kendi HttpClient'ýmýzý kullanarak isteði hazýrla
        var payload = new { text };
        var jsonPayload = JsonSerializer.Serialize(payload);
        var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        // Oluþturulan imzalý baþlýðý isteðe ekle
        request.Headers.TryAddWithoutValidation("Authorization", authHeader);
        request.Content = httpContent;

        // 4. Ýsteði gönder
        var response = await httpClient.SendAsync(request);

        // 5. Cevabý ve hatalarý daha önce yaptýðýmýz gibi yönet
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new NexusApiException($"An error occurred while calling the Twitter API. Status: {response.StatusCode}, Response: {errorContent}", new HttpRequestException());
        }

        var contentStream = await response.Content.ReadAsStreamAsync();
        using var jsonDocument = await JsonDocument.ParseAsync(contentStream);
        var dataElement = jsonDocument.RootElement.GetProperty("data");
        var createdTweet = JsonSerializer.Deserialize<Tweet>(dataElement.GetRawText());

        return createdTweet ?? throw new NexusApiException("Twitter created a tweet but the response body was empty or invalid.");
    }

    public void Authenticate(string token)
    {
        throw new NotSupportedException("TwitterProvider is authenticated via its constructor with 4 keys. Use the .As<ITwitterActions>() method directly.");
    }
}
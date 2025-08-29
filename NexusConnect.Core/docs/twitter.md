# Twitter Provider Documentation

The `TwitterProvider` allows you to interact with the Twitter API v2 using OAuth 1.0a User Context authentication.

## Configuration

The `TwitterProvider` requires four keys for authentication. You must register the provider at startup with these keys in your `Program.cs` or equivalent startup file.

```csharp
NexusConnector.Configure(config =>
{
    config.RegisterProvider<TwitterProvider>(() => new TwitterProvider(
        "YOUR_API_KEY",
        "YOUR_API_KEY_SECRET",
        "YOUR_ACCESS_TOKEN",
        "YOUR_ACCESS_TOKEN_SECRET"
    ));
});
```

## Actions (`ITwitterActions`)

The available actions for the Twitter provider are defined in the `ITwitterActions` interface.

### Post a Tweet

You can post a new tweet to the timeline of the authenticated user.

**Signature:**
```csharp
Task<Tweet> PostTweet(string text);
```

**Example:**
```csharp
string tweetText = $"Hello Twitter from NexusConnect! - {DateTime.UtcNow.Ticks}";

var postedTweet = await Connect.To<TwitterProvider>()
                               .As<ITwitterActions>()
                               .PostTweet(tweetText);

Console.WriteLine($"Tweet posted successfully! ID: {postedTweet.Id}");
```
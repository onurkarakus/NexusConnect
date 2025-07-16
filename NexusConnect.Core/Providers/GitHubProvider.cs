using NexusConnect.Core.Providers.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusConnect.Core.Providers;

public class GitHubProvider : IProvider, IGitHubActions
{
    public string Name => "GitHub";

    private string? _token;

    public void Authenticate(string token)
    {
        _token = token;

        Console.WriteLine($"---> GitHub Provider: Token ile kimlik doğrulandı. Token başlangıcı: '{_token.Substring(0, 4)}...'");
    }

    public void Post(string message)
    {
        if (string.IsNullOrEmpty(_token))
        {
            throw new InvalidOperationException("Post işlemi yapmadan önce kimlik doğrulaması gereklidir.");
        }

        // Gerçek bir uygulamada bu metot GitHub API'sine bir HTTP isteği yapar.
        Console.WriteLine($"---> GitHub Provider: Issue/Comment gönderiliyor -> '{message}'");
    }

    public void CreateIssue(string title, string body)
    {
        if (string.IsNullOrEmpty(_token))
        {
            throw new InvalidOperationException("Authentication required.");
        }

        Console.WriteLine($"---> GitHub Provider: Issue oluşturuluyor. Title: '{title}', Body: '{body}'");
    }
}

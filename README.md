# NexusConnect: The Fluent API Orchestrator

[![NuGet version](https://img.shields.io/nuget/v/NexusConnect.svg)](https://www.nuget.org/packages/NexusConnect/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

NexusConnect is a .NET orchestration library that unifies multiple service APIs (like GitHub, Twitter, etc.) under a single, fluent, and intuitive syntax. It is designed to maximize the developer experience (DX).

> **"Stop juggling SDKs. Start writing logic."**

## What Problem Does It Solve?

Developing a modern application often requires integration with dozens of third-party services. Each service (GitHub, Dropbox, Slack, Twitter...) has its own SDK, its own authentication logic, and its own design philosophy. This leads to:
-   A steep learning curve for each new integration.
-   Inconsistency across the codebase.
-   Dozens of lines of boilerplate code for even simple operations.

NexusConnect solves these problems by abstractacting all these different APIs behind a single, consistent, and fluent interface.

## Core Usage (Quick Start)

With NexusConnect, complex API interactions become self-documenting, readable, chained method calls:

```csharp
// Creating a GitHub issue and getting its details has never been easier.
var newIssue = await Connect.To<GitHubProvider>()
                            .WithToken("YOUR_PERSONAL_ACCESS_TOKEN")
                            .As<IGitHubActions>()
                            .CreateIssue("New Idea: An Awesome Feature", "We should definitely implement this feature.");

Console.WriteLine($"Successfully created issue #{newIssue.Number}! URL: {newIssue.Url}");
```

## Getting Started

### 1. Installation

Add the NexusConnect library to your project using the NuGet Package Manager:

```bash
dotnet add package NexusConnect
```

### 2. Configuration

At your application's entry point (e.g., `Program.cs`), register the providers you intend to use. This teaches the library how each provider should be constructed.

```csharp
using NexusConnect.Core;
using NexusConnect.Core.Providers;

public static void Main(string[] args)
{
    // Register your providers
    NexusConnector.Configure(config =>
    {
        config.RegisterProvider<GitHubProvider>(() => 
            new GitHubProvider("YOUR_USERNAME", "YOUR_REPO_NAME")
        );
    });

    // (Optional) Set a default token for the entire application lifecycle
    NexusConnector.SetDefaultToken("YOUR_SECRET_TOKEN_HERE");

    // ... rest of your application
}
```

## Features & Examples

### GitHub Provider (`GitHubProvider`)

The `GitHubProvider` allows you to interact with the GitHub API.

#### Creating an Issue

```csharp
var issue = await Connect.To<GitHubProvider>()
                         .WithToken(githubToken)
                         .As<IGitHubActions>()
                         .CreateIssue("Critical Bug", "The application crashes on the main page.");
```

#### Listing Issues

By default, `GetIssues` fetches open issues.

```csharp
var openIssues = await Connect.To<GitHubProvider>()
                              .WithToken(githubToken)
                              .As<IGitHubActions>()
                              .GetIssues();
```

You can use the `IssueState` enum to list closed or all issues:

```csharp
var closedIssues = await Connect.To<GitHubProvider>()
                                .WithToken(githubToken)
                                .As<IGitHubActions>()
                                .GetIssues(IssueState.Closed);
```

#### Getting a Single Issue

```csharp
int issueNumber = 42;
var issue = await Connect.To<GitHubProvider>()
                         .WithToken(githubToken)
                         .As<IGitHubActions>()
                         .GetIssueByNumber(issueNumber);
```

#### Updating an Issue

You only need to specify the fields you want to change.

```csharp
// Change the title of issue #42 and close it.
var updatedIssue = await Connect.To<GitHubProvider>()
                                .WithToken(githubToken)
                                .As<IGitHubActions>()
                                .UpdateIssue(42, title: "UPDATED - Critical Bug Fixed", state: IssueState.Closed);
```

#### Using the Default Token

If you have set a default token during configuration, you can use the `.WithDefaultToken()` method to avoid passing the token on every call.

```csharp
// Assumes NexusConnector.SetDefaultToken() has been called at startup.
var issues = await Connect.To<GitHubProvider>()
                          .WithDefaultToken()
                          .As<IGitHubActions>()
                          .GetIssues();
```

## Contributing

This project welcomes community contributions. Bug reports, feature requests, and pull requests are highly appreciated.

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
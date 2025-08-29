### GitHub Provider (`GitHubProvider`)

The `GitHubProvider` allows you to interact with the GitHub API.

#### Using the Default Token

If you have set a default token during configuration, you can use the `.WithDefaultToken()` method to avoid passing the token on every call.

```csharp
// Assumes NexusConnector.SetDefaultToken() has been called at startup.
var issues = await Connect.To<GitHubProvider>()
                          .WithDefaultToken()
                          .As<IGitHubActions>()
                          .GetIssues();
```

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

#### Creating a Comment on an Issue

You can easily add a new comment to any existing issue using its number.

```csharp
// Adds a new comment to issue #42
var createdComment = await Connect.To<GitHubProvider>()
                                  .WithToken(githubToken)
                                  .As<IGitHubActions>()
                                  .CreateComment(42, "I've investigated this bug and I have a potential fix.");

Console.WriteLine($"Successfully posted a new comment! URL: {createdComment.Url}");
```
#### Managing Labels

NexusConnect provides a full set of methods to manage labels on your issues.

**List All Repository Labels**
```csharp
// Get all available labels for the configured repository.
var repoLabels = await Connect.To<GitHubProvider>()
                              .WithToken(githubToken)
                              .As<IGitHubActions>()
                              .GetLabelsForRepository();
```

**Add Labels to an Issue**
```csharp
// Add the "bug" and "documentation" labels to issue #42.
var updatedLabels = await Connect.To<GitHubProvider>()
                                 .WithToken(githubToken)
                                 .As<IGitHubActions>()
                                 .AddLabelsToIssue(42, "bug", "documentation");
```
        
**Remove a Label from an Issue**
```csharp
// Remove the "bug" label from issue #42.
var finalLabels = await Connect.To<GitHubProvider>()
                               .WithToken(githubToken)
                               .As<IGitHubActions>()
                               .RemoveLabelFromIssue(42, "bug");
```
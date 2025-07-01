namespace NexusConnect.Core.Fluent;

public interface IAuthenticationStage
{
    IActionStage WithToken(string token);
}

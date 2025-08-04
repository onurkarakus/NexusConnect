using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NexusConnect.Core.Providers.GitHub.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IssueState
{
    Open,
    Closed,
    All
}

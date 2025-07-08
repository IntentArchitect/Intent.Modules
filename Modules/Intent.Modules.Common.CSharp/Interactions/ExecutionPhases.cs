#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Interactions;

public class ExecutionPhases
{
    public const string Validation = "Validation";
    public const string Retrieval = "Retrieval";
    public const string BusinessLogic = "BusinessLogic";
    public const string Persistence = "Persistence";
    public const string IntegrationEvents = "IntegrationEvents";
    public const string Response = "Response";

    public static readonly List<string> ExecutionPhaseOrder =
    [
        Validation,
        Retrieval,
        BusinessLogic,
        Persistence,
        IntegrationEvents,
        Response
    ];
}
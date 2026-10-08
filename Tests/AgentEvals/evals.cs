#!/usr/bin/env dotnet
#:property PublishAot=false
#:include Conductor.cs
#:include Workspace.cs
#:include Harnesses.cs
#:include HarnessesMore.cs
#:include McpStub.cs
#:include Transcript.cs
#:include Evidence.cs
#:include Scenarios.cs
#:include Wiring.cs
#:include Judge.cs
#:include Report.cs
#:include Processes.cs

// The agent eval suite: checks that the hooks, instructions and skills the Intent.ModuleBuilder.AI.*
// modules generate reach each AI harness and work, against the real harness CLIs. See README.md.
// "mcp-stub" is the runner serving as the Intent MCP stub a harness launches during a run.
return args is ["mcp-stub", .. var rest]
    ? await AgentEvals.McpStub.RunAsync(rest)
    : await AgentEvals.Conductor.RunAsync(args);

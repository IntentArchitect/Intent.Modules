using System.Text.Json;

namespace Intent.Agent.Gate;

/// <summary>
/// Which AI harness is asking. Claude Code and Codex share a JSON dialect for a
/// PreToolUse-style decision (confirmed from vendor docs), so they are one case here.
/// </summary>
public enum Harness
{
    ClaudeCodeOrCodex,
    Cursor,
    Kiro
}

/// <summary>
/// Serializes a gate decision into the harness-specific shape its hook runner expects. Every
/// harness this gate targets blocks via exit code 2 - that is the one universal signal - and
/// this class only decides what, if anything, additionally goes to stdout so a human or the
/// model can read the reason. If real hook wiring shows a different shape, only this file
/// needs to change.
///
/// Confirmed from vendor documentation:
///  - Claude Code / Codex: "hookSpecificOutput.permissionDecision" / "permissionDecisionReason"
///    for a PreToolUse-style decision; Claude Code's Stop hook reads "systemMessage" for a
///    non-blocking note.
///  - Cursor: exit 2 blocks regardless of the JSON body; the body is
///    "permission": "allow"|"deny", "agent_message". Cursor fails OPEN on any other non-zero
///    exit code, so the caller must always exit exactly 0 or 2, never the runtime's default
///    unhandled-exception code.
///  - Kiro: blocks purely via exit code 2 plus the reason on stderr - no JSON body is read for
///    a PreToolUse deny. On a successful (exit 0) hook, Kiro incorporates stdout directly into
///    the agent's context, so a close-out warning is written there as plain text, not JSON.
/// </summary>
public static class HarnessProtocol
{
    public static Harness Parse(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "cursor" => Harness.Cursor,
        "kiro" => Harness.Kiro,
        _ => Harness.ClaudeCodeOrCodex,
    };

    /// <summary>
    /// PreToolUse-equivalent decision, used by guard-write and guard-version. Returns what to
    /// write to stdout, or null when nothing should be written there (Kiro - stderr alone
    /// carries the reason on a deny).
    /// </summary>
    public static string? FormatToolPermission(Harness harness, bool allow, string reason) => harness switch
    {
        Harness.Kiro => null,
        Harness.Cursor => Serialize(new { permission = allow ? "allow" : "deny", agent_message = reason }),
        _ => Serialize(new
        {
            hookSpecificOutput = new
            {
                hookEventName = "PreToolUse",
                permissionDecision = allow ? "allow" : "deny",
                permissionDecisionReason = reason,
            },
        }),
    };

    /// <summary>
    /// Stop-equivalent, NON-BLOCKING note used by close-out. Close-out never denies (see the
    /// module's deny/warn split), so this only decides how the reason is surfaced when there
    /// is one - callers always exit 0 regardless of what this returns.
    /// </summary>
    public static string FormatWarning(Harness harness, string reason) => harness switch
    {
        Harness.Kiro => reason,
        Harness.Cursor => Serialize(new { permission = "allow", agent_message = reason }),
        _ => Serialize(new { systemMessage = reason }),
    };

    private static string Serialize(object payload) => JsonSerializer.Serialize(payload);
}
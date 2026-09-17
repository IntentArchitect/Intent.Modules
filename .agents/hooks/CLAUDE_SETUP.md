# Wiring the agent gate into Claude Code

Claude Code reads hooks from `.claude/settings.json` - a file this module does not generate,
because it is shared with permissions, environment variables, and hooks you already have.
Add the block below under your existing `"hooks"` key (or create the file if you don't have
one yet), merging it with whatever is already there rather than replacing the file.

```json
{
  "hooks": {
    "SessionStart": [
      {
        "hooks": [
          { "type": "command", "command": "dotnet run \"$CLAUDE_PROJECT_DIR/.agents/hooks/gate.cs\" -- warm" }
        ]
      }
    ],
    "PreToolUse": [
      {
        "matcher": "Write|Edit",
        "hooks": [
          { "type": "command", "command": "dotnet run \"$CLAUDE_PROJECT_DIR/.agents/hooks/gate.cs\" --no-build -- guard-write --harness claude" }
        ]
      },
      {
        "matcher": ".*run_designer_script.*",
        "hooks": [
          { "type": "command", "command": "dotnet run \"$CLAUDE_PROJECT_DIR/.agents/hooks/gate.cs\" --no-build -- guard-version --harness claude" }
        ]
      }
    ],
    "Stop": [
      {
        "hooks": [
          { "type": "command", "command": "dotnet run \"$CLAUDE_PROJECT_DIR/.agents/hooks/gate.cs\" --no-build -- close-out --harness claude" }
        ]
      }
    ]
  }
}
```

`$CLAUDE_PROJECT_DIR` is Claude Code's own project-root variable - never a bare relative path,
which breaks the moment a session's working directory moves into a subdirectory.

This is a one-time step per repository. Re-running the Software Factory does not touch
`.claude/settings.json` and will not repeat this instruction once you've done it - it is not
tracked as done anywhere, so if you reinstall this module elsewhere, do it again there too.
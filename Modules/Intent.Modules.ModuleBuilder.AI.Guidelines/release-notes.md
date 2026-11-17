### Version 1.0.1

- Improvement: The agent gate lets an agent resolve a merge or rebase conflict in Intent metadata, an `.imodspec` or a `modules.config` with its file-editing tools. It denies only a resolution that writes conflict markers back, and the workflow instructions say how to resolve such a conflict.
- Improvement: The close-out check runs about three times faster in large repositories, so it no longer times out.
- Fixed: The close-out check now attributes a new file whose name has non-ASCII characters to its module.

### Version 1.0.0

- New Feature: Guides AI agents through a four-phase module-building workflow with bundled skills and instructions, and an agent gate for Claude Code, Codex, GitHub Copilot CLI, Cursor, Kiro and OpenCode that blocks hand-edits to Intent Architect metadata and invalid version changes.

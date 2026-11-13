# AGENTS.md — TOLGRIM: ARISE - Shadow Monarch

Permanent guidance for AI development agents working in this repository.

## 1. Project context

- **Mod:** TOLGRIM: ARISE - Shadow Monarch
- **Target game:** RimWorld `1.6.4871+`
- **Technology:** C#, XML, .NET Framework 4.8 (`net48`), C# `latest`, nullable enabled
- **Local repository:** `C:\Users\jelys\3D Objects\antigravity-projects\rimworld-modding\TOLGRIM-ARISE-Shadow-Monarch`
- **Remote repository:** `https://github.com/Tolgrim-arch/ARISE-Shadow-Monarch`
- **Development document:** `C:\Users\jelys\3D Objects\antigravity-projects\rimworld-modding\TOLGRIM-ARISE-Shadow-Monarch\ARISE_SHADOW_MONARCH_DEVELOPMENT.md`

The mod follows a **Vanilla Expanded** philosophy, preserving RimWorld's aesthetics, mechanics, and conventions. Feature scope is defined by each task, not this file.

## 2. Permanent engineering rules

1. **Vanilla first.** Reuse RimWorld Core APIs, `Defs`, stats, components, UI, and assets. Prefer native extension points such as `ITab`, `StatPart`, `ThingComp`, `CompUseEffect`, and recipes. Use Harmony only when necessary.
2. **Simplicity and modularity.** Apply KISS and DRY; use SOLID where it improves clarity. Avoid duplication, one-off exceptions, and speculative abstractions. Inspect and reuse existing architecture before making changes.
3. **Non-destructive changes.** Avoid replacing whole vanilla `Defs` or methods. Use targeted, conditional XML patches where appropriate; do not add redundant `PatchOperationFindMod` checks for guaranteed dependencies.
4. **Native UI.** Preserve vanilla styling, controls, tooltips, and existing collapsible sections when integrating UI. Do not add windows or nested tabs unless requested.
5. **Synchronized visual assets.** Reference Core assets when appropriate instead of duplicating PNGs. Support third-party texture replacements where technically feasible; never assume universal compatibility.
6. **Reliable stats.** Integrate bonuses with native calculations, track their sources, prevent double application, and keep displayed explanations consistent with actual gameplay values.
7. **Persistence and compatibility.** Use native serialization and entity-bound components when appropriate. Protect existing saves; disabling a feature in Mod Settings must not destroy saved data or leave invalid references.
8. **Localization.** All player-facing text must be localized in `1.6/Languages/English/Keyed/`, `1.6/Languages/Spanish/Keyed/`, and `1.6/Languages/SpanishLatin/Keyed/`.
9. **Performance.** Avoid costly per-tick work, repeated global scans, and invasive patches. Cache only when necessary, with correct invalidation.
10. **Verification and scope.** Implement only what was requested. Verify real APIs before using them, build and run available tests, and never equate successful compilation with in-game validation.

## 3. Scope and design reference

See `ARISE_SHADOW_MONARCH_DEVELOPMENT.md` for the mod's design, mechanics, and current development plan. Inspect the codebase before assuming implementation details.

## 4. Build and deployment

Run commands from the repository root.

### Build C#

```powershell
& "C:\Users\jelys\scoop\apps\dotnet-sdk\current\dotnet.exe" build -c Release
```

Before deploying, confirm that paths exist and the build succeeded. Do not delete unrelated files at the destination.

## 5. Workflow

**Inspect → Implement → Verify → Report.**

After each task, briefly summarize changes, affected files, tests performed, and remaining limitations. Always prioritize **native integration, stability, compatibility, and simplicity**.

### Git & GitHub Workflow

While developing the mod "https://github.com/Tolgrim-arch/ARISE-Shadow-Monarch":

- **Use branches and PRs:** Create feature/fix branches, push meaningful changes, open Pull Requests, and merge them rather than committing directly to the default branch.
- **Include co-authors:** Add the following trailers to every commit message, ALWAYS:

Co-Authored-By: Claude <noreply@anthropic.com>
Co-authored-by: Codex <noreply@openai.com>

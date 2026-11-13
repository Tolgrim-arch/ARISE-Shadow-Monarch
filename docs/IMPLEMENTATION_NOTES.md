# Implementation Notes

## Phase 0 — Environment & API Audit

- **Target:** RimWorld 1.6 (`net48`, C# `latest`, nullable enabled).
- **Package ID:** `tolgrim.ariseshadowmonarch`
- **Dependencies:** `Ludeon.RimWorld`, `brrainz.harmony` (Harmony 2.3.3 via `Lib.Harmony` with `ExcludeAssets="runtime"`).
- **Build command:**
  ```powershell
  & "C:\Users\jelys\scoop\apps\dotnet-sdk\current\dotnet.exe" build Source/ShadowMonarch/ShadowMonarch.csproj -c Release
  ```
- **Deployment:** Directory junction at `C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\TOLGRIM-ARISE-Shadow-Monarch` pointing to the repository root.

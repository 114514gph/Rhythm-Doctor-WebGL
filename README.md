# Rhythm Doctor WebGL

A WebGL port of the rhythm puzzle game *Rhythm Doctor*, rebuilt from decompiled Mono assemblies and AssetRipper-exported resources.

---

## Quick Start

1. Download `Rhythm-Doctor-WebGL.zip` from [Releases](https://github.com/114514gph/Rhythm-Doctor-WebGL/releases)
2. Extract the archive
3. Open `index.html` in Chrome or Edge
4. Wait for assets to load (~328 MB data file)

**Controls:** `Enter` (Player 1 defibrillate) · `Z` (Player 2 defibrillate) · `Arrow Keys` (navigation) · `Esc` (pause/back)

---

## Troubleshooting

**"Failed to download file" / "Loading web pages via a file:// URL is not supported"**

Some browsers (Chrome, Edge) block Unity WebGL content when opened directly via `file://`. Use the included local server instead:

- **Windows:** Double-click `启动游戏.bat` (or run `start-server.ps1` with PowerShell)
- The server starts at `http://localhost:8080/` and opens your browser automatically
- Close the terminal window to stop the server

---

## Technical Stack

| Component | Technology |
|-----------|------------|
| Original Engine | Unity 6000.3.21f1 (Mono backend) |
| Rebuild Engine | Unity 2022.3.62f1 (Tuanjie Engine) |
| Target Platform | WebGL (IL2CPP, Emscripten) |
| Build System | GitHub Actions + game-ci/unity-builder@v4 |
| Decompiler | ILSpy 11.1 (.NET 8) |
| Asset Exporter | AssetRipper 2.0.0 (Linux headless) |
| Third-party | DOTween Pro, Rewired, Ink-Libraries, tk2d, Steamworks.NET |

## Build Pipeline

```
Source (Windows Beta, Mono)
  ├─ ILSpy decompilation → 3,687 C# files (53 MB)
  └─ AssetRipper export → 19,504 files (677 MB)
        ↓
  Compilation error resolution (67,599 → 0)
  ├─ Stub generation for missing assemblies
  ├─ Extension methods and implicit conversions
  └─ #if UNITY_WEBGL conditional compilation
        ↓
  GitHub Actions (ubuntu-latest, 7 GB RAM)
  └─ game-ci Docker image → WebGL build (324 MB)
```

## Project Structure

```
├── Assets/
│   ├── Scripts/Assembly-CSharp/       # Decompiled game scripts (2,063 types)
│   ├── Plugins/Assembly-CSharp-firstpass/  # First-pass assemblies (2,060 types)
│   ├── Editor/WebGLBuilder.cs         # Build automation script
│   ├── Resources/credits.txt          # Credits (includes WebGL port credit)
│   └── Fizzd/Scenes/                  # 10 game scenes
├── ProjectSettings/
├── Packages/
└── .github/workflows/build.yml        # CI/CD pipeline
```

## Scenes

`scnLogo` · `scnMenu` · `scnGame` · `scnLevelSelect` · `scnCalibration` · `scnCLS` · `scnIanDesktop` · `scnRhythmWeightlifter` · `scnSleevePaint`

## Credits

| Role | Name |
|------|------|
| Original Game | 7th Beat Games |
| WebGL Port & Rebuild | 梦犹YEG (MengYou YEG) |
| WebGL Port Copyright | © MengYou® Studio™ (port work only) |

## License

All game assets, source code, and original content are © 7th Beat Games. This WebGL port is a fan-made reconstruction and is not affiliated with or endorsed by 7th Beat Games.

The WebGL porting and rebuild work is © MengYou® Studio™, but this covers only the port-specific modifications and build configuration.

Personal use only. Modification, redistribution, and commercial use are prohibited. See [LICENSE](LICENSE) for full terms.

[中文版](README.zh-CN.md)

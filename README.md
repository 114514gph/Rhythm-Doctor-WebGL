# Rhythm Doctor WebGL

A WebGL port of the rhythm puzzle game *Rhythm Doctor*, rebuilt from decompiled Mono assemblies and AssetRipper-exported resources.

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
| Copyright | © MengYou® Studio™ |

## License

Personal use only. Modification, redistribution, and commercial use are prohibited. See [LICENSE](LICENSE) for full terms. Original game © 7th Beat Games.

[中文版](README.zh-CN.md)

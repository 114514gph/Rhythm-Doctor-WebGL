# 节奏医生 WebGL

音乐节奏解谜游戏《节奏医生》的 WebGL 移植版，基于反编译的 Mono 程序集与 AssetRipper 导出的资源重建。

---

## 快速开始

1. 从 [Releases](https://github.com/114514gph/Rhythm-Doctor-WebGL/releases) 下载 `Rhythm-Doctor-WebGL.zip`
2. 解压压缩包
3. 用 Chrome 或 Edge 浏览器打开 `index.html`
4. 等待资源加载（约 328 MB 数据文件）

**操作方式：** `Enter`（玩家一除颤）· `Z`（玩家二除颤）· `方向键`（导航）· `Esc`（暂停/返回）

---

## 常见问题

**"Failed to download file" / "Loading web pages via a file:// URL is not supported"**

部分浏览器（Chrome、Edge）会阻止直接通过 `file://` 打开 Unity WebGL 内容。请使用附带的本地服务器：

- **Windows：** 双击 `启动游戏.bat`（或用 PowerShell 运行 `start-server.ps1`）
- 服务器将在 `http://localhost:8080/` 启动并自动打开浏览器
- 关闭终端窗口即可停止服务器

---

## 技术栈

| 组件 | 技术 |
|------|------|
| 原版引擎 | Unity 6000.3.21f1（Mono 后端） |
| 重建引擎 | Unity 2022.3.62f1（团结引擎） |
| 目标平台 | WebGL（IL2CPP，Emscripten） |
| 构建系统 | GitHub Actions + game-ci/unity-builder@v4 |
| 反编译器 | ILSpy 11.1（.NET 8） |
| 资源导出 | AssetRipper 2.0.0（Linux headless） |
| 第三方库 | DOTween Pro、Rewired、Ink-Libraries、tk2d、Steamworks.NET |

## 构建流程

```
源文件（Windows Beta，Mono）
  ├─ ILSpy 反编译 → 3,687 个 C# 文件（53 MB）
  └─ AssetRipper 导出 → 19,504 个文件（677 MB）
        ↓
  编译错误修复（67,599 → 0）
  ├─ 缺失程序集的桩代码生成
  ├─ 扩展方法与隐式转换
  └─ #if UNITY_WEBGL 条件编译
        ↓
  GitHub Actions（ubuntu-latest，7 GB 内存）
  └─ game-ci Docker 镜像 → WebGL 构建（324 MB）
```

## 项目结构

```
├── Assets/
│   ├── Scripts/Assembly-CSharp/       # 反编译游戏脚本（2,063 个类型）
│   ├── Plugins/Assembly-CSharp-firstpass/  # 首遍程序集（2,060 个类型）
│   ├── Editor/WebGLBuilder.cs         # 构建自动化脚本
│   ├── Resources/credits.txt          # 制作人员（含网页版重置署名）
│   └── Fizzd/Scenes/                  # 10 个游戏场景
├── ProjectSettings/
├── Packages/
└── .github/workflows/build.yml        # CI/CD 流水线
```

## 场景列表

`scnLogo` · `scnMenu` · `scnGame` · `scnLevelSelect` · `scnCalibration` · `scnCLS` · `scnIanDesktop` · `scnRhythmWeightlifter` · `scnSleevePaint`

## 制作人员

| 角色 | 姓名 |
|------|------|
| 原版游戏 | 7th Beat Games |
| 网页版移植与重置 | 梦犹YEG (MengYou YEG) |
| 网页版版权 | © MengYou® Studio™（仅移植部分）|

## 许可证

所有游戏资源、源代码及原创内容均 © 7th Beat Games。本 WebGL 移植版为爱好者自制的重建项目，与 7th Beat Games 无关联，亦未获其官方认可。

网页版移植与重置工作 © MengYou® Studio™，但仅涵盖移植相关的修改与构建配置部分。

仅供个人游玩。禁止修改、再分发及商业使用。完整条款参见 [LICENSE.zh-CN](LICENSE.zh-CN)。

[English Version](README.md)

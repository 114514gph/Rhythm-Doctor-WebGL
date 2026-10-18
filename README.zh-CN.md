# 节奏医生 WebGL 网页版

音乐节奏解谜游戏《节奏医生》的 WebGL 网页浏览器移植版，经过重新适配和构建以支持在线游玩。

## 项目简介

《节奏医生》是一款由 7th Beat Games 开发的单键节奏游戏。本项目是该游戏的 WebGL 构建版本，玩家无需安装原生客户端，即可直接在网页浏览器中运行游戏。构建流程基于 Unity WebGL 导出，并通过 GitHub Actions 实现自动化。

## 制作人员

- 原版游戏：7th Beat Games
- 网页版移植与重置：梦犹YEG
- 项目版权所有：(c) MengYou(R) Studio(TM)

## 构建说明

本仓库使用 GitHub Actions 与 game-ci/unity-builder 动作来生成 WebGL 构建产物。

### 前置条件

- 已为构建运行器激活 Unity Personal 或 Plus 许可证
- 在仓库设置中配置以下 GitHub Secrets：
  - `UNITY_EMAIL`：Unity 账号的电子邮箱
  - `UNITY_PASSWORD`：Unity 账号的密码
  - `UNITY_LICENSE`：已激活的 `.ulf` 许可证文件内容

### 触发构建

每次向 `main` 或 `master` 分支推送代码时会自动触发构建。你也可以在 Actions 标签页中通过 "Build WebGL" 工作流手动触发构建。

### 构建产物

构建成功后，工作流会将 WebGL 输出作为名为 `WebGL-Build` 的 GitHub Artifact 上传。产物包含：

- `index.html`：网页玩家的入口文件
- `Build/`：编译后的 Unity WebGL 数据，包括 `.data`、`.framework.js`、`.wasm` 和 `.loader.js` 文件

## 许可证

本 WebGL 移植版仅供个人游玩。完整条款与条件请参见 [LICENSE.zh-CN](LICENSE.zh-CN)。原版游戏《节奏医生》版权归 7th Beat Games 所有。

[English Version](README.md)

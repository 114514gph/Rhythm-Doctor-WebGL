# Rhythm Doctor WebGL

A WebGL web browser port of the rhythm puzzle game *Rhythm Doctor*, rebuilt and adapted for online play.

## Project Overview

Rhythm Doctor is a one-button rhythm game developed by 7th Beat Games. This project is a WebGL build of the game, allowing players to run the game directly in a web browser without installing a native client. The build pipeline uses Unity's WebGL export and is automated through GitHub Actions.

## Credits

- Original Game: 7th Beat Games
- WebGL Port and Rebuild: MengYou YEG (梦犹YEG)
- Project Copyright: (c) MengYou(R) Studio(TM)

## Build Instructions

This repository uses GitHub Actions with the game-ci/unity-builder action to produce the WebGL build artifact.

### Prerequisites

- A Unity Personal or Plus license activated for the build runner
- The following GitHub Secrets configured in the repository settings:
  - `UNITY_EMAIL`: The email address of the Unity account
  - `UNITY_PASSWORD`: The password of the Unity account
  - `UNITY_LICENSE`: The content of the activated `.ulf` license file

### Triggering a Build

Builds are triggered automatically on every push to the `main` or `master` branch. You can also trigger a manual build from the Actions tab using the "Build WebGL" workflow.

### Build Output

On a successful build, the workflow uploads the WebGL output as a GitHub Artifact named `WebGL-Build`. The artifact contains:

- `index.html`: The entry point for the web player
- `Build/`: The compiled Unity WebGL data, including `.data`, `.framework.js`, `.wasm`, and `.loader.js` files

## License

This WebGL port is for personal use only. See the [LICENSE](LICENSE) file for the full terms and conditions. The original game Rhythm Doctor is the property of 7th Beat Games.

[中文版](README.zh-CN.md)

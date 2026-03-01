# Developer's guide

## Setup

### IDE

- Rider (recommended) with [MonoGame plugin](https://plugins.jetbrains.com/plugin/18415-monogame)
- Visual Studio 2026
- Visual Studio Code

### .NET

Add MonoGame templates:

```bash
dotnet new install "MonoGame.Templates.CSharp"
```

<!--
Available templates:

Template Name                               | Short Name        | Tags
--------------------------------------------|-------------------|--------------------------------------------------------------
MonoGame 2D StartKit                        | mg2dstartkit      | MonoGame/Games/Mobile/Android/iOS/Desktop/Windows/Linux/macOS
MonoGame Android Application                | mgandroid         | MonoGame/Games/Mobile/Android
MonoGame Blank 2D StartKit                  | mgblank2dstartkit | MonoGame/Games/Mobile/Android/iOS/Desktop/Windows/Linux/macOS
MonoGame Content Builder                    | mgcb              | MonoGame/Games/Library
MonoGame Content Pipeline Extension         | mgpipeline        | MonoGame/Games/Extensions
MonoGame Cross-Platform Desktop Application | mgdesktopgl       | MonoGame/Games/Desktop/Windows/Linux/macOS
MonoGame Game Library                       | mglib             | MonoGame/Games/Library
MonoGame iOS Application                    | mgios             | MonoGame/Games/Mobile/iOS
MonoGame Shared Library Project             | mgshared          | MonoGame/Games/Library
MonoGame Windows Desktop Application        | mgwindowsdx       | MonoGame/Games/Desktop/Windows
-->

## Backlog

<!-- Todo 2008-12-30 -->

Add new basic functionalities:

- always resize the model to a unitary size
- font support with at least coordinates and frame rate
- build the bounding sphere around the model

Change behavior:

- handle keyboard input to move the model

Fix problem:

- better display, delimited string length and number format

## Quality checks

```bash
dotnet format
docker run --rm -v "$(pwd)":/data cytopia/yamllint .
docker run --rm -v "$(pwd)":/workdir davidanson/markdownlint-cli2 "**/*.md"
```

## Inspirations

- [MonoGame/MonoGame.Samples](https://github.com/MonoGame/MonoGame.Samples)
- [SimonDarksideJ/XNAGameStudio](https://github.com/SimonDarksideJ/XNAGameStudio) by [DarkGenesis](https://darkgenesis.zenithmoon.com/)
  - Forks: [rbwhitaker/MonoGameSamples](https://github.com/rbwhitaker/MonoGameSamples)

Convert old models with [FBX 2013.3 Converter for Windows 64-bit](https://aps.autodesk.com/developer/overview/fbx-converter-archives), save in ASCII,
edit the file manually to fix the relative path (replace ".." by ".")

<!--
C:\Program Files\Autodesk\FBX\FBX Converter\2013.3\samples
-->

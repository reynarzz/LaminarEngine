# GFS (Game From Scratch)

![Gameplay](Intro.gif)

GFS is a 2D video game and game engine built entirely from scratch with **C#**, heavily inspired by the **Unity3D C# API**.

## Features

- Custom engine written in C#
- 2D Rendering (Sprites/Tilemap)
- 2D Physics (Box2D)
- Sprite batching
- Particle system.
- Dynamic text rendering.
- Audio system.
- Modular architecture for easy expansion.
- Post-processing stack system (via framebuffer passes)
- Custom file system (simplified).
- Build assets in dev mode (Unity-like import system) and release mode.
- Cross-platform: Windows, MacOs

## Roadmap
- [x] Implement Audio system.
- [x] File system.
- [x] Tilemap optimized auto collider.
- [x] Font rendering.
- [x] Windows deploy.
- [x] Implement particle system.
- [ ] Optimize rendering, and expand it to support complex geometries.
- [ ] Make the demo game (Proper architecture).
- [ ] macOS, Android, and IOS platforms deploy.

### Platforms
- Windows
- MacOs
- Android (Experimental)

## Getting Started
#### Note: If you are only interested in playing the game, download the repo, and go to DemoBin/win32

- Windows or macOS: .NET 9.0 SDK or later: https://dotnet.microsoft.com/en-us/download/dotnet/9.0
- cd platforms/desktop
- dotnet build

Optional
--------
- Use an IDE like Visual Studio 2022+, Rider, or VS Code with C# support.
- Make sure your PATH includes the .NET 9.0 SDK.

## License
This project is licensed under the Non-Commercial Copyleft License (NCCL).

### You are allowed to:
- View and learn from the code
- Fork and modify it for non-commercial purposes
- Use parts of it in your own non-commercial projects, with attribution

### You are NOT allowed to:
- Use this code in commercial games, engines, or products
- Sell or monetize any part of it
- Use it in revenue-generating projects

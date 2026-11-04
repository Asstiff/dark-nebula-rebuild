# Dark Nebula Rebuild

A decompilation and rebuild project for **Dark Nebula Episode One** and **Dark Nebula Episode Two**, restoring recovered game data and C# behavior in Unity 6 for modern Android devices.

This is an independent fan restoration and reverse-engineering project. It is not an official release and is not affiliated with the original developers or publishers. Recovered code is reconstructed from compiled game assemblies; it is not the original development source tree.

## Projects

- `EpisodeOne/`: standalone Unity project for Episode One.
- `EpisodeTwo/`: standalone Unity project for Episode Two.

Open either project in **Unity 6000.4.11f1** with Android Build Support, IL2CPP and ARM64 support. Each episode keeps its own Assets, Packages and ProjectSettings. Generated Library folders, toolchains, signing keys, device saves and APKs are excluded from Git.

## Engineering scope

- Modern Unity and Android API compatibility.
- Portrait framing for tall phones while retaining the authored horizontal scale.
- Recovery of legacy sprite shaders and separate alpha textures.
- Reconstruction of level loading, gameplay, mechanisms and moving-platform physics.
- Forest-to-Matrix transitions, sprite layers and camera effects.
- Selected Episode Two RGB atlas data recovered from the iOS build.

The display names are **Dark Nebula Episode One** and **Dark Nebula Episode Two**.

Each project can be opened and built independently. Enable its recovered scenes in Unity Build Settings and select Android with the IL2CPP scripting backend and ARM64 architecture. The project-specific build entry points are in each project's `Assets/Editor` directory. Build tools, investigation notes, validation reports and test packages are maintained locally.

## License and original content

The MIT license in `LICENSE` applies only to original contributions made for this rebuild. Recovered Dark Nebula code, levels, artwork, audio, fonts and third-party components retain their respective rights and licenses. No ownership or permission to relicense those materials is claimed. See [NOTICE](NOTICE.md).

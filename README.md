# Dark Nebula Rebuild

A decompilation and rebuild project for **Dark Nebula Episode One** and **Dark Nebula Episode Two**, integrating content recovered from iOS and Android versions and restoring game behavior in Unity 6 for modern mobile devices. Android builds are available; iOS compatibility and IPA packaging are planned.

This is an independent fan restoration and reverse-engineering project. It is not an official release and is not affiliated with the original developers or publishers. Recovered code is reconstructed from compiled game assemblies; it is not the original development source tree.

## Projects

- `EpisodeOne/`: standalone Unity project for Episode One.
- `EpisodeTwo/`: standalone Unity project for Episode Two.

Open either project in **Unity 6000.4.11f1** with Android Build Support, IL2CPP and ARM64 support. Each episode keeps its own Assets, Packages and ProjectSettings. Generated Library folders, toolchains, signing keys, device saves and APKs are excluded from Git.

## Project features

### Core features

- [x] Unity 6 and modern Android compatibility.
- [x] Tall-screen portrait framing that preserves the original horizontal scale.
- [x] Recovered sprite shaders and separate alpha textures.
- [x] Reconstructed level loading, gameplay, mechanisms and moving-platform behavior.
- [x] Content integrated from different iOS and Android branches and versions.
- [ ] iOS compatibility and IPA packaging.

### Extensions

- [ ] Optional Episode One timer in the settings of both episodes.
- [ ] Language selection and Chinese localization with matching scalable fonts.
- [ ] Gameplay-driven vibration feedback and positional stereo audio.
- [ ] Background-thread gyroscope calibration during each level load, with loading completion waiting for calibration to finish.
- [ ] WASD movement for Unity debugging on Windows, enabled only in DEBUG builds and excluded from Release builds.

The display names are **Dark Nebula Episode One** and **Dark Nebula Episode Two**.

Each project can be opened and built independently. Enable its recovered scenes in Unity Build Settings and select Android with the IL2CPP scripting backend and ARM64 architecture. The project-specific build entry points are in each project's `Assets/Editor` directory. Build tools, investigation notes, validation reports and test packages are maintained locally.

## License and original content

The MIT license in `LICENSE` applies only to original contributions made for this rebuild. Recovered Dark Nebula code, levels, artwork, audio, fonts and third-party components retain their respective rights and licenses. No ownership or permission to relicense those materials is claimed. See [NOTICE](NOTICE.md).

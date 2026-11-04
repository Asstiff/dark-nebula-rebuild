# Dark Nebula Rebuild

A decompilation and rebuild project for **Dark Nebula Episode One** and **Dark Nebula Episode Two**, restoring recovered game data and C# behavior in Unity 6 for modern Android devices.

This is an independent fan restoration and reverse-engineering project. It is not an official release and is not affiliated with the original developers or publishers. Recovered code is reconstructed from compiled game assemblies; it is not the original development source tree.

## Projects

- `EpisodeOne/`: standalone Unity project for Episode One.
- `EpisodeTwo/`: standalone Unity project for Episode Two, including ongoing tall-screen rendering and physics corrections.
- `docs/`: build notes, provenance and current validation status.

Open either project in **Unity 6000.4.11f1** with Android Build Support, IL2CPP and ARM64 support. Each episode keeps its own Assets, Packages and ProjectSettings. Generated Library folders, toolchains, signing keys, device saves and APKs are excluded from Git.

## Rebuild work

- Modern Unity and Android API compatibility.
- Portrait framing for tall phones while retaining the authored horizontal scale.
- Recovery of legacy sprite shaders and separate alpha textures.
- Moving-platform support reconciliation during jump-pad landings.
- Mechanism activation based on the expanded visible region.
- Forest-to-Matrix transition rendering and scan coverage under active verification.
- Selected Episode Two RGB atlas data recovered from the iOS build.

The display names remain **Dark Nebula Episode One** and **Dark Nebula Episode Two**. Version numbers distinguish development builds.

See [build instructions](docs/BUILDING.md), [validation status](docs/STATUS.md) and [provenance](docs/PROVENANCE.md).

The current Episode Two development package is **[0.1.11](https://github.com/FluorescentLava/dark-nebula-rebuild/releases/tag/episode2-v0.1.11)**. [The audio audit](docs/AUDIO-AUDIT.md) records source comparisons, Android encoding measurements and listening excerpts; audio changes have not been applied by that assessment.

## License and original content

The MIT license in `LICENSE` applies only to original contributions made for this rebuild. Recovered Dark Nebula code, levels, artwork, audio, fonts and third-party components retain their respective rights and licenses. No ownership or permission to relicense those materials is claimed. See [NOTICE](NOTICE.md).

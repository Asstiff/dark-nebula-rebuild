# Original game and third-party notices

Dark Nebula, its episode names, recovered gameplay code, level data, textures, audio, fonts and other original materials remain the property of their respective rights holders. Their presence in this reconstruction does not place them under the MIT license or grant redistribution rights.

The repository contains recovered Unity assets and reconstructed C# assemblies alongside new compatibility, rendering, tooling and physics work. Original contribution licensing must not be interpreted as relicensing an entire recovered file or embedded third-party library.

Third-party components include Unity packages, 2D Toolkit (`tk2d`), PlayMaker/HutongGames code and other recovered dependencies. Existing attribution and license notices are retained where available; their original terms continue to apply. Unity itself and Android build tools are not included.

ZCOOL QingKe HuangYou (站酷庆科黄油体) is distributed under the SIL Open Font License 1.1. The original TTF and its license are retained in each episode’s `Assets/Resources/Fonts` directory. The bundled derivative, Dark Nebula CJK FullWidth, adjusts Chinese glyph outlines and advances horizontally to two halfwidth digit cells, preserves the original vertical metrics and Latin glyphs, and remains licensed under SIL OFL 1.1.

Dark Nebula Nickname is a second bundled derivative with the same outlines, character coverage and metrics as Dark Nebula CJK FullWidth, and a separate internal family name. It is imported separately for the user-name font atlas so requesting additional nickname characters does not repack interface font atlases. Both derived TTF files remain licensed under SIL OFL 1.1.

Noto Sans SC is copyright 2014–2021 Adobe, with Reserved Font Name “Source”, and is distributed under SIL Open Font License 1.1. Static Regular (400) and Bold (700) instances provide the Chinese sans-serif match for Episode One’s Arial leaderboard family and Episode Two’s original bold FontInput bitmap family. These are matching CJK faces, not an official Chinese edition of Arial. Their original Unicode coverage is retained while unused OpenType alternate glyphs are omitted. Each episode retains `NotoSansSC-OFL.txt` beside its font resource. Episode Two’s separate `LeaderboardChineseAtlas` is generated from the Bold instance for fixed interface text; names use the font’s separate dynamic atlas, preserving the menu atlas and original text color treatment.

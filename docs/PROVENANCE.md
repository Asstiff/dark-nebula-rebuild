# Provenance

The starting point was recovered Unity data and decompiled managed assemblies for both Dark Nebula episodes. Decompiled identifiers, serialized data and embedded third-party code are retained where needed for compatibility.

Episode Two uses selected RGB atlas data from the recovered iOS package. In particular `Assets/Texture2D/atlas0_49.png` is the recovered iOS 2048-pixel atlas in the original layout; its separate alpha map is evaluated independently because package variants differ in resolution.

Original installation archives, signing keys, development toolchains and user save files are not committed. Unity .meta GUIDs are preserved so serialized references resolve.

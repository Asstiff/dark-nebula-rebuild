# Episode Two 0.1.11 validation

Date: 2026-11-04. Android version code: 12. Application label: **Dark Nebula Episode Two**.

The tested APK is identified by [tested-artifact.json](tested-artifact.json); its SHA-256 matches the [build manifest](../episode2-build.json) and release checksum. Unity export and Android Gradle packaging succeeded. Android manifest metadata confirms ARM64, minimum API 26, target API 35, and no launcher label override. The signing certificate matches the preceding development packages.

Native validation used an Android 16 emulator at **540×1200**. Installation was explicitly restricted to an emulator. No new phone test was performed.

- [Rotated portal overlay](portal-overlay-rotated.png): the grid continues across the expanded top, bottom and sides. The original ring retains its scale, the upper foliage join is continuous, and the original character disappearance is retained.
- [Portal sequence frames](portal-late-check.jpg): native opacity progression during the stone-ring camera rotation.
- [Portal approach](portal-approach.png) and [Matrix arrival](matrix-arrival.png): surrounding sequence states, including restored cyan direction arrows after the whiteout.
- [Both forest scans](scans-check.jpg): original artwork and moving bars. The [native diagnostics](native-diagnostics.txt) log both lower entry edges at viewport Y **1.001404**, just above the actual top, and completion of both scan and portal fixtures without observed runtime exceptions.
- [Installed app information](app-info.png): the application name has no test suffix. Installed package metadata also confirmed version **0.1.11 / 12**.
- [Editor foliage comparison](editor-foliage-comparison.png): left is an earlier internal candidate with the rotated upper seam; middle is the corrected rendering; right temporarily hides the grid to inspect the foliage itself. These editor images supplement the actual APK captures above.

The fixtures fire native trigger handlers and move the camera/player to the relevant authored locations. They establish the listed rendering and sequence checks, not a complete manual level playthrough. The accepted leaderboard, audio import settings, moving-platform logic and frame-target policy are unchanged by this rendering revision.

# Building

Use Unity **6000.4.11f1**, Android Build Support/IL2CPP, an Android SDK with platform 35, Android NDK **27.2.12479018**, and **JDK 17**. The current editor build entry points use the SDK at `%LOCALAPPDATA%/Android/Sdk` on Windows.

Open `EpisodeOne` or `EpisodeTwo` as an independent Unity project. Let Unity import the recovered assets, switch the build target to Android, then use the enabled scenes in Build Settings.

For the Windows batch export and Gradle packaging workflow:

```powershell
$env:DARK_NEBULA_UNITY = 'C:/path/to/6000.4.11f1/Editor/Unity.exe'
$env:DARK_NEBULA_JDK17 = 'C:/path/to/jdk-17'
powershell -ExecutionPolicy Bypass -File Tools/build-android.ps1 -Episode 2
```

The script exports IL2CPP/ARM64, packages the Gradle release variant and copies the result into `Builds`. Close the project in any other Unity editor before building. Unity licensing must be configured locally. The source does not include signing keys; local development signing is used.

Current Android identifiers: Episode One `com.codex.darknebula.episode1.test`; Episode Two `com.darknebula.reference.tall`. Episode Two retains its existing test identifier so `adb install -r` preserves the current device saves. Desktop labels are `Dark Nebula Episode One` and `Dark Nebula Episode Two`.

Optional Episode Two test intent `--ez portalReview true` runs a controlled portal capture, firing preceding native trigger handlers and moving the player to the portal approach. It is a diagnostic fixture, not a complete level playthrough. Normal launch is manual gameplay.

The repository excludes local editor state, APKs, source installation archives and user saves. Embedded credentials in the recovered localization utility have been removed; restoration uses local packaged content.

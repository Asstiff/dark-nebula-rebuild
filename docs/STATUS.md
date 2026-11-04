# Validation status — 2026-11-04

Current Episode Two build: **0.1.11 / Android version code 12**, display name **Dark Nebula Episode Two**. Episode One retains its existing 0.1.7 configuration and **Dark Nebula Episode One** label.

## Current behavior

- The forest scan keeps its original sprite, two-second duration and destination. A one-frame `TallScanEntry` component starts the curve after the camera and scrolling parent finish LateUpdate. This fixes a native trigger timing problem missed by the earlier editor-only fixture. No repeated scan tiles or stretched scan artwork are used.
- The translucent Matrix overlay covers the expanded portal view. Its original central ring stays at the authored scale; neutral perimeter tiles continue outside it. Extension materials use the same additive blending and native opacity as the original layer, while avoiding extra depth writes. They are enabled only during the stone-ring approach, so the earlier scan patches are unchanged. During that approach, transparent scenery draws before the grid; this prevents split canopy batches from cutting a horizontal band out of the overlay as the camera rotates. The original grid's depth write is preserved. The recovered header's white padding tile is replaced rather than hiding valid overlay rows.
- The upper forest portal foliage retains its silhouette. The opaque exterior and the join across its upper four tile rows blend into the original undergrowth. Including the fourth row removes the remaining dark wedge at the rotated top edge. A complete eight-row undergrowth cycle and half-texel UV inset avoid incomplete repeats and black atlas-gutter seams.
- The temporary puck material/render-queue/depth override is removed. The character disappears during the stone-ring transition as originally designed.
- The accepted post-level leaderboard remains unchanged.
- The display name contains no Jump Fix, Preview or similar suffix.

## Verified on the actual Android package

Fresh Unity export and fresh Android Gradle packaging passed with zero build errors. Legacy serialization/deprecation warnings remain. The APK is ARM64, minimum API 26, target API 35.

Android 16 emulator, **540×1200**:

- Both scan flags 10 and 11 logged a lower bar edge at normalized viewport Y **1.001404**, just outside the top. Native recordings show both bars entering from above and traversing the view.
- The native portal sequence completed through flags 60, 104, 106, 103, 105 and 99, without observed runtime exceptions. Recordings show the original character disappearance and the corrected top foliage transition before the whiteout.
- Installed Android app information displays **Dark Nebula Episode Two**. Manifest label and launcher activity were checked; the launcher has no label override.
- APK signatures match the preceding 0.1.8-preview and 0.1.9 artifacts. Package identity is unchanged, enabling an upgrade without intentionally removing saves.

Selected screenshots and native scan diagnostics are in [episode2-0.1.11 validation](validation/episode2-0.1.11). Package identity and checksum are in the [build manifest](validation/episode2-build.json).

The user reported that their linked 0.1.9 download still showed the old name and old behavior. Local 0.1.9 metadata and emulator results did not establish the cause of that delivery discrepancy. The newer packages use unique versioned filenames and dedicated GitHub release downloads to make the delivered artifacts unambiguous.

The user deferred phone testing. Emulator fixtures do not establish complete manual level playthrough or phone acceptance.

## Retained earlier checks

The level-one mechanism sweep recorded **2,632 frames / 22,671 visible entity samples / zero frozen visible samples**. Wake-bound checks covered scrolling parents, large visible children, a taller frustum and an intentionally distant sleeping object. This revision does not change those wake-distance rules.

Eight controlled jump-pad/moving-platform landings passed earlier, with departure as a negative control. A preceding Vivo V2307A measurement improved median presentation intervals from about 33.30 ms to 16.64 ms after the frame-target change. Those are historical measurements, not new 0.1.11 phone measurements.

## Audio assessment

[Audio source and encoding impact report](AUDIO-AUDIT.md): all 116 source clips still use the existing Vorbis quality-0 import settings. All 83 Episode Two files match recovered iOS payloads byte-for-byte. Twenty-seven Android encoding fixtures quantify the additional error and the size tradeoff of higher-quality encoding. Raising quality on direct MP3 inputs alone produced identical output in these tests; PCM-decoded inputs responded to high-quality Vorbis. The assessment does not apply audio changes to the game.

## Remaining source limitation

Individual cyan arrow contours are approximately 64 pixels wide in the current separate mask. The matching recovered iOS mask is smaller than the current Android mask. The restored native 2048×2048 RGB atlas is uncompressed, but it cannot supply a genuinely higher-resolution contour. No fabricated or redrawn replacement is included.

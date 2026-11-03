# Validation status ¡ª 2026-11-04

Episode Two development build: **0.1.9**, Android version code **10**. Display name: **Dark Nebula Episode Two**. Episode One retains its existing **Dark Nebula Episode One** label and 0.1.7 build configuration.

## Changes in this revision

- Restored the original player disappearance during the stone-ring transition. Removed the temporary puck material/render-queue override and custom depth-test parameter at the user's request.
- Separated the top forest portal cap from the main tile batch. During the tall portal approach, its black exterior and the matching Matrix overlay cap are hidden. The original central portal artwork remains.
- Retained the original scan sprite, two-second curve duration and destination. The entry point alone moves far enough outside the actual tall viewport for the entire bar to enter from the top. No repeated scan-edge tiles or stretched scan artwork are added.
- Expanded mechanism wake distances with camera height and visible child bounds. Moving scrolling parents retain dynamic bounds.
- Disabled lossy RGB block compression for the recovered forest/Matrix atlas.
- Preserved the user-accepted post-level leaderboard layout.

## Verified

Android ARM64 export and Gradle packaging succeeded with zero build errors. APK metadata confirms version 0.1.9/code 10 and the original Episode Two display name. The source retains legacy serialization/deprecation warnings. See [build manifest](validation/episode2-build.json).

Unity editor at **540¡Á1200**:

- Both native scan trigger handlers (flags 10 and 11) captured. At activation, each scan bar's lower edge is at normalized viewport Y **1.0014** (just outside the top); each then crosses the screen and leaves below it.
- A sweep across the level-one forest region produced **2,632 frames**, **22,671 visible entity samples**, and **0 frozen visible samples**.
- Wake-bound checks passed for a scrolling parent, a large visible child, a taller frustum and an intentionally distant sleeping object.
- The native stone-ring sequence was captured before the whiteout. Layer isolation identified the black cap; the corrected capture shows its removal and the restored original player disappearance.
- Atlas import audit: RGB **2048¡Á2048 RGB24**, separate mask **1024¡Á1024 RGB24**, both uncompressed.

Earlier checks retained from the preceding work: eight controlled jump-pad/moving-platform landings passed; leaving the platform falls as expected. A prior Vivo V2307A measurement improved median presentation intervals from about 33.30 ms to 16.64 ms after the frame-rate target change. Those are historical measurements, not a fresh measurement of 0.1.9.

The user deferred phone testing for this revision. Editor trigger fixtures and scene sweeps do not establish a complete manual level playthrough or final phone acceptance.

## Remaining visual limitation

The cyan direction arrows use a separate mask. Their current individual contours are approximately 64 pixels wide. The matching recovered iOS mask atlas is only 512¡Á512 (smaller than the current 1024¡Á1024 mask), so replacing the RGB atlas cannot supply a higher-resolution contour. No fabricated or redrawn replacement is included. Additional source material would be needed to establish a genuine higher-resolution original arrow.

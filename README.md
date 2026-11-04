# Dark Nebula Rebuild

A decompilation and rebuild project for **Dark Nebula Episode One** and **Dark Nebula Episode Two**, integrating content recovered from iOS and Android versions and restoring game behavior in Unity 6 for modern mobile devices. Android builds are available; iOS compatibility and IPA packaging are planned.

This is an independent fan restoration and reverse-engineering project. It is not an official release and is not affiliated with the original developers or publishers. Recovered code is reconstructed from compiled game assemblies; it is not the original development source tree.

## Projects

- `EpisodeOne/`: standalone Unity project for Episode One.
- `EpisodeTwo/`: standalone Unity project for Episode Two.

Open either project in **Unity 6000.4.11f1** with Android Build Support, IL2CPP and ARM64 support. Each episode keeps its own Assets, Packages and ProjectSettings. Generated Library folders, toolchains, signing keys, device saves and APKs are excluded from Git.

## 项目功能

✅ 表示已实现的基础功能；❌ 表示尚待完成或验收的项目。

- [x] ✅ Unity 6 与现代 Android API 兼容。
- [x] ✅ 保留原版水平尺度的竖屏关卡视野适配。
- [x] ✅ 恢复旧版精灵着色器与分离的透明度纹理。
- [x] ✅ 恢复关卡加载、游戏逻辑、机关和移动平台运行逻辑。
- [x] ✅ 从 iOS、Android 的不同分支版本中整合内容并入重建工程。

### 还原与工程待办

- [ ] ❌ 完成 iOS 运行兼容与 IPA 打包，提供 Android、iOS 双平台支持。
- [ ] ❌ 修复森林第二次扫描的高屏适配：第一次扫描正常，第二次仍从原窄屏范围开始；保留原扫描动画，只调整其入屏位置。
- [ ] ❌ 统一主菜单、排行榜、加载及其他游戏界面的高屏适配规则，核对缩放、视野和裁切逻辑的一致性；保持已验收的通关排行榜表现。
- [ ] ❌ 整合新增适配代码，提高共用逻辑的复用性，清理多轮修改遗留的废代码和过时注释，并检查场景切换、重复操作及异常尺寸下的健壮性。
- [ ] ❌ 修复加载背景图的显示比例：保持原图纵横比，仅允许左右裁切，禁止非等比拉伸填充。
- [ ] ❌ 修复重复点击陀螺仪校准按钮后，按钮文字消失的问题。
- [ ] ❌ 修复字体中 `I` 等字符的字间距，优先寻找并使用与原版对应的 TTF 字体。

### 拓展功能待办

以下功能留待基础还原完成后推进，本轮不实现。

- [ ] ❌ 将一代计时器作为一代、二代设置中的可选功能。
- [ ] ❌ 完善语言系统与中文适配，寻找匹配原版风格、支持相应字符的 TTF 字体。
- [ ] ❌ 增加跟随游戏事件的震动反馈和具有空间位置的立体声音效。
- [ ] ❌ 每次进入关卡时，在加载期间分配独立线程执行一次陀螺仪矫正；主线程等待矫正完成后才判定加载结束，应用启动时不执行。
- [ ] ❌ 增加 Unity Windows 调试用的 WASD 移动控制，仅在 DEBUG 构建启用，Release 构建不保留。

The display names are **Dark Nebula Episode One** and **Dark Nebula Episode Two**.

Each project can be opened and built independently. Enable its recovered scenes in Unity Build Settings and select Android with the IL2CPP scripting backend and ARM64 architecture. The project-specific build entry points are in each project's `Assets/Editor` directory. Build tools, investigation notes, validation reports and test packages are maintained locally.

## License and original content

The MIT license in `LICENSE` applies only to original contributions made for this rebuild. Recovered Dark Nebula code, levels, artwork, audio, fonts and third-party components retain their respective rights and licenses. No ownership or permission to relicense those materials is claimed. See [NOTICE](NOTICE.md).

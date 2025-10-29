# Changelog

All notable changes to GTFO Artifact Generator will be documented in this file.

## [1.0.0] - 2025-01-XX

### Added
- Initial release
- In-game IMGUI UI accessible via F6 key
- Custom Artifact Booster creation system
- Template selection from BoosterImplantTemplate
- Effect addition with custom values (0.0 to 5.0)
- Condition selection (LowHealth, StandingStill, etc.)
- Uses slider (1-3 charges)
- Host-only booster application with network sync
- Automatic cursor enabling when UI is open
- Retry button for failed data loads
- Dynamic type discovery via reflection
- Nested type support for IL2CPP compatibility
- ESC key to close UI
- Clear All button to reset selections
- Real-time status messages

### Technical
- BepInEx 6 IL2CPP plugin architecture
- Harmony patching for GameStateManager
- ClassInjector for IL2CPP MonoBehaviour registration
- IMGUI (OnGUI) for IL2CPP compatibility
- Reflection-based type scanning with nested type support
- Automatic DLL deployment to r2modman profiles

### Known Issues
- Data loading only works in lobby/expedition (not main menu)
- First load may take 1-2 seconds due to reflection scanning
- Console spam if retry button is clicked too frequently

---

## Future Plans

### Planned Features
- [ ] Preset saving/loading system
- [ ] Config file for custom keybinds
- [ ] Booster history/favorites
- [ ] Import/export booster configs
- [ ] Visual effect preview
- [ ] Multi-language support
- [ ] Better error messages with solutions

### Potential Improvements
- [ ] Optimize reflection performance
- [ ] Add booster validation
- [ ] Support for modded effects/conditions
- [ ] In-game documentation/tooltips
- [ ] Mouse wheel scrolling in UI

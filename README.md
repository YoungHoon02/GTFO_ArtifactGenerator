# GTFO Artifact Generator

**Host-only BepInEx plugin for generating custom Artifact Boosters in-game**

Press **F6** to open the in-game UI, select effects, conditions, and template, then apply custom boosters to yourself!

---

## Features

- 🎮 **In-game IMGUI UI** - Press F6 to toggle
- 🎯 **Custom Booster Creation** - Mix and match effects, conditions, and templates
- 🔄 **Auto-sync** - Host applies boosters that sync to all players
- 🖱️ **Mouse Support** - Cursor automatically enabled when UI is open
- 🔍 **Auto-reload** - Retry button if data fails to load

---

## Installation

### Option 1: r2modman (Recommended)

1. Install [r2modman](https://thunderstore.io/c/gtfo/p/ebkr/r2modman/)
2. Select **GTFO** in r2modman
3. Search for "**GTFO Artifact Generator**" and click Install
4. Click "**Start modded**" to launch the game

### Option 2: Manual Installation

1. Download the latest release from [Releases](https://github.com/YoungHoon02/GTFO_ArtifactGenerator/releases)
2. Extract `GTFO_ArtifactGenerator.dll` to:
   ```
   [GTFO Install]/BepInEx/plugins/GTFO_ArtifactGenerator/
   ```
3. Make sure BepInEx 6 (IL2CPP) is already installed
4. Launch the game

---

## Usage

### Quick Start

1. **Launch GTFO** and enter a lobby or expedition
2. Press **F6** to open the UI
3. Click **"Retry Load"** if no options appear (wait for game data to load)
4. Select a **Template** (e.g., Bold, Aggressive)
5. Add **Effects** with values (e.g., DamageBoost = 0.20 for +20%)
6. (Optional) Add **Conditions** (e.g., LowHealth)
7. Set **Uses** (1-3)
8. Click **APPLY** (Host only!)
9. Press **ESC** or **CLOSE** to exit

### Controls

| Key | Action |
|-----|--------|
| **F6** | Toggle UI |
| **ESC** | Close UI |
| Mouse | Navigate UI (cursor auto-enabled) |

### UI Sections

#### 1. Template Selection
Choose a base template for your booster:
- **Muted** (100)
- **Bold** (101)
- **Aggressive** (102)
- **Bleed** (116)
- And more...

#### 2. Effects
Add multiple effects with custom values (0.0 to 5.0):
- **DamageBoost** - Increased weapon damage
- **HealthBoost** - Increased max health
- **MovementSpeedBoost** - Faster movement
- **InfectionResistance** - Infection reduction
- **ReviveSpeedBoost** - Faster revives
- **WeaponReloadSpeedBoost** - Faster reloads
- And more...

Example: `DamageBoost = 0.20` = +20% damage

#### 3. Conditions (Optional)
Add activation conditions:
- **LowHealth** - Only active when health is low
- **StandingStill** - Only active when not moving
- **WithinZone** - Only active in certain areas

#### 4. Uses
Set how many times the booster can be used (1-3)

---

## Troubleshooting

### "No templates/effects found!"
**Cause:** Game data not loaded yet (too early in startup)

**Solution:**
1. Wait until you're in a lobby or expedition
2. Click the **"Retry Load"** button
3. Check BepInEx console for errors

### F6 doesn't open UI
**Cause:** Plugin not loaded or UI initialization failed

**Solution:**
1. Check BepInEx console for errors
2. Verify DLL is in `BepInEx/plugins/GTFO_ArtifactGenerator/`
3. Restart the game

### "Host only!" error when clicking APPLY
**Cause:** Only the host can apply custom boosters

**Solution:**
- Create a solo lobby or be the host

### Cursor not visible
**Cause:** Game's FPS mode locks the cursor

**Solution:**
- Already handled! Opening the UI (F6) automatically enables the cursor

### Console spam / logs repeating
**Cause:** Clicking "Retry Load" too many times

**Solution:**
- Wait a few seconds between retries
- Only retry after entering lobby/expedition

---

## Development

### Building from Source

#### Prerequisites
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- BepInEx 6 (IL2CPP) installed in GTFO
- GTFO run at least once to generate interop assemblies

#### Build Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/YoungHoon02/GTFO_ArtifactGenerator.git
   cd GTFO_ArtifactGenerator
   ```

2. **Configure paths** (edit `Directory.Build.props`)
   ```xml
   <GTFOPath>C:\SteamLibrary\steamapps\common\GTFO</GTFOPath>
   <R2ModManProfile>C:\Users\YourName\AppData\Roaming\r2modmanPlus-local\GTFO\profiles\Default</R2ModManProfile>
   ```

3. **Build**
   ```bash
   dotnet build
   ```

4. **Output**
   DLL is automatically copied to:
   ```
   [R2ModManProfile]/BepInEx/plugins/GTFO_ArtifactGenerator/
   ```

### Project Structure

```
GTFO_ArtifactGenerator/
├── Directory.Build.props           # Path configuration
├── GTFO_ArtifactGenerator/
│   ├── Plugin.cs                   # BepInEx entry point, IL2CPP type registration
│   ├── ArtifactSelectorUI.cs       # IMGUI UI implementation
│   ├── CustomBoosterFactory.cs     # pBoosterImplantData builder
│   ├── UIInitPatch.cs              # GameStateManager patch to create UI
│   └── ArtifactPickupPatch.cs      # Optional artifact pickup hook
└── README.md
```

### Key Implementation Details

#### IL2CPP Type Registration
Custom MonoBehaviour types must be registered before use:
```csharp
ClassInjector.RegisterTypeInIl2Cpp<ArtifactSelectorUI>();
```

#### Reflection for Dynamic Type Discovery
Uses reflection to find `BoosterImplantEffect`, `BoosterImplantCondition`, and `BoosterImplantTemplate` types, including nested classes.

#### IMGUI (OnGUI) for IL2CPP Compatibility
Uses Unity's immediate mode GUI instead of Unity UI for better IL2CPP compatibility.

---

## API Reference

### BoosterImplantManager (Game API)

```csharp
// Sync booster to player
bool SyncSetBoosterImplant(SNet_Player player, pBoosterImplantData data)

// Create from template
pBoosterImplantData CreateImplantFromDataBlock(uint templateID)

// Get local player
bool TryGetLocalPlayer(out SNet_Player player)
```

### CustomBoosterFactory (Plugin)

```csharp
pBoosterImplantData BuildFromTemplate(
    uint templateId,
    IReadOnlyList<(uint effectId, float value)> effects,
    IReadOnlyList<uint> conditions,
    int uses)
```

---

## Changelog

### v1.0.0 (Current)
- Initial release
- IMGUI-based UI with F6 toggle
- Dynamic type discovery via reflection
- Nested type support for IL2CPP
- Host-only custom booster application
- Retry button for failed data loads
- Auto cursor enabling

---

## Known Issues

- **Reflection Performance**: First load may take 1-2 seconds while scanning assemblies
- **Main Menu Limitation**: Data loading only works in lobby/expedition (game data not initialized in main menu)

---

## Contributing

Contributions welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Submit a pull request

---

## License

This project is provided for educational and modding purposes.

**Disclaimer:** This is a third-party mod not affiliated with or endorsed by 10 Chambers. Use at your own risk.

---

## Credits

- **BepInEx** - IL2CPP plugin framework
- **HarmonyX** - Runtime patching
- **Unity** - IMGUI system
- **10 Chambers** - GTFO game

---

## Support

- **Issues**: [GitHub Issues](https://github.com/YoungHoon02/GTFO_ArtifactGenerator/issues)
- **Thunderstore**: [GTFO Artifact Generator](https://thunderstore.io/c/gtfo/p/yourusername/GTFOArtifactGenerator/)

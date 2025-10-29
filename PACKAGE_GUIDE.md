# Thunderstore Packaging Guide

This guide explains how to package and upload GTFO Artifact Generator to Thunderstore for r2modman distribution.

## Prerequisites

1. **Thunderstore Account**
   - Create account at https://thunderstore.io/
   - Verify email

2. **Thunderstore CLI** (Optional but recommended)
   ```bash
   dotnet tool install -g tcli
   ```

3. **Icon File**
   - Create a 256x256 PNG icon
   - Save as `icon.png` in project root

## Package Structure

Your package must have this exact structure:

```
GTFO_Artifact_Generator/
├── icon.png                           # 256x256 PNG (REQUIRED)
├── manifest.json                      # Package metadata (REQUIRED)
├── README.md                          # Mod description (REQUIRED)
├── CHANGELOG.md                       # Version history (OPTIONAL)
└── plugins/
    └── GTFO_ArtifactGenerator/
        └── GTFO_ArtifactGenerator.dll # Your plugin DLL
```

## Step-by-Step Packaging

### 1. Prepare Files

```bash
# Create package directory
mkdir GTFO_Artifact_Generator
cd GTFO_Artifact_Generator

# Copy required files
cp icon.png .
cp manifest.json .
cp README.md .
cp CHANGELOG.md .

# Create plugins directory structure
mkdir -p plugins/GTFO_ArtifactGenerator

# Copy built DLL
cp ../GTFO_ArtifactGenerator/bin/Debug/net6.0/GTFO_ArtifactGenerator.dll plugins/GTFO_ArtifactGenerator/
```

### 2. Verify manifest.json

```json
{
  "name": "GTFO_Artifact_Generator",
  "version_number": "1.0.0",
  "website_url": "https://github.com/yourusername/GTFO_ArtifactGenerator",
  "description": "Host-only in-game UI for creating custom Artifact Boosters. Press F6 to mix and match effects, conditions, and templates!",
  "dependencies": [
    "BepInEx-BepInExPack_IL2CPP-6.0.0"
  ]
}
```

**Important:**
- `name` must match folder name (use underscores, not hyphens)
- `version_number` must be semantic versioning (X.Y.Z)
- `dependencies` must list exact package versions

### 3. Create ZIP Archive

**Windows:**
```bash
# Using PowerShell
Compress-Archive -Path * -DestinationPath ../GTFO_Artifact_Generator-1.0.0.zip

# Or using 7-Zip
7z a ../GTFO_Artifact_Generator-1.0.0.zip *
```

**Linux/Mac:**
```bash
zip -r ../GTFO_Artifact_Generator-1.0.0.zip .
```

**Important:** The ZIP must contain the files directly, NOT a root folder!

### 4. Verify Package

Unzip and check structure:
```
GTFO_Artifact_Generator-1.0.0.zip
  ├── icon.png
  ├── manifest.json
  ├── README.md
  ├── CHANGELOG.md
  └── plugins/
      └── GTFO_ArtifactGenerator/
          └── GTFO_ArtifactGenerator.dll
```

### 5. Test Locally with r2modman

1. Open r2modman
2. Select GTFO
3. Settings → Browse profile folder
4. Extract your ZIP to `BepInEx/`
5. Test the mod works
6. If issues, fix and re-package

### 6. Upload to Thunderstore

#### Option A: Web Upload (Easiest)

1. Go to https://thunderstore.io/c/gtfo/create/
2. Log in
3. Fill out form:
   - **Team**: Select or create a team
   - **Community**: GTFO
   - **Category**: Mods
   - **Has NSFW content**: No
   - **Upload file**: Select your ZIP
4. Review and submit
5. Wait for approval (usually 24-48 hours)

#### Option B: CLI Upload (Advanced)

```bash
# Initialize (first time only)
tcli init

# Login
tcli login

# Publish
tcli publish --file GTFO_Artifact_Generator-1.0.0.zip
```

## Version Updates

When releasing a new version:

### 1. Update Version Numbers

- `manifest.json`: `"version_number": "1.1.0"`
- `GTFO_ArtifactGenerator.csproj`: `<Version>1.1.0</Version>`
- `Plugin.cs`: `[BepInPlugin(..., ..., "1.1.0")]`

### 2. Update CHANGELOG.md

```markdown
## [1.1.0] - 2025-02-XX

### Added
- New feature X

### Fixed
- Bug Y

### Changed
- Improved Z
```

### 3. Rebuild and Package

```bash
dotnet build -c Release
# Follow packaging steps again with new version number
```

### 4. Upload New Version

Upload the new ZIP to Thunderstore (same process as initial upload)

## Common Issues

### "Invalid manifest"
- Check JSON syntax with https://jsonlint.com/
- Ensure all required fields are present
- Verify version format is X.Y.Z

### "Icon dimensions invalid"
- Icon MUST be exactly 256x256 pixels
- Format must be PNG
- File name must be `icon.png` (lowercase)

### "Dependency not found"
- Ensure dependency names match exactly
- Check dependency versions exist on Thunderstore
- Format: `AuthorName-PackageName-X.Y.Z`

### "Mod doesn't load in r2modman"
- Verify folder structure matches exactly
- Check DLL is in correct location
- Test with manual installation first

### "ZIP structure incorrect"
- Files must be at root of ZIP, not in a subfolder
- Use `zip -r` or `Compress-Archive -Path *` (not folder name)

## Best Practices

1. **Testing**
   - Always test locally before uploading
   - Test with fresh r2modman profile
   - Test with and without other mods

2. **Documentation**
   - Clear, concise README
   - Include troubleshooting section
   - Add screenshots/GIFs if possible

3. **Versioning**
   - Follow semantic versioning
   - Update changelog with every release
   - Tag releases in Git

4. **Dependencies**
   - Only list direct dependencies
   - Use exact version numbers
   - Test with specified dependency versions

5. **Icons**
   - High quality, clear imagery
   - Recognizable at small sizes
   - Relevant to mod function

## Useful Links

- **Thunderstore**: https://thunderstore.io/c/gtfo/
- **Package Format Docs**: https://thunderstore.io/package/create/docs/
- **r2modman**: https://github.com/ebkr/r2modmanPlus
- **BepInEx Docs**: https://docs.bepinex.dev/
- **GTFO Modding Discord**: https://discord.gg/rRMPtv4FAh

## Quick Checklist

Before uploading, verify:

- [ ] icon.png is 256x256 PNG
- [ ] manifest.json has correct version and dependencies
- [ ] README.md is complete and formatted
- [ ] CHANGELOG.md is updated
- [ ] DLL is in `plugins/GTFO_ArtifactGenerator/`
- [ ] ZIP structure is correct (no root folder)
- [ ] Tested locally with r2modman
- [ ] All files are latest versions
- [ ] No debug/test code left in
- [ ] Git repository is tagged with version

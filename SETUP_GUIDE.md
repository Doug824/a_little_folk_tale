# A Little Folk Tale - Unity Setup Guide

## Quick Start

1. **Create New Unity Project**
   - Open Unity Hub
   - Click "New Project"
   - Select Unity 2022.3 LTS
   - Choose "3D (URP)" template
   - Name your project "ALittleFolkTale"

2. **Import Project Structure**
   - Delete the default "Assets/Scenes" folder
   - Copy all files from this template into your project root
   - Unity will import and compile scripts

3. **Configure Project Settings**
   - Go to Edit > Project Settings
   - In Player settings:
     - Company Name: Your Company
     - Product Name: A Little Folk Tale
     - Default Icon: Set your game icon
   - In Input System Package:
     - Active Input Handling: Both (for transition)

4. **Set Up URP**
   - Right-click in Assets/URPSettings
   - Create > Rendering > URP Asset (with Universal Renderer)
   - Go to Edit > Project Settings > Graphics
   - Set Scriptable Render Pipeline Settings to your new URP Asset

5. **Import Input Actions**
   - Select PlayerInputActions.inputactions
   - Click "Generate C# Class" in Inspector
   - This creates the input bindings

## Scene Setup

### Main Menu Scene
1. Create new scene: Assets/_Project/Scenes/MainMenu.unity
2. Add UI Canvas
3. Create UI hierarchy:
   ```
   Canvas
   ├── MainMenuPanel
   │   ├── Title (TextMeshPro)
   │   ├── BookImage
   │   └── Buttons
   │       ├── StartButton
   │       ├── SettingsButton
   │       └── QuitButton
   └── SettingsPanel (inactive)
   ```
4. Add MainMenu script to a GameObject
5. Link all UI references

### Tutorial Path Scene
1. Create new scene: Assets/_Project/Scenes/TutorialPath.unity
2. Set up basic terrain or plane
3. Add player spawn point (empty GameObject tagged "PlayerSpawn")
4. Create path with obstacles
5. Place a Lantern prefab
6. Add trigger zones for tutorial prompts

### Player Setup
1. Create Player Prefab:
   ```
   Player (GameObject)
   ├── Model (Capsule for now)
   ├── CharacterController component
   ├── PlayerController script
   ├── Player Input component
   └── CameraTarget (empty child)
   ```
2. Configure Player Input:
   - Actions Asset: PlayerInputActions
   - Default Map: Player
   - Behavior: Send Messages

### Camera Setup
1. Create camera rig:
   ```
   CameraRig
   └── Main Camera
       └── CameraController script
   ```
2. Set camera position (0, 15, -10)
3. Set rotation (45, 0, 0)
4. Configure CameraController target

## Prefab Creation

### Lantern Prefab
1. Create 3D Object > Cylinder
2. Add Lantern script
3. Add Light component (child object)
4. Add Particle System for glow
5. Create lit/unlit materials
6. Save as prefab

### UI Prefabs
1. Create dialogue box prefab
2. Create health/stamina bar prefabs
3. Create interaction prompt prefab

## Testing Your Setup

1. **Test Input**
   - Play MainMenu scene
   - Test controller navigation
   - Test keyboard input

2. **Test Scene Flow**
   - Start game from MainMenu
   - Verify scene transition
   - Check player spawning

3. **Test Core Systems**
   - Move player with WASD/controller
   - Test attack animation
   - Test roll mechanic
   - Interact with lantern

## Common Issues

### Input Not Working
- Ensure Input System Package is installed
- Check Player Input component configuration
- Verify input actions are generated

### Player Not Moving
- Check CharacterController component
- Verify camera is tagged "MainCamera"
- Check input action bindings

### UI Not Responding
- Ensure EventSystem exists in scene
- Check Canvas render mode
- Verify button interactable state

## Next Steps

1. **Art Integration**
   - Replace capsule with character model
   - Import and set up animations
   - Create environment assets

2. **Expand Systems**
   - Add inventory UI
   - Implement save system
   - Create enemy AI

3. **Polish**
   - Add sound effects
   - Implement particle effects
   - Create scene transitions

## Blender Export Settings
When exporting from Blender:
- Scale: FBX All
- Forward: -Z Forward  
- Up: Y Up
- Apply Unit: ✓
- Apply Transform: ✓

## Git Setup
1. Create .gitignore:
```
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/
*.pidb.meta
*.pdb.meta
*.mdb.meta
sysinfo.txt
*.apk
*.aab
*.unitypackage
*.app
.DS_Store
.vs/
.idea/
```

2. Initialize repository:
```bash
git init
git add .
git commit -m "Initial Unity project setup"
git remote add origin https://github.com/Doug824/a_little_folk_tale.git
git push -u origin main
```
# A Little Folk Tale - Project Status & Reference

## 🎮 Project Overview
**Game Type:** Top-down 3D Action-Adventure (Diablo meets Zelda meets Redwall)  
**Engine:** Unity 2022.3 LTS with Universal Render Pipeline (URP)  
**Target Platforms:** Nintendo Switch (primary), Steam Deck, PC  
**Repository:** https://github.com/Doug824/a_little_folk_tale.git

## 📊 Current Development Status

### ✅ Complete Unity Project Structure Created!

I've set up a comprehensive Unity project template for "A Little Folk Tale" with:

### 📁 File Structure
- Organized folders for Art, Audio, Scripts, Prefabs, Scenes, etc.
- Separated project assets from external/third-party assets
- Clear hierarchy for different game systems

### 🎮 Core Systems
1. **GameManager** - Handles game state, scene loading, save/load
2. **PlayerController** - Full movement, combat, stamina system with controller support
3. **CameraController** - Fixed-angle follow camera (Diablo-style)
4. **DialogueSystem** - Branching dialogue with portraits and choices
5. **AudioManager** - Centralized audio handling for music, SFX, and ambient

### 🎯 Input System
- Complete InputActions file configured for:
  - Controller-first design (Switch/Steam Deck layout)
  - Keyboard & Mouse fallback
  - Separate UI navigation actions

### 📝 ScriptableObjects
- **ItemData** - For weapons, consumables, key items
- **DialogueData** - For conversation trees
- **EnemyData** - For enemy stats and behaviors

### 🔧 Additional Features
- Lantern save point system
- UI components (MainMenu with book animation)
- Package manifest with all required Unity packages
- Complete .gitignore for Unity projects
- Detailed SETUP_GUIDE.md

### 🚀 Ready to Use
This structure is production-ready and includes:
- Namespaces for code organization
- Interface-based interaction system
- Event-driven architecture where appropriate
- Comments for complex sections
- Proper null checking and error handling

### ✅ NEW: Prototype Testing Framework Added!

**Code-First Development Setup:**
- **TestSceneSetup** - Automatically generates test environment with Unity primitives
- **DebugUI** - Real-time player stats monitoring (Health, Stamina, Position, Input)
- **Enhanced PlayerController** - Ready for immediate testing and iteration
- **TestScene.unity** - Complete test scene ready to run

**How to Test in Unity:**
1. Open Unity project with this folder structure
2. Load `Assets/_Project/Scenes/TestScene.unity`
3. Press Play - everything auto-spawns (ground, player, enemies, walls)
4. **Controls**: WASD/Left Stick (move), Space/B (roll), Left Click/X (attack)
5. **Debug Info**: Press F1 to toggle stats overlay
6. **Camera**: Diablo-style fixed-angle automatically follows player

**What Auto-Generates:**
- Green ground plane (20x20 units)
- Blue capsule player with CharacterController
- Red cube enemies for combat testing
- Gray boundary walls
- Camera positioned at optimal Diablo-style angle

**Perfect for:**
- Testing movement feel and responsiveness
- Iterating on combat mechanics
- Adjusting camera angles and follow behavior
- Controller input validation
- Performance testing with multiple enemies

You can now open Unity, create a new URP project, and copy these files in to get started immediately!

---

## 🎯 Game Design Specifications

### Confirmed Design Elements:
- **Engine:** Unity (C#)
- **Modeling Tool:** Blender
- **Camera:** Fixed top-down 3D (Diablo-style)
- **Input:** Controller-first (Switch/Steam Deck), WASD/Mouse secondary
- **Combat:** Melee primary, utility gear (bow, grappling hook, etc.) secondary
- **Visuals:** Hand-painted look, low poly with shading
- **Spells:** No casting; potions/extracts instead
- **Light FX:** Lantern glow, ambient lighting, VFX for key items
- **Vertical Slice:** Start Screen ➜ Tutorial Path ➜ Thistle Hollow ➜ 1 POI

### Core Gameplay Loop:
1. Explore atmospheric environments
2. Combat with melee focus
3. Solve environmental puzzles
4. Collect items and equipment
5. Save progress at lantern checkpoints
6. Experience story through dialogue and exploration

---

## 📋 Next Development Steps

### ✅ COMPLETED: Core Combat & Camera Systems
**Prototype Framework:**
- Unity project structure with all core scripts
- TestScene.unity with auto-setup system
- Ready for immediate Unity testing

**Combat System (COMPLETE):**
- Responsive player movement with WASD controls
- Attack system with timing, range visualization, and effects
- Rolling/dodge mechanics with stamina management
- Enemy AI with health, damage, and knockback
- Visual feedback: hit effects, damage effects, attack cones
- Sound placeholder system for all actions

**Camera System (COMPLETE):**
- Diablo-style fixed-angle camera (40° angle)
- Smooth player following with configurable speed
- Player properly centered in viewport
- Camera shake effects for impact feedback
- Boundary support for level limits
- Singleton pattern for easy access

**Input System (COMPLETE):**
- Direct keyboard input (WASD, Space, Left Mouse, E)
- Input System fallback for controller support
- Reliable input handling without spam or issues

### Current Priority Tasks:
1. **UI System Development**
   - Health and stamina bars
   - Damage numbers
   - Basic HUD elements
   - Menu systems

2. **Save/Load System**
   - Implement existing save system framework
   - Player progress persistence
   - Scene transitions

3. **Inventory System**
   - Item pickup and management
   - Equipment system
   - UI integration

### Future Milestones:
- Replace placeholder assets with custom models
- Implement inventory system UI
- Add enemy AI behaviors
- Create environmental puzzles
- Polish combat feel and feedback
- Implement save/load system
- Add audio and music

---

## 🛠️ Technical Details

### Project Architecture:
- **Namespace:** `ALittleFolkTale`
- **Input:** Unity Input System (new)
- **Rendering:** Universal Render Pipeline
- **UI:** Unity UI with TextMeshPro
- **Audio:** Custom AudioManager singleton

### Key Scripts Location:
- Player Control: `Assets/_Project/Scripts/Characters/PlayerController.cs`
- Game Management: `Assets/_Project/Scripts/Core/GameManager.cs`
- Camera: `Assets/_Project/Scripts/Core/CameraController.cs`
- Dialogue: `Assets/_Project/Scripts/Dialogue/DialogueSystem.cs`
- Items: `Assets/_Project/Scripts/Items/`
- UI: `Assets/_Project/Scripts/UI/`

### Scene Structure:
1. **MainMenu.unity** - Start screen with book
2. **TutorialPath.unity** - Movement and combat tutorial
3. **ThistleHollow.unity** - First hub area

---

## 📌 Important Notes

### Version Control:
- Repository is connected to GitHub
- .gitignore is configured for Unity
- Regular commits recommended

### Testing Priorities:
1. Controller input responsiveness
2. Camera feel and visibility
3. Combat impact and feedback
4. Save system reliability
5. Scene transitions

### Performance Targets:
- 60 FPS on Switch/Steam Deck
- Optimized for handheld play
- Quick load times between scenes

---

## 🔗 Quick References

### Blender Export Settings:
- Scale: FBX All
- Forward: -Z Forward
- Up: Y Up
- Apply Unit: ✓
- Apply Transform: ✓

### Unity Packages Required:
- Input System
- Universal RP
- TextMeshPro
- Cinemachine

### Input Mappings:
- Move: Left Stick / WASD
- Attack: X Button / Left Mouse
- Roll: B Button / Space
- Interact: A Button / E
- Pause: Start / Escape
- Inventory: Y Button / Tab

---

## 🚀 Quick Start Guide

### For Immediate Testing:
1. **Clone Repository**: `git clone https://github.com/Doug824/a_little_folk_tale.git`
2. **Open Unity**: Create new URP project, import all files
3. **Load TestScene**: Open `Assets/_Project/Scenes/TestScene.unity`
4. **Press Play**: Auto-setup creates full test environment
5. **Test Controls**: WASD (move), Space (roll), Left Click (attack), F1 (debug)

### Key Files for Development:
- **Player Logic**: `Assets/_Project/Scripts/Characters/PlayerController.cs`
- **Camera**: `Assets/_Project/Scripts/Core/CameraController.cs`
- **Test Setup**: `Assets/_Project/Scripts/Core/TestSceneSetup.cs`
- **Debug UI**: `Assets/_Project/Scripts/UI/DebugUI.cs`
- **Input Config**: `Assets/_Project/PlayerInputActions.inputactions`

### Development Guidelines:
- Follow `CLAUDE.md` for commit practices
- Use `ALF/` branch naming convention
- Test with controller first, keyboard second
- Commit frequently during development

---

Last Updated: 2025-07-06
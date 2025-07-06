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

### Immediate Tasks:
1. **Unity Project Setup**
   - Create new Unity project with URP template
   - Import all generated files
   - Configure project settings
   - Test basic scene flow

2. **Asset Creation Pipeline**
   - Set up Blender export settings
   - Create player character proxy
   - Design tutorial path layout
   - Build Thistle Hollow greybox

3. **Core Mechanics Implementation**
   - Test movement and camera
   - Implement basic combat
   - Create first lantern checkpoint
   - Add dialogue triggers

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

Last Updated: 2025-07-02
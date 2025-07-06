# A Little Folk Tale - Unity Project

## Project Overview
A top-down 3D action-adventure game inspired by Diablo, Zelda, and Brian Jacques' Redwall series.

## Unity Version
- Unity 2022.3 LTS (or latest stable)
- Universal Render Pipeline (URP)

## Project Structure

### Assets/_Project
Main project assets organized by type:
- **Art/**: All visual assets (models, textures, sprites)
- **Audio/**: Music and sound effects
- **Materials/**: Unity materials and shaders
- **Prefabs/**: Reusable game objects
- **Scenes/**: Game scenes
- **Scripts/**: C# scripts organized by system
- **ScriptableObjects/**: Data containers
- **Textures/**: Raw texture files

### Key Systems
1. **Character Controller**: Top-down movement with controller support
2. **Combat System**: Melee-focused with utility items
3. **Dialogue System**: Branching conversations
4. **Save System**: Lantern-based checkpoints
5. **Inventory System**: Items and equipment management

## Input Configuration
- Primary: Controller (Switch/Steam Deck layout)
- Secondary: Keyboard & Mouse
- Uses Unity Input System

## Build Settings
- Platform: PC, Mac & Linux Standalone
- Graphics: URP with custom lighting
- Target Resolution: 1920x1080
- Aspect Ratio: 16:9

## Getting Started
1. Open project in Unity 2022.3 LTS
2. Install required packages via Package Manager:
   - Universal RP
   - Input System
   - Cinemachine
   - TextMeshPro
3. Open MainMenu scene to start

## Scene Flow
1. MainMenu → Book animation
2. TutorialPath → Learn controls
3. ThistleHollow → First hub area

## Development Guidelines
- Use prefabs for all game objects
- Follow C# naming conventions
- Comment complex systems
- Test with controller first
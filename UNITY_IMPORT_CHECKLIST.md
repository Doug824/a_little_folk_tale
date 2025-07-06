# Unity Import Checklist

## Before Import
- [x] Unity 2022.3 LTS installed
- [x] All project files ready in `/mnt/d/Phoenix Games/a_little_folk_tale/`
- [x] Project structure verified

## Import Steps (Do After Unity Opens)

### 1. Clean Default Scene
- [ ] Delete `SampleScene` from Project window
- [ ] Delete `Scenes` folder if it exists

### 2. Import Project Files
- [ ] Copy entire `Assets` folder contents into Unity's Assets folder
- [ ] Wait for Unity to finish importing (progress bar in bottom-right)
- [ ] Check Console for any import errors

### 3. Configure Project Settings
- [ ] Edit > Project Settings > Input System Package: Set to "Both" or "New"
- [ ] Edit > Project Settings > XR Plug-in Management: Can ignore for now
- [ ] File > Build Settings: Platform should be PC, Mac & Linux Standalone

### 4. Test Scene Setup
- [ ] Navigate to `Assets/_Project/Scenes/TestScene.unity`
- [ ] Double-click to open TestScene
- [ ] Should see camera positioned at angle in Scene view

### 5. First Play Test
- [ ] Press Play button ▶️
- [ ] Should auto-spawn: green ground, blue player, red enemies, gray walls
- [ ] Check Console for any errors

### 6. Input Testing
- [ ] WASD should move blue player capsule
- [ ] Space should trigger roll (if working)
- [ ] Left mouse click should trigger attack
- [ ] F1 should toggle debug UI (if working)

## Common Issues & Solutions

### If Nothing Spawns on Play:
- Check TestSceneManager object exists in Hierarchy
- Check Console for script errors
- Verify TestSceneSetup.cs is attached to TestSceneManager

### If Movement Doesn't Work:
- Check Console for Input System errors
- May need to enable Input System package in Package Manager
- Check PlayerInputActions.inputactions is properly assigned

### If Console Shows Errors:
- Most likely missing dependencies (Input System, TextMeshPro)
- Install via Window > Package Manager

## Expected First Test Results
- ✅ Player (blue capsule) spawns at center
- ✅ Ground (green plane) visible
- ✅ 3 enemies (red cubes) scattered around
- ✅ Boundary walls (gray cubes) at edges
- ✅ Camera follows player from diagonal angle
- ✅ Movement with WASD works smoothly

## Next Steps After Successful Test
1. Fine-tune movement speed in PlayerController
2. Test combat mechanics
3. Adjust camera distance/angle
4. Add visual feedback effects
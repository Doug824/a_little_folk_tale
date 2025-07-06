# Claude Development Guidelines

## Rules for Claude
- Do NOT add "Generated with Claude Code" or "Co-Authored-By: Claude" comments to commit messages
- Keep commit messages clean and professional

## Git Commit Practices

### Commit Frequency
- Commit after completing each meaningful feature or fix
- Commit when switching between major tasks
- Commit before making significant changes to working code

### Before Committing
1. Run tests if available
2. Run lint/typecheck commands:
   - Check package.json for available scripts
   - Common commands: `npm run lint`, `npm run typecheck`

### Commit Message Format
- Use clear, descriptive messages
- Format: `<type>: <description>`
- Types: feat, fix, docs, style, refactor, test, chore

## Branch Naming Convention
Use the following format for branch names:
```
ALF/{relevant_descriptive_name}
```

Examples:
- `ALF/implement_player_movement`
- `ALF/add_dialogue_system`
- `ALF/fix_camera_controller`
- `ALF/create_lantern_save_points`

### Important Reminders
- Never commit secrets or API keys
- Always review changes with `git diff` before committing
- Create meaningful commits that can be easily understood later

## Project-Specific Notes
- This is a Unity game project ("A Little Folk Tale")
- Main development branch: main
- Repository: https://github.com/Doug824/a_little_folk_tale.git

## When to Commit
- After creating/modifying core scripts (PlayerController, GameManager, etc.)
- After setting up new scenes or prefabs
- After implementing new game systems
- After completing items from PROJECT_STATUS.md milestones
- Before switching between major development tasks

## Unity-Specific Checks
- Ensure .meta files are included with their assets
- Verify scene references are saved
- Check that prefab changes are applied

## Test Commands (Unity)

### Play Mode Tests
```bash
# Run from Unity Editor
# Window > General > Test Runner > PlayMode
```

### Edit Mode Tests  
```bash
# Run from Unity Editor
# Window > General > Test Runner > EditMode
```

### Command Line Testing (if configured)
```bash
# Run all tests
Unity -batchmode -runTests -projectPath . -testResults results.xml

# Run specific test assembly
Unity -batchmode -runTests -projectPath . -testResults results.xml -testFilter "TestAssemblyName"
```

### Build Verification
```bash
# Build for target platform
Unity -batchmode -quit -projectPath . -buildTarget StandaloneWindows64 -buildWindows64Player builds/game.exe
```
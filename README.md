# Confetti Flow

Confetti Flow is a sensory-first mobile Unity app focused on calming, tactile interaction.

## Vision

- Soft, satisfying interactions
- No ads, no pressure, no fail states in the first prototype
- Mobile-first for iPhone and Android
- Discovery/reward progression instead of score pressure

## Current Prototype Scope (Prototype_01)

- Balloon spawning
- Balloon tap-to-pop interaction
- Confetti burst and persistence
- Touch swipe interaction with confetti
- Tilt interaction with confetti drift
- Bottom color-fill progression
- Minimal UI (`Pause`, `Reset`, `Continue`, `Next`)

## Development Progress Log

### 2026-03-19

- Created Unity project using Universal 2D template
- Added project folder architecture under `Assets/_Project`
- Implemented modular event-driven gameplay scripts
- Added object pooling for confetti pieces
- Wired balloon spawn/pop pipeline
- Wired confetti burst, swipe, and tilt interaction
- Implemented color fill progression system
- Implemented minimal HUD controls and scene flow
- Verified first playable prototype loop in editor
- Initialized local git repo and created initial commit

## Next Milestones

### Milestone 1 — Feel Polish

- Tune spawn cadence and movement softness
- Improve confetti visual variety (shape, size, motion)
- Add subtle pop SFX variation and ambient bed

### Milestone 2 — Scene Progression

- Build second sensory scene
- Wire `Next` to real scene transition
- Add simple transition polish between scenes

### Milestone 3 — Mobile Validation

- Build and test on Android device
- Test on iOS via Mac build pipeline
- Profile performance and battery behavior

## How to Run

1. Open the project in Unity LTS
2. Open scene: `Assets/_Project/Scenes/Prototype_01.unity`
3. Press Play

## Repository Notes

- Unity-generated folders (`Library`, `Temp`, etc.) are ignored via `.gitignore`
- Keep all game code and assets under `Assets/_Project`

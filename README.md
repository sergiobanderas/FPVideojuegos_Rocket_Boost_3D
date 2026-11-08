# Rocket Boost 3D

A 2.5D Unity game. You pilot a rocket ship, fly through the level, and shoot flying insects that appear in groups.

The camera looks at the XY plane. The ship, the shots, and the enemies share the player's depth.

## Requirements

- Unity **6000.6.3f1**
- Universal Render Pipeline
- Input System and DOTween (included in the project)
- [Git LFS](https://git-lfs.com), because models, textures, audio, and the DOTween DLLs are stored in LFS

## How to open it

1. Clone the repository with Git LFS installed: `git lfs install`, then `git clone`.
2. Open that folder in Unity Hub with version 6000.6.3f1.
3. Open the scene `Assets/Scenes/SampleScene` and press Play.

If the models are invisible or the materials are magenta, the clone did not download the LFS files. From the project folder, run `git lfs pull`.

## Controls

| Action | Keyboard | Gamepad |
| --- | --- | --- |
| Thrust | Space | Right trigger |
| Rotate | A / D or arrow keys | Left stick |
| Shoot | Left click | South button |

Ammo is 10 shots. Infinite ammo can be enabled on the `PlayerShooting` component.

## What the game does

- The ship thrusts toward its nose. Thrust is short, damping is high, and speed is capped, so a boost does not throw the ship across the level.
- The shot is a laser with a beam, a trail, and a muzzle flash. Impacts spark. When an enemy dies, it explodes and returns to its pool.
- The shot sound picks `sfx_laser1` or `sfx_laser2` at random. A new shot cuts the previous sound.
- There are three enemy groups (Silly, Tiger, and Cocodrile). Each has its own pool, color, and spawn area. They appear when the ship passes a position on X.
- They spawn on an ellipse in the gameplay plane, kept apart from each other and always above the ground. Scale is 0.15.
- Enemies float, chase you when you get close, and keep their head pointed at the ship, including while they follow you.
- Touching an enemy costs one life. The ship blinks for one second and cannot be hit again during that time. The run starts with 5 lives. At zero, the scene reloads.

## Scripts

| Script | Role |
| --- | --- |
| `PlayerMovement` | Thrust, rotation, and speed limit |
| `PlayerShooting` | Fire rate, ammo, laser, and shot audio |
| `BulletController` | Beam, trail, impact, and returning the bullet to the pool |
| `CombatVfx` | Muzzle flash, impact, and explosion |
| `ObjectPool` | Bullet pool and enemy pools |
| `EnemySpawner` | Spawn areas, separation, tint, and scale |
| `FlyingEnemy` | Floating, chasing, and looking at the player |
| `EnemyHealth` | Health, explosion, and deactivation on death |
| `PlayerHealth` | Lives, one second of blinking invulnerability, and scene reload |

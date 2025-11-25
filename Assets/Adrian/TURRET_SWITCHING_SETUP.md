# Turret Switching System Setup Guide

This system allows you to click on turrets during gameplay to switch first-person control to them.

## Components Created

1. **TurretController.cs** - Handles first-person control of a turret (camera rotation and shooting)
2. **ClickableTurret.cs** - Makes turrets clickable and provides visual feedback
3. **TurretSwitchManager.cs** - Manages switching between turrets

## Setup Instructions

### Step 1: Add TurretController to Your Turret Prefabs

For each turret prefab you want to be controllable:

1. Select the turret prefab in the Project window
2. Add the `TurretController` component to the root GameObject
3. Configure the following in the Inspector:
   - **Turret Camera**: Assign a Camera component (create one as a child if needed)
   - **Camera Pivot**: The Transform where the camera should be positioned (usually a child empty GameObject at the turret's view position)
   - **Rotating Head**: The Transform that rotates horizontally (the turret head)
   - **Muzzle**: The Transform where projectiles spawn
   - **Projectile Prefab**: The projectile prefab to shoot
   - Adjust **Look Speed**, **Look X Limit**, **Fire Rate**, and **Projectile Damage** as needed

### Step 2: Add ClickableTurret Component

1. Add the `ClickableTurret` component to the same turret GameObject
2. The component requires a Collider (BoxCollider, SphereCollider, etc.) on the turret
3. Optionally configure:
   - **Renderers To Highlight**: Array of Renderer components that will be highlighted when hovering
   - **Highlight Color**: Color to use for highlighting (default: Yellow)
   - **Highlight Intensity**: How bright the highlight should be

### Step 3: Set Up the TurretSwitchManager

1. In your scene, create an empty GameObject (e.g., "TurretSwitchManager")
2. Add the `TurretSwitchManager` component to it
3. Configure:
   - **Turret Layer Mask**: Which layers turrets are on (or leave as "Everything")
   - **Max Click Distance**: Maximum distance to click turrets (default: 1000)
   - **Exit Turret Key**: Key to press to exit turret control (default: Escape)

### Step 4: Camera Setup

For each turret:

1. Create an empty GameObject as a child of the turret (name it "CameraPivot")
2. Position it where you want the first-person camera to be (usually at eye level on the turret)
3. Add a Camera component to the CameraPivot (or create a child GameObject with a Camera)
4. Assign this Camera to the TurretController's "Turret Camera" field
5. Assign the CameraPivot Transform to the "Camera Pivot" field

### Step 5: Collider Setup

Make sure each turret has a Collider component (BoxCollider, CapsuleCollider, etc.) so it can be clicked. The collider should cover the turret's visible area.

## Usage

- **Click on a turret** (left mouse button) to take control of it
- **Mouse movement** rotates the camera and turret head
- **Left mouse button** fires projectiles
- **Escape key** (or configured exit key) exits turret control

## Notes

- When controlling a turret, the cursor is locked and hidden
- When not controlling a turret, hovering over one will highlight it
- Only one turret can be controlled at a time
- The system automatically handles camera switching


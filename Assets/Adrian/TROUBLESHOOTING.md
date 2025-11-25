# Turret Switching Troubleshooting Guide

If your turret isn't clicking, the camera isn't moving, or the turret isn't rotating, follow these steps:

## Quick Checklist

### 1. Is TurretSwitchManager in your scene?
- **Check**: Look in the Hierarchy for a GameObject with `TurretSwitchManager` component
- **Fix**: If missing, create an empty GameObject and add the `TurretSwitchManager` component

### 2. Are all references assigned in TurretController?
On your "Turret Tower 03" prefab, check the `TurretController` component:
- ✅ **Turret Camera**: Must have a Camera assigned
- ✅ **Camera Pivot**: Transform where camera should be (create empty GameObject if needed)
- ✅ **Rotating Head**: The "Turret" child object that rotates
- ✅ **Muzzle**: Where projectiles spawn (can be the Turret child or an empty GameObject)
- ✅ **Projectile Prefab**: Your projectile prefab

### 3. Does the turret have a Collider?
- **Check**: Select "Turret Tower 03" and look for a Collider component
- **Fix**: If missing, add a BoxCollider (or the ClickableTurret will auto-add one)

### 4. Is the Camera set up correctly?
The camera setup is critical:
1. Create an empty GameObject as a child of "Turret Tower 03" (name it "CameraPivot")
2. Position it where you want the first-person view (usually at the top, at eye level)
3. Add a Camera component to the CameraPivot (or create a child with a Camera)
4. Assign this Camera to TurretController's "Turret Camera" field
5. Assign the CameraPivot Transform to "Camera Pivot" field

### 5. Using the Debug Helper
1. Add the `TurretSetupHelper` component to your "Turret Tower 03" prefab
2. Right-click the component in the Inspector
3. Select "Check Turret Setup"
4. Check the Console for any errors (red ❌ messages)

## Common Issues

### Issue: Can't click on turret
**Possible causes:**
- No Collider on the turret
- TurretSwitchManager not in scene
- Camera not found (check Console for errors)
- Turret is on a layer that's excluded from the Layer Mask

**Solutions:**
- Add a Collider component
- Create TurretSwitchManager in scene
- Make sure there's a Main Camera in the scene
- Check TurretSwitchManager's "Turret Layer Mask" setting

### Issue: Camera not moving
**Possible causes:**
- Turret Camera not assigned
- Camera Pivot not set up correctly
- Cursor not locked (check if cursor is visible)

**Solutions:**
- Assign Turret Camera in TurretController
- Set up Camera Pivot properly (see step 4 above)
- The cursor should auto-lock when controlling a turret

### Issue: Turret not rotating
**Possible causes:**
- Rotating Head not assigned
- Rotating Head is the wrong object

**Solutions:**
- Assign the "Turret" child object to "Rotating Head" field
- Make sure it's the object that should rotate horizontally (yaw)

## Step-by-Step Setup for Turret Tower 03

1. **Open the prefab**: Select "Turret Tower 03" in Project window
2. **Add components**: Add `TurretController` and `ClickableTurret` to the root GameObject
3. **Set up camera**:
   - Create empty GameObject child named "CameraPivot"
   - Position it at the top of the turret (where eyes would be)
   - Add Camera component to CameraPivot
   - In TurretController, assign:
     - Turret Camera → the Camera you just created
     - Camera Pivot → the CameraPivot Transform
4. **Set up turret references**:
   - Rotating Head → the "Turret" child object
   - Muzzle → create an empty GameObject as child of "Turret" at the barrel tip, or use "Turret" transform
5. **Add Collider**: Make sure there's a Collider (BoxCollider works well)
6. **In your scene**: Create empty GameObject, add `TurretSwitchManager` component

## Testing

1. Play the scene
2. Click on the turret (left mouse button)
3. The cursor should lock and you should see from the turret's camera
4. Move mouse to rotate camera and turret
5. Left-click to shoot (if projectile prefab is assigned)
6. Press Escape to exit turret control

## Debug Mode

Enable "Debug Mode" in TurretController to see detailed logs in the Console about what's happening.


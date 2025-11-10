# Endless Dodge Runner Game - Unity Setup Guide

This guide will help you set up all the components for your endless dodge runner game in Unity.

## 📋 Prerequisites

- Unity project with the following scripts:
  - `PlayerEndlessMovement.cs`
  - `GroundSpawner.cs`
  - `ObstacleSpawner.cs`
  - `PlayerObstacleCollision.cs`
  - `DistanceCounter.cs`
  - `EndlessGameManager.cs`

## 🎮 Step-by-Step Setup

### 1. Player Setup

1. **Create or Select Player GameObject:**
   - Create a GameObject (e.g., Capsule, Cube, or your player model)
   - Name it "Player"
   - **Tag it as "Player"** (Inspector → Tag → Player)

2. **Add Components to Player:**
   - **Rigidbody** component:
     - Set `Use Gravity` = true (or false if you want floating)
     - Set `Is Kinematic` = false
     - Set `Freeze Rotation` on X and Z axes (only allow Y rotation if needed)
   
   - **Collider** component (BoxCollider, CapsuleCollider, or MeshCollider):
     - Make sure it's not a trigger (unless you want trigger-based collision)
   
   - **PlayerEndlessMovement** script:
     - `Speed`: 5 (adjust as needed)
     - `Lateral Speed`: 5 (for left/right movement)
     - `Max Lateral Distance`: 5 (how far player can move left/right)
     - `Use Lateral Movement`: true (for side-to-side movement)
   
   - **PlayerObstacleCollision** script:
     - `Obstacle Tag`: "Obstacle"
     - `Collision Cooldown`: 0.5 (prevents multiple triggers)

3. **Position Player:**
   - Set position to (0, 1, 0) or appropriate starting position

---

### 2. Ground Setup

1. **Create Ground Tile Prefab:**
   - Create a GameObject (e.g., Plane or Cube)
   - Scale it to your desired size (e.g., 10x1x10)
   - Add a **MeshCollider** or **BoxCollider**
   - Position at (0, 0, 0)
   - Create a material and apply it
   - **Drag it to Prefabs folder** to create a prefab
   - Name it "GroundTile"

2. **Create Ground Spawner GameObject:**
   - Create an empty GameObject
   - Name it "GroundSpawner"
   - Add **GroundSpawner** script:
     - `Ground Tile`: Drag your GroundTile prefab here
     - `Tiles Ahead`: 5 (how many tiles to spawn ahead)
     - `Tile Length`: 10 (length of each tile - should match your prefab)
     - `Despawn Distance`: 20 (distance behind player to despawn)
     - `Ground Width`: 10 (width of ground - should match your prefab)

---

### 3. Obstacle Setup

1. **Create Obstacle Prefabs:**
   - Create obstacle GameObjects (e.g., Cubes, Spheres, or your obstacle models)
   - Add **Collider** components (BoxCollider, SphereCollider, etc.)
   - **Tag them as "Obstacle"** (Inspector → Tag → Add Tag → Create "Obstacle" tag)
   - Make sure colliders are NOT triggers (unless you want trigger-based collision)
   - Create multiple obstacle prefabs for variety
   - **Drag them to Prefabs folder**
   - Name them "Obstacle1", "Obstacle2", etc.

2. **Create Obstacle Spawner GameObject:**
   - Create an empty GameObject
   - Name it "ObstacleSpawner"
   - Add **ObstacleSpawner** script:
     - `Obstacle Prefabs`: Drag all your obstacle prefabs into this array
     - `Min Distance Between Obstacles`: 5
     - `Max Distance Between Obstacles`: 15
     - `Spawn Ahead Distance`: 50
     - `Despawn Behind Distance`: 20
     - `Spawn Width`: 10 (should match GroundSpawner's ground width)
     - `Number Of Lanes`: 3 (for left, center, right lanes)
     - `Spawn On Sides`: true
     - `Spawn Chance`: 0.7 (70% chance to spawn obstacles)
     - `Min Obstacles Per Section`: 1
     - `Max Obstacles Per Section`: 3

---

### 4. UI Setup (Distance Counter)

1. **Create Canvas:**
   - Right-click in Hierarchy → UI → Canvas
   - Name it "GameCanvas"

2. **Create Distance Text:**
   - Right-click on Canvas → UI → Text - TextMeshPro (or Text)
   - Name it "DistanceText"
   - Position it (e.g., top-left corner)
   - Set font size, color, etc.

3. **Create Distance Counter GameObject:**
   - Create an empty GameObject
   - Name it "DistanceCounter"
   - Add **DistanceCounter** script:
     - `Start Z`: 0 (starting Z position)
     - `Distance Unit`: "m" (meters)
     - `Update Interval`: 1 (update every frame)
     - `Distance Text TMP`: Drag your TextMeshPro component here (if using TMP)
     - OR `Distance Text`: Drag your Text component here (if using Unity Text)
     - `Distance Format`: "Distance: {0:F1} {1}" (format string)
     - `Save Best Distance`: true
     - `Best Distance Key`: "BestDistance"

---

### 5. Game Manager Setup

1. **Create Game Manager GameObject:**
   - Create an empty GameObject
   - Name it "GameManager"
   - Add **EndlessGameManager** script:
     - `Is Game Running`: true
     - `Is Game Paused`: false
     - `Player`: Drag your Player GameObject here
     - `Ground Spawner`: Drag your GroundSpawner GameObject here
     - `Obstacle Spawner`: Drag your ObstacleSpawner GameObject here
     - `Distance Counter`: Drag your DistanceCounter GameObject here
     - `Player Collision`: Drag your Player GameObject here (or leave empty, it will find it)
     - `Game Over Scene Name`: "GameOver" (name of your game over scene)
     - `Game Over Delay`: 2 (seconds before loading game over)

---

### 6. Camera Setup

1. **Set Up Camera:**
   - Position camera to follow player (or use Cinemachine)
   - For simple setup:
     - Position at (0, 5, -10)
     - Rotate to look at player
   - Or create a simple follow script:
     ```csharp
     public Transform player;
     public Vector3 offset = new Vector3(0, 5, -10);
     
     void LateUpdate() {
         transform.position = player.position + offset;
     }
     ```

---

### 7. Scene Setup Checklist

- [ ] Player GameObject with:
  - [ ] "Player" tag
  - [ ] Rigidbody component
  - [ ] Collider component
  - [ ] PlayerEndlessMovement script
  - [ ] PlayerObstacleCollision script

- [ ] GroundTile prefab created
- [ ] GroundSpawner GameObject with GroundSpawner script
- [ ] GroundSpawner has GroundTile prefab assigned

- [ ] Obstacle prefabs created with "Obstacle" tag
- [ ] ObstacleSpawner GameObject with ObstacleSpawner script
- [ ] ObstacleSpawner has obstacle prefabs assigned

- [ ] Canvas with DistanceText UI element
- [ ] DistanceCounter GameObject with DistanceCounter script
- [ ] DistanceCounter has UI text component assigned

- [ ] GameManager GameObject with EndlessGameManager script
- [ ] GameManager has all references assigned

- [ ] Camera positioned to view the game

---

## 🎯 Testing Your Setup

1. **Press Play** in Unity
2. **Check Console** for any errors
3. **Test Player Movement:**
   - Use arrow keys or WASD to move
   - Player should move forward automatically
   - Left/Right should move player sideways
4. **Test Ground Spawning:**
   - Ground tiles should spawn ahead of player
   - Old tiles should despawn behind player
5. **Test Obstacle Spawning:**
   - Obstacles should spawn ahead of player
   - Obstacles should be in different lanes
6. **Test Collision:**
   - Collide with an obstacle
   - Player should die and game over scene should load
7. **Test Distance Counter:**
   - Distance should increase as player moves forward
   - Distance should display in UI

---

## 🔧 Common Issues & Solutions

### Issue: Player doesn't move
- **Solution:** Check if Rigidbody is attached and not kinematic
- **Solution:** Check if PlayerEndlessMovement script is enabled

### Issue: Ground doesn't spawn
- **Solution:** Check if GroundSpawner has GroundTile prefab assigned
- **Solution:** Check if Player has "Player" tag

### Issue: Obstacles don't spawn
- **Solution:** Check if ObstacleSpawner has obstacle prefabs assigned
- **Solution:** Check if obstacles have "Obstacle" tag

### Issue: No collision detection
- **Solution:** Check if Player has PlayerObstacleCollision script
- **Solution:** Check if obstacles have "Obstacle" tag
- **Solution:** Check if colliders are not set as triggers (unless using trigger-based collision)

### Issue: Distance counter doesn't update
- **Solution:** Check if DistanceCounter has UI text component assigned
- **Solution:** Check if Player has "Player" tag

---

## 🎨 Optional Enhancements

1. **Add Particle Effects:**
   - Add particle effects when player collides with obstacles
   - Add particle effects when player dies

2. **Add Sound Effects:**
   - Add sound when player collides
   - Add background music

3. **Add Score System:**
   - Modify DistanceCounter to also track score
   - Add collectibles for bonus points

4. **Add Power-ups:**
   - Speed boost
   - Invincibility
   - Shield

5. **Add Difficulty Scaling:**
   - Increase obstacle spawn rate over time
   - Increase player speed over time

---

## 📝 Notes

- Make sure all GameObjects have appropriate tags
- Adjust spawn distances and speeds based on your game's feel
- Test collision detection thoroughly
- Consider using object pooling for better performance with many obstacles
- Save your scene frequently!

---

## 🚀 You're Ready!

Once all components are set up, your endless dodge runner game should be fully functional. Press Play and start dodging those obstacles!

Good luck with your game development! 🎮


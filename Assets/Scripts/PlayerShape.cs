using UnityEngine;

[CreateAssetMenu(fileName = "PlayerShape", menuName = "Game/PlayerShape")]
public class PlayerShape : ScriptableObject
{
    public string shapeName;
    public Mesh mesh;

    public enum ColliderType { Box, Sphere, Capsule, Mesh }
    public ColliderType colliderType;

    public Sprite preview;
}

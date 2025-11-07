using UnityEngine;

/// <summary>
/// ScriptableObject describing a skin: materials for player and obstacles, plus preview.
/// Create via: Assets → Create → Game → Skin
/// </summary>
[CreateAssetMenu(menuName = "Game/Skin", fileName = "NewSkin")]
public class Skin : ScriptableObject
{
	[Tooltip("Unique identifier for this skin (used for PlayerPrefs)")]
	public string id = System.Guid.NewGuid().ToString();

	[Header("Materials")]
	[Tooltip("Material applied to the player renderers")] public Material playerMaterial;
	[Tooltip("Material applied to obstacle renderers")] public Material obstacleMaterial;

	[Header("UI")]
	[Tooltip("Display name for UI")] public string displayName = "Skin";
	[Tooltip("Preview sprite for the selection UI")] public Sprite preview;
}



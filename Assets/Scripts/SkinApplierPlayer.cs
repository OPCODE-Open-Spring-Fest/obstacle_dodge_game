using UnityEngine;

/// <summary>
/// Attach to the Player root. Applies the current skin's player material
/// to all Renderer components under the player.
/// </summary>
public class SkinApplierPlayer : MonoBehaviour
{
	private Renderer[] renderers;

	private void Awake()
	{
		renderers = GetComponentsInChildren<Renderer>(true);
	}

	private void OnEnable()
	{
		if (SkinManager.Instance != null)
		{
			SkinManager.Instance.OnSkinChanged += Apply;
			Apply(SkinManager.Instance.CurrentSkin);
		}
	}

	private void OnDisable()
	{
		if (SkinManager.Instance != null)
		{
			SkinManager.Instance.OnSkinChanged -= Apply;
		}
	}

	private void Apply(Skin skin)
	{
		if (skin == null || skin.playerMaterial == null || renderers == null) return;
		for (int i = 0; i < renderers.Length; i++)
		{
			var r = renderers[i];
			if (r == null) continue;
			// Assign material instance to avoid mutating shared material across prefabs
			r.material = skin.playerMaterial;
		}
	}
}



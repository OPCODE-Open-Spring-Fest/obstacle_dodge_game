using UnityEngine;

/// <summary>
/// Scene-level helper that applies the current skin's obstacle material to all
/// objects tagged "Obstacle" (and their children) whenever the skin changes.
/// Place once per scene (e.g., on an empty GameObject).
/// </summary>
public class SkinApplierObstacles : MonoBehaviour
{
	[SerializeField] private string obstacleTag = "Obstacle";

	private void OnEnable()
	{
		if (SkinManager.Instance != null)
		{
			SkinManager.Instance.OnSkinChanged += ApplyToAll;
			ApplyToAll(SkinManager.Instance.CurrentSkin);
		}
	}

	private void OnDisable()
	{
		if (SkinManager.Instance != null)
		{
			SkinManager.Instance.OnSkinChanged -= ApplyToAll;
		}
	}

	private void ApplyToAll(Skin skin)
	{
		if (skin == null || skin.obstacleMaterial == null) return;
		var obstacles = GameObject.FindGameObjectsWithTag(obstacleTag);
		for (int i = 0; i < obstacles.Length; i++)
		{
			var root = obstacles[i];
			if (root == null) continue;
			var renderers = root.GetComponentsInChildren<Renderer>(true);
			for (int r = 0; r < renderers.Length; r++)
			{
				var rend = renderers[r];
				if (rend == null) continue;
				rend.material = skin.obstacleMaterial;
			}
		}
	}
}



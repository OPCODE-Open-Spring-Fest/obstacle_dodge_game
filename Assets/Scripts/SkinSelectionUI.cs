using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple UI that populates buttons for each available skin and lets user select one.
/// Attach to the Skin Selection Panel. Provide a container and a button prefab.
/// </summary>
public class SkinSelectionUI : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField] private Transform skinsContainer;
	[SerializeField] private Button skinButtonPrefab;

	[Header("Optional")]
	[SerializeField] private TextMeshProUGUI currentSkinLabel;

	private void OnEnable()
	{
		BuildList();
		UpdateCurrentSkinLabel();
	}

	public void BuildList()
	{
		if (skinsContainer == null || skinButtonPrefab == null) return;
		if (SkinManager.Instance == null) return;

		// Clear existing items
		for (int i = skinsContainer.childCount - 1; i >= 0; i--)
		{
			Destroy(skinsContainer.GetChild(i).gameObject);
		}

		var skins = SkinManager.Instance.Skins;
		for (int i = 0; i < skins.Count; i++)
		{
			var skin = skins[i];
			if (skin == null) continue;

			var btn = Instantiate(skinButtonPrefab, skinsContainer);
			btn.onClick.RemoveAllListeners();
			int capturedIndex = i;
			btn.onClick.AddListener(() =>
			{
				SkinManager.Instance.SetSkinByIndex(capturedIndex);
				UpdateCurrentSkinLabel();
			});

			// Try populate visuals if prefab has common children
			var label = btn.GetComponentInChildren<TextMeshProUGUI>();
			if (label != null)
			{
				label.text = string.IsNullOrEmpty(skin.displayName) ? "Skin" : skin.displayName;
			}
			var image = btn.GetComponentInChildren<Image>();
			if (image != null && skin.preview != null)
			{
				image.sprite = skin.preview;
				image.preserveAspect = true;
			}
		}
	}

	private void UpdateCurrentSkinLabel()
	{
		if (currentSkinLabel == null) return;
		var cur = SkinManager.Instance != null ? SkinManager.Instance.CurrentSkin : null;
		currentSkinLabel.text = cur != null && !string.IsNullOrEmpty(cur.displayName) ? cur.displayName : "";
	}
}



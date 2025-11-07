using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton that stores available skins, current selection, and raises change events.
/// Place this on a GameObject in your initial scene. It persists across scenes.
/// </summary>
public class SkinManager : MonoBehaviour
{
	public static SkinManager Instance { get; private set; }

	[Header("Available Skins")]
	[SerializeField] private List<Skin> skins = new List<Skin>();

	[Header("Defaults")]
	[SerializeField] private int defaultSkinIndex = 0;

	private const string PrefKey = "SelectedSkinId";
	private Skin currentSkin;

	public event Action<Skin> OnSkinChanged;

	public Skin CurrentSkin => currentSkin;
	public IReadOnlyList<Skin> Skins => skins;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		LoadOrSetDefaultSkin();
		ApplyCurrentSkin();
	}

	private void LoadOrSetDefaultSkin()
	{
		string savedId = PlayerPrefs.GetString(PrefKey, string.Empty);
		if (!string.IsNullOrEmpty(savedId))
		{
			var found = skins.Find(s => s != null && s.id == savedId);
			if (found != null)
			{
				currentSkin = found;
				return;
			}
		}
		if (skins.Count > 0)
		{
			int idx = Mathf.Clamp(defaultSkinIndex, 0, skins.Count - 1);
			currentSkin = skins[idx];
			SaveCurrentSkin();
		}
	}

	public void SetSkinByIndex(int index)
	{
		if (index < 0 || index >= skins.Count) return;
		SetSkin(skins[index]);
	}

	public void SetSkinById(string id)
	{
		if (string.IsNullOrEmpty(id)) return;
		var found = skins.Find(s => s != null && s.id == id);
		if (found != null)
		{
			SetSkin(found);
		}
	}

	private void SetSkin(Skin skin)
	{
		if (skin == null || skin == currentSkin) return;
		currentSkin = skin;
		SaveCurrentSkin();
		ApplyCurrentSkin();
	}

	private void SaveCurrentSkin()
	{
		if (currentSkin != null)
		{
			PlayerPrefs.SetString(PrefKey, currentSkin.id);
			PlayerPrefs.Save();
		}
	}

	public void ApplyCurrentSkin()
	{
		OnSkinChanged?.Invoke(currentSkin);
	}
}



using Random = UnityEngine.Random;
using System;
using FLD;
using UnityEngine;

public class RandomHint : MonoBehaviour
{
	[Serializable]
	public struct Entry
	{
		public string SpriteId;

		public string StringId;
	}

	[SerializeField]
	private tk2dSprite _sprite;

	[SerializeField]
	private DynamicFont _text;

	[SerializeField]
	private Entry[] _entries;

	private void Start()
	{
		ShowRandomEntry();
	}

	public void ShowRandomEntry()
	{
		if (_entries.Length > 0)
		{
			ShowEntry(_entries[Random.Range(0, _entries.Length)]);
		}
	}

	private void ShowEntry(Entry pEntry)
	{
		_sprite.spriteId = _sprite.GetSpriteIdByName(pEntry.SpriteId);
		string message=pEntry.StringId.Localize();
        if(message.ToUpperInvariant().Contains("PALRINGO"))
        {
            _sprite.gameObject.SetActive(false);
            _text.Text="VIEW YOUR BEST TIMES IN LEADERBOARDS.";
        }
        else _text.Text=message;
	}
}

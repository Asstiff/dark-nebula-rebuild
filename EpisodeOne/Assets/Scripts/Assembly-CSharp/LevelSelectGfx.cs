using System.Text.RegularExpressions;
using UnityEngine;

public class LevelSelectGfx : MonoBehaviour
{
	private tk2dSprite _sprite;

	public bool IsLockedGate { get; set; }

	public void SetState(bool on)
	{
		if (_sprite == null)
		{
			_sprite = GetComponent<tk2dSprite>();
		}
		Regex regex = new Regex("(\\d+)");
		Match match = regex.Match(base.name);
		string value = match.Value;
		if (on)
		{
			SetSpriteOn(value);
			_sprite.color = ((!IsLockedGate) ? Color.white : Color.green);
		}
		else
		{
			_sprite.spriteId = _sprite.GetSpriteIdByName("unlocked_" + value);
			_sprite.color = Color.white;
		}
	}

	private void SetSpriteOn(string level)
	{
		if (level.Length == 1)
		{
			_sprite.spriteId = _sprite.GetSpriteIdByName("Select0" + level);
		}
		else
		{
			_sprite.spriteId = _sprite.GetSpriteIdByName("Select" + level);
		}
		if (_sprite.spriteId < _sprite.GetSpriteIdByName("select_" + level))
		{
			_sprite.spriteId = _sprite.GetSpriteIdByName("select_" + level);
		}
	}
}

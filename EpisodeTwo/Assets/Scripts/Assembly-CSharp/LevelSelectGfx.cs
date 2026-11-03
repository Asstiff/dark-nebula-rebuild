using UnityEngine;

public class LevelSelectGfx : MonoBehaviour
{
	private tk2dSprite _sprite;

	private string _level;

	private void Awake()
	{
		_sprite = GetComponent<tk2dSprite>();
		_level = base.gameObject.name.Substring("Selected".Length);
	}

	public void SetState(bool on)
	{
		if (on)
		{
			if (_level.Length == 1)
			{
				_sprite.spriteId = _sprite.GetSpriteIdByName("Select0" + _level);
			}
			else
			{
				_sprite.spriteId = _sprite.GetSpriteIdByName("Select" + _level);
			}
		}
		else
		{
			_sprite.spriteId = _sprite.GetSpriteIdByName("unlocked_" + _level);
		}
	}
}

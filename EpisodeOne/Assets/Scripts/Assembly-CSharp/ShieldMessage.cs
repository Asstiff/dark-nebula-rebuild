using FLD;
using UnityEngine;

public class ShieldMessage : Entity
{
	[SerializeField]
	private DynamicFont _text;

	public void Play()
	{
		Invoke("Recycle", 3f);
	}

	public void SetStringID(string stringID)
	{
		_text.Text = stringID.Localize();
	}

	private void Recycle()
	{
		ObjectPool.instance.PoolObject(this);
	}
}

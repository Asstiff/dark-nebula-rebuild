public class ShieldMessage : SimpleText
{
	public void Play()
	{
		Invoke("Recycle", 3f);
	}

	private void Recycle()
	{
		ObjectPool.instance.PoolObject(this);
	}
}

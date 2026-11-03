public class MurtyArmAttachment : Entity
{
	protected override void Awake()
	{
		base.Awake();
		if (base.GetComponent<UnityEngine.Collider>() != null)
		{
			base.GetComponent<UnityEngine.Collider>().isTrigger = true;
		}
	}
}

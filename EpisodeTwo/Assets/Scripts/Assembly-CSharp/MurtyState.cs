public class MurtyState : EnemyState
{
	protected MurtyHellicasterEntity murty;

	public float frame;

	public MurtyState(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty = pMurty;
	}
}

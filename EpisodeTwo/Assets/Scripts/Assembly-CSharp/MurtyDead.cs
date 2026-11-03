public class MurtyDead : MurtyState
{
	public MurtyDead(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.spinVelocityFactor = 0f;
		murty.targetSpeed = 0f;
		murty.eyeFactor = 0f;
		murty.bladeFactor = 0f;
		murty.armFactor = 0f;
		murty.speed = 0f;
		murty.showWarning = false;
	}
}

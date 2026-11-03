public class ElectroPuckEntity : Entity
{
	public int color;

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		InitParameters();
	}

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
		InitParameters();
	}

	protected void InitParameters()
	{
		color = GetParameterInt("color");
		base.GetComponent<UnityEngine.Rigidbody>().linearDamping = 2.5f;
		tk2dBaseSprite componentInChildren = GetComponentInChildren<tk2dBaseSprite>();
		componentInChildren.spriteId = componentInChildren.GetSpriteIdByName("electropuck/" + color);
	}
}

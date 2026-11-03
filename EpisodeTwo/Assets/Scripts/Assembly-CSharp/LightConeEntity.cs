using UnityEngine;

public class LightConeEntity : Entity
{
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
		tk2dSprite componentInChildren = GetComponentInChildren<tk2dSprite>();
		componentInChildren.transform.localScale = Vector3.one * GetParameterFloat("scale");
		componentInChildren.transform.localRotation = Quaternion.Euler(0f, 0f, GetParameterFloat("rotation"));
	}
}

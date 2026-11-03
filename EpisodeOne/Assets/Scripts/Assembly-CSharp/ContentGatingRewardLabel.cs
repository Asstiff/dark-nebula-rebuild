using FLD;
using UnityEngine;

[RequireComponent(typeof(DynamicFont))]
public class ContentGatingRewardLabel : MonoBehaviour
{
	private void Start()
	{
		GetComponent<DynamicFont>().Text = "REWARD AMOUNT".Localize(25);
	}
}

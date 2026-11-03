using UnityEngine;

public class AndroidEvents : MonoBehaviour
{
	public delegate void HandlerDelegate();

	public static event HandlerDelegate BackButtonReleased;

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape) && BackButtonReleased != null)
		{
			BackButtonReleased();
		}
	}
}

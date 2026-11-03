using System.Collections;
using UnityEngine;

public class InvokeRealtimeBehaviour : MonoBehaviour
{
	public string Message;

	public MonoBehaviour TargetScript;

	public void InvokeRealtime(MonoBehaviour pTargetScript, string pMessage, float pSeconds)
	{
		Message = pMessage;
		TargetScript = pTargetScript;
		StopCoroutine("InvokeCoroutine");
		StartCoroutine(InvokeCoroutine(pMessage, pSeconds));
	}

	private IEnumerator InvokeCoroutine(string pMessage, float pSeconds)
	{
		float timeToCall = Time.realtimeSinceStartup + pSeconds;
		while (timeToCall > Time.realtimeSinceStartup)
		{
			yield return 0;
		}
		if (TargetScript != null)
		{
			TargetScript.SendMessage(pMessage);
		}
		Dispose();
	}

	public void CancelInvokeRealtime()
	{
		StopCoroutine("InvokeCoroutine");
		Dispose();
	}

	private void Dispose()
	{
		TargetScript = null;
		Object.Destroy(this);
	}
}

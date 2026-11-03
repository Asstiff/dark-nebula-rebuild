using System;
using UnityEngine;

public static class MonoBehaviourExtensions
{
	public static void InvokeRealtime(this MonoBehaviour pMonoBehaviour, string pMessage, float pSeconds)
	{
		if (pMonoBehaviour == null)
		{
			throw new ArgumentNullException("pMonoBehaviour");
		}
		if (pMessage == null)
		{
			throw new ArgumentNullException("pMessage");
		}
		InvokeRealtimeBehaviour orCreateBehaviour = GetOrCreateBehaviour(pMonoBehaviour, pMessage);
		orCreateBehaviour.InvokeRealtime(pMonoBehaviour, pMessage, pSeconds);
	}

	public static void CancelInvokeRealtime(this MonoBehaviour pMonoBehaviour, string pMessage)
	{
		if (pMonoBehaviour == null)
		{
			throw new ArgumentNullException("pMonoBehaviour");
		}
		if (pMessage == null)
		{
			throw new ArgumentNullException("pMessage");
		}
		InvokeRealtimeBehaviour behaviour = GetBehaviour(pMonoBehaviour, pMessage);
		if (behaviour != null)
		{
			behaviour.CancelInvokeRealtime();
		}
	}

	private static InvokeRealtimeBehaviour GetOrCreateBehaviour(MonoBehaviour pMonoBehaviour, string pMessage)
	{
		InvokeRealtimeBehaviour[] components = pMonoBehaviour.gameObject.GetComponents<InvokeRealtimeBehaviour>();
		InvokeRealtimeBehaviour[] array = components;
		foreach (InvokeRealtimeBehaviour invokeRealtimeBehaviour in array)
		{
			if (invokeRealtimeBehaviour.Message == pMessage && invokeRealtimeBehaviour.TargetScript == pMonoBehaviour)
			{
				return invokeRealtimeBehaviour;
			}
		}
		InvokeRealtimeBehaviour invokeRealtimeBehaviour2 = pMonoBehaviour.gameObject.AddComponent<InvokeRealtimeBehaviour>();
		invokeRealtimeBehaviour2.Message = pMessage;
		return invokeRealtimeBehaviour2;
	}

	private static InvokeRealtimeBehaviour GetBehaviour(MonoBehaviour pMonoBehaviour, string pMessage)
	{
		InvokeRealtimeBehaviour[] components = pMonoBehaviour.GetComponents<InvokeRealtimeBehaviour>();
		InvokeRealtimeBehaviour[] array = components;
		foreach (InvokeRealtimeBehaviour invokeRealtimeBehaviour in array)
		{
			if (invokeRealtimeBehaviour.Message == pMessage && invokeRealtimeBehaviour.TargetScript == pMonoBehaviour)
			{
				return invokeRealtimeBehaviour;
			}
		}
		return null;
	}
}

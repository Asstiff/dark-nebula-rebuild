using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimatedEntityNode : IEntity
{
	public List<string> ParameterNames = new List<string>();

	public List<string> ParameterExpert = new List<string>();

	public List<string> ParameterNormal = new List<string>();

	public List<AnimatedEntityNode> nodes;

	public float x;

	public float y;

	public float time;

	public float easein;

	public float easeout;

	public float pause;

	public Vector2 position
	{
		get
		{
			return new Vector2(x, y);
		}
	}

	public void InitWith(float posX, float posY, float t, float ein, float eout, float p)
	{
		x = posX;
		y = posY;
		time = t;
		easein = ein;
		easeout = eout;
		pause = p;
	}

	public void SetParameter(string key, string normalValue, string expertValue)
	{
		ParameterNames.Add(key);
		ParameterNormal.Add(normalValue);
		ParameterExpert.Add(expertValue);
		string empty = string.Empty;
		empty = ((!UserSettings.I.expert) ? normalValue : expertValue);
		try
		{
			switch (key)
			{
			case "x":
				x = float.Parse(empty);
				break;
			case "y":
				y = float.Parse(empty);
				break;
			case "time":
				time = float.Parse(empty);
				break;
			case "easein":
				easein = float.Parse(empty);
				break;
			case "easeout":
				easeout = float.Parse(empty);
				break;
			case "pause":
				pause = float.Parse(empty);
				break;
			}
		}
		catch
		{
			string message = "AnimatedEntityNode " + key + " (" + empty + ") not a float " + ((!UserSettings.I.expert) ? "normal" : "expert");
			Debug.LogError(message);
			throw new Exception(message);
		}
	}

	public void SetParameterArray(string memberName, List<AnimatedEntityNode> array)
	{
		if (memberName == "nodes")
		{
			nodes = array;
			return;
		}
		Debug.LogWarning("TODO SetParameterArray unknown name: " + memberName + " " + array.Count);
	}

	public void ResetParameters()
	{
	}
}

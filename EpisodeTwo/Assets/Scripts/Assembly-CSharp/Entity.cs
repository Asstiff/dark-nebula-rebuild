using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IEntity, IFreezable
{
	private Dictionary<string, string> Parameters;

	public List<string> ParameterNames = new List<string>();

	public List<string> ParameterExpert = new List<string>();

	public List<string> ParameterNormal = new List<string>();

	public List<AnimatedEntityNode> nodes;

	public bool isCreatedFromLevel;

	[HideInInspector]
	public bool Dead;

	private bool _keepalive;

	private bool _alwaysAlive;

	public bool IsPooled;

	public bool IsFrozen;

	private Transform _transf;

	public bool freezable = true;

	private static readonly Vector2 hibernateDelta = new Vector2(640f, 960f);

	private Collider _cachedCollider;

	private bool hasCollider = true;

	protected EntityFreezeBehaviour _entityFreezeBehaviour;

	protected bool IsBaseInited;

	public bool isInScollingLayer;

	protected bool hasRunPostInit;

	private bool isListeningToDisableKeepAlive;

	public List<Renderer> DisabledRenderers;

	public List<Collider> DisabledColliders;

	private readonly Vector3 BBOffset = new Vector3(64f, 0f, 64f);

	public bool keepalive
	{
		get
		{
			return _keepalive;
		}
		set
		{
			_keepalive = value;
			UpdateFreezeBehaviour();
		}
	}

	protected bool alwaysAlive
	{
		get
		{
			return _alwaysAlive;
		}
		set
		{
			_alwaysAlive = value;
			UpdateFreezeBehaviour();
		}
	}

	public new Transform transform
	{
		get
		{
			if (_transf == null && base.gameObject != null)
			{
				_transf = base.transform;
			}
			return _transf;
		}
	}

	private Collider cachedCollider
	{
		get
		{
			if (hasCollider && _cachedCollider == null)
			{
				_cachedCollider = base.GetComponent<Collider>();
				hasCollider = _cachedCollider != null;
			}
			return _cachedCollider;
		}
	}

	public virtual Dictionary<string, Type> GetAssetsToPreload()
	{
		return new Dictionary<string, Type>();
	}

	protected virtual void Awake()
	{
	}

	public virtual bool PostInit()
	{
		if (hasRunPostInit)
		{
			return true;
		}
		hasRunPostInit = true;
		UpdateFreezeBehaviour();
		return false;
	}

	private void UpdateFreezeBehaviour()
	{
		if (!hasRunPostInit)
		{
			return;
		}
		if (keepalive || alwaysAlive)
		{
			ListenToDisableKeepAlive(true);
			UnityEngine.Object.Destroy(GetComponent<EntityFreezeBehaviour>());
			return;
		}
		ListenToDisableKeepAlive(false);
		if (_entityFreezeBehaviour == null)
		{
			_entityFreezeBehaviour = base.gameObject.GetOrAddComponent<EntityFreezeBehaviour>();
			_entityFreezeBehaviour.Host = this;
			_entityFreezeBehaviour.delayFrames = 3;
			_entityFreezeBehaviour.LateUpdate();
		}
	}

	private void ListenToDisableKeepAlive(bool pListen)
	{
		if (isListeningToDisableKeepAlive != pListen)
		{
			isListeningToDisableKeepAlive = pListen;
			if (isListeningToDisableKeepAlive)
			{
				World.OnDisableAlwaysAliveAndKeepaliveEntities += DisableAlwaysAliveAndKeepaliveEntities;
			}
			else
			{
				World.OnDisableAlwaysAliveAndKeepaliveEntities -= DisableAlwaysAliveAndKeepaliveEntities;
			}
		}
	}

	public void CreateParameters()
	{
		if (!IsBaseInited && isCreatedFromLevel)
		{
			Debug.LogWarning(string.Concat("IsBaseInited = false when trying to CreateParameters! in ", this, " isCreatedFromLevel: ", isCreatedFromLevel));
		}
		Parameters = new Dictionary<string, string>();
		if (UserSettings.I.expert)
		{
			for (int i = 0; i < ParameterNames.Count; i++)
			{
				Parameters[ParameterNames[i]] = ParameterExpert[i];
			}
		}
		else
		{
			for (int j = 0; j < ParameterNames.Count; j++)
			{
				Parameters[ParameterNames[j]] = ParameterNormal[j];
			}
		}
	}

	public void ResetParameters()
	{
		ParameterNames = new List<string>();
		ParameterNormal = new List<string>();
		ParameterExpert = new List<string>();
	}

	public void SetRenderersEnabled(bool enabled)
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = enabled;
		}
	}

	public void SetParameter(string key, string normalValue)
	{
		SetParameter(key, normalValue, normalValue);
	}

	public virtual void SetParameter(string key, string normalValue, string expertValue)
	{
		int num = ParameterNames.IndexOf(key);
		if (num == -1)
		{
			ParameterNames.Add(key);
			ParameterNormal.Add(normalValue);
			ParameterExpert.Add(expertValue);
		}
		else
		{
			ParameterNames[num] = key;
			ParameterNormal[num] = normalValue;
			ParameterExpert[num] = expertValue;
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

	public string GetParameterString(string key)
	{
		if (Parameters == null)
		{
			CreateParameters();
		}
		string value = null;
		if (Parameters.TryGetValue(key, out value))
		{
			return value;
		}
		return string.Empty;
	}

	public float GetParameterFloat(string key)
	{
		if (Parameters == null)
		{
			CreateParameters();
		}
		float result = 0f;
		string value = null;
		if (Parameters.TryGetValue(key, out value) && float.TryParse(value, out result))
		{
			return result;
		}
		return 0f;
	}

	public bool GetParameterBool(string key)
	{
		return GetParameterInt(key) == 1;
	}

	public int GetParameterInt(string key)
	{
		if (Parameters == null)
		{
			CreateParameters();
		}
		int result = 0;
		string value = null;
		if (Parameters.TryGetValue(key, out value) && int.TryParse(value, out result))
		{
			return result;
		}
		return 0;
	}

	public virtual void InitParametersInEditor()
	{
		IsBaseInited = true;
		transform.Translate(GetParameterFloat("x"), 0f, 0f - GetParameterFloat("y"));
	}

	public virtual void InitParametersInRuntime()
	{
		IsBaseInited = true;
		transform.Translate(GetParameterFloat("x"), 0f, 0f - GetParameterFloat("y"));
	}

	protected virtual void OnDestroy()
	{
		hasRunPostInit = false;
		Dead = true;
		ListenToDisableKeepAlive(false);
	}

	public virtual void OnTakenFromPool()
	{
		if (DisabledColliders != null)
		{
			foreach (Collider disabledCollider in DisabledColliders)
			{
				disabledCollider.enabled = true;
			}
		}
		DisabledColliders = null;
		IsPooled = false;
		Dead = false;
	}

	public virtual void OnReturnedToPool()
	{
		DisabledColliders = new List<Collider>();
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			if (collider.enabled)
			{
				collider.enabled = false;
				DisabledColliders.Add(collider);
			}
		}
		IsPooled = true;
		hasRunPostInit = false;
		if (IsFrozen)
		{
			World.FreezeManager.RemoveFromFrozenObjects(this);
		}
		IsFrozen = false;
	}

	public virtual void DisableAlwaysAliveAndKeepaliveEntities()
	{
		if (!IsPooled && (keepalive || alwaysAlive))
		{
			float windowPosZ = World.cameraFollow.windowPosZ;
			Vector2 vector = hibernateDelta;
			float num = windowPosZ - vector.y;
			float num2 = transform.position.z + 64f;
			if (num2 < num)
			{
				RemoveFromWorld();
			}
		}
	}

	public virtual void Defrost()
	{
		base.enabled = true;
		if (_entityFreezeBehaviour != null)
		{
			_entityFreezeBehaviour.enabled = true;
		}
		IsFrozen = false;
		if (DisabledRenderers == null)
		{
			return;
		}
		foreach (Renderer disabledRenderer in DisabledRenderers)
		{
			disabledRenderer.enabled = true;
		}
		DisabledRenderers = null;
	}

	public bool IsOutsideActiveView()
	{
		Vector3 vector = transform.position;
		Vector3 vector2 = transform.position;
		if (cachedCollider == null)
		{
			vector -= BBOffset;
			vector2 += BBOffset;
		}
		else
		{
			vector = cachedCollider.bounds.min;
			vector2 = cachedCollider.bounds.max;
		}
		float windowPosZ = World.cameraFollow.windowPosZ;
		Vector2 vector3 = hibernateDelta;
		vector3.y=Mathf.Max(vector3.y,World.VisibleHalfSize.y+240f);
		if (windowPosZ + vector3.y < vector.z)
		{
			return true;
		}
		Vector2 vector4 = hibernateDelta;
		vector4.y=Mathf.Max(vector4.y,World.VisibleHalfSize.y+240f);
		if (windowPosZ - vector4.y > vector2.z)
		{
			return true;
		}
		float windowPosX = World.cameraFollow.windowPosX;
		Vector2 vector5 = hibernateDelta;
		if (windowPosX - vector5.x > vector2.x)
		{
			return true;
		}
		Vector2 vector6 = hibernateDelta;
		if (windowPosX + vector6.x < vector.x)
		{
			return true;
		}
		return false;
	}

	public virtual void RemoveFromWorld()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDrawGizmos()
	{
		if (freezable)
		{
			Gizmos.color = Color.cyan;
			if (IsFrozen)
			{
				Gizmos.DrawWireCube(transform.position, Vector3.one * 64f);
				Gizmos.DrawLine(transform.position + new Vector3(-32f, 0f, -32f), transform.position + new Vector3(32f, 0f, 32f));
				Gizmos.DrawLine(transform.position + new Vector3(-32f, 0f, 32f), transform.position + new Vector3(32f, 0f, -32f));
			}
			else
			{
				Gizmos.DrawWireCube(transform.position, Vector3.one * 64f);
			}
		}
	}
}

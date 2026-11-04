using UnityEngine;
using System.Collections.Generic;

public class ParallaxLayer : MonoBehaviour
{
	public int layerIndex;

	public float scrollValue = 1f;

	public int MapWidth;

	public int MapHeight;

	public bool Render = true;

	public int layerWidth;

	private CameraFollow cameraFollow;

	private static float VirtualScreenHeight = 480f;

	private static float VirtualScreenHeightHalf = 240f;

	private float _height;

	private Material sharedMaterial;
    readonly Dictionary<Material,Material> ownedMaterials = new Dictionary<Material,Material>();
    readonly Dictionary<Material,Material> overlayExtensions = new Dictionary<Material,Material>();
    bool hasAlphaOverride;
    public Material TileMaterial(Material original, bool overlayExtension = false)
    {
        var materials = overlayExtension ? overlayExtensions : ownedMaterials;
        Material copy;
        if (!materials.TryGetValue(original,out copy))
        {
            copy=new Material(original);copy.name=original.name+" ("+name+")";
            if (overlayExtension) copy.shader=Resources.Load<Shader>("PortalOverlayExtension");
            if (hasAlphaOverride && copy.HasProperty("_Color")) copy.color=new Color(1f,1f,1f,currentAlpha);
            materials.Add(original,copy);
        }
        return copy;
    }
    void OnDestroy()
    {
        World.RemoveLayer(this);
        foreach (var material in ownedMaterials.Values) if(material!=null)Destroy(material);
        foreach (var material in overlayExtensions.Values) if(material!=null)Destroy(material);
    }

	private float currentAlpha = 1f;

	private bool blend;

	private Vector3 pos;

	private float winZ;

	private float z;

	private Transform _transform;

	private float _distortion;

	public bool isNoScrollLayer;

	public bool Enabled
	{
		set
		{
			SetAlpha((!value) ? 0f : 1f);
		}
	}

	public float Distortion
	{
		get
		{
			return _distortion;
		}
		set
		{
			_distortion = value;
		}
	}

	private void Start()
	{
		if (scrollValue == 1f)
		{
			isNoScrollLayer = true;
		}
		_transform = base.transform;
		World.AddLayer(this);
		if (!isNoScrollLayer)
		{
			_height = (float)(-World.mapHeight) * 64f - 480f;
			cameraFollow = World.cameraFollow;
			pos = _transform.position;
		}
	}

	private void OnDisable()
	{
		cameraFollow = null;
	}

	private void LateUpdate()
	{
		if (isNoScrollLayer || Time.timeScale == 0f)
		{
			return;
		}
		float num = (float)(1000 - MapHeight) * 64f;
		num = (1f - scrollValue) * num;
		if (cameraFollow == null)
		{
			cameraFollow = World.cameraFollow;
			if (cameraFollow == null)
			{
				return;
			}
		}
		winZ = cameraFollow.windowPosZ;
		z = (_height - winZ) * scrollValue;
		z = _height - z;
		pos.z = winZ - z - VirtualScreenHeightHalf - (0.5f - scrollValue) * VirtualScreenHeight - num;
		pos.x = cameraFollow.windowPosX * (1f - scrollValue) - (float)layerWidth * 32f;
		_transform.position = pos;
	}

	public void SetAlpha(float value)
	{
        if (value < 0f) return; // -1 means use the material's authored opacity.
		if (currentAlpha == value)
		{
			return;
		}
		currentAlpha = value;
        hasAlphaOverride = true;
		if (sharedMaterial == null)
		{
			CombineChildren componentInChildren = GetComponentInChildren<CombineChildren>();
			if (componentInChildren != null && componentInChildren.GetComponent<Renderer>() != null)
			{
				sharedMaterial = componentInChildren.GetComponent<Renderer>().sharedMaterial;
			}
		}
		if (sharedMaterial != null && sharedMaterial.HasProperty("_Color"))
		{
			sharedMaterial.color = new Color(1f, 1f, 1f, currentAlpha);
		}
        foreach (var material in ownedMaterials.Values)
            if (material != null && material.HasProperty("_Color")) material.color=new Color(1f,1f,1f,currentAlpha);
        foreach (var material in overlayExtensions.Values)
            if (material != null && material.HasProperty("_Color")) material.color=new Color(1f,1f,1f,currentAlpha);
	}

	public void SetBlend(bool pBlend)
	{
		if (blend != pBlend)
		{
		}
	}
}

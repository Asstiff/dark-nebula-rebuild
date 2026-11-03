using System.Collections.Generic;
using UnityEngine;

// The temple exit and its respawn are separate authored views. A taller camera
// must not turn their unused separation into a continuous piece of level.
[DefaultExecutionOrder(450)]
public sealed class PortalRegionVisibility : MonoBehaviour
{
    sealed class TileView { public Renderer renderer; public ParallaxLayer layer; }
    readonly List<TileView> tiles = new List<TileView>();
    readonly List<Renderer> entities = new List<Renderer>();
    MaterialPropertyBlock properties;
    static readonly int Region = Shader.PropertyToID("_PortalRegion");
    float sourceZ, destinationZ, direction, forestMin, forestMax;
    bool ready, arrived;
    Vector4 lastTileState = new Vector4(float.NaN,0,0,0);
    Mesh backingMesh;
    Material backingMaterial;
    Renderer backingRenderer;
    Material extensionMaterial;
    Renderer extensionRenderer;
    public bool Arrived { get { return arrived; } }
    public float SourceZ { get { return sourceZ; } }
    public float DestinationZ { get { return destinationZ; } }

    void Start()
    {
        properties = new MaterialPropertyBlock();
        MagnetVesselSlotEntity source = null;
        VesselRespawn destination = null;
        foreach (var entity in GetComponentsInChildren<MagnetVesselSlotEntity>())
            if (entity.GetParameterInt("flag") == 104) source = entity;
        foreach (var entity in GetComponentsInChildren<VesselRespawn>())
            if (entity.GetParameterInt("activateEventFlag") == 105) destination = entity;
        // Tile layers are not necessarily children of World in recovered scenes.
        if (source == null) foreach (var entity in Object.FindObjectsByType<MagnetVesselSlotEntity>())
            if (entity.gameObject.scene == gameObject.scene && entity.GetParameterInt("flag") == 104) source = entity;
        if (destination == null) foreach (var entity in Object.FindObjectsByType<VesselRespawn>())
            if (entity.gameObject.scene == gameObject.scene && entity.GetParameterInt("activateEventFlag") == 105) destination = entity;
        if (source == null || destination == null) { enabled = false; return; }
        sourceZ = source.transform.position.z; destinationZ = destination.transform.position.z;
        direction = Mathf.Sign(destinationZ - sourceZ);
        if (Mathf.Abs(destinationZ-sourceZ) <= 480f) { enabled = false; return; }
        forestMin = float.PositiveInfinity; forestMax = float.NegativeInfinity;
        foreach (var layer in Object.FindObjectsByType<ParallaxLayer>())
        {
            if (layer.gameObject.scene != gameObject.scene) continue;
            foreach (var renderer in layer.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer.GetComponentInParent<Entity>() != null)
                { if(renderer.GetComponentInParent<Vessel>() == null)entities.Add(renderer); continue; }
                if (renderer.GetComponentInParent<CombineChildren>() == null) continue;
                tiles.Add(new TileView { renderer = renderer, layer = layer });
                if (layer.name == "aztek_floor")
                { forestMin = Mathf.Min(forestMin,renderer.bounds.min.z); forestMax = Mathf.Max(forestMax,renderer.bounds.max.z); }
            }
        }
        ready = true;
        CreateNeutralBacking();
        Debug.Log("PORTAL_REGIONS source="+sourceZ+" destination="+destinationZ+" tiles="+tiles.Count+" entities="+entities.Count);
    }

    void CreateNeutralBacking()
    {
        if(direction<0 || float.IsInfinity(forestMax))return;
        Transform asset=LevelAssets.GetAsset("tileset_matrix_floor");
        if(asset==null)return;
        var sprite=asset.GetComponent<tk2dSprite>();
        if(sprite==null || sprite.collection.spriteDefinitions.Length<2)return;
        // Tile 1 is the existing plain grid. No portal, route or scenery is
        // copied into this background, and it has no collision or entities.
        var tile=sprite.collection.spriteDefinitions[1];
        var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
        Quaternion rotation=Quaternion.LookRotation(Vector3.down);
        int low=Mathf.FloorToInt((sourceZ-2048f)/64f),high=Mathf.CeilToInt((destinationZ+1536f)/64f);
        for(int x=-16;x<=16;x++)for(int z=low;z<=high;z++)
        {
            int first=vertices.Count;
            for(int i=0;i<tile.positions.Length;i++)
            {vertices.Add(rotation*tile.positions[i]+new Vector3(x*64f,20f,z*64f+32f));uv.Add(tile.uvs[i]);}
            foreach(int index in tile.indices)triangles.Add(first+index);
        }
        backingMesh=new Mesh();backingMesh.name="Neutral portal background";
        backingMesh.SetVertices(vertices);backingMesh.SetUVs(0,uv);backingMesh.SetTriangles(triangles,0);backingMesh.RecalculateBounds();
        var backing=new GameObject("Neutral portal background");backing.transform.SetParent(transform,true);
        backing.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);backing.AddComponent<MeshFilter>().sharedMesh=backingMesh;
        backingMaterial=new Material(tile.material);backingMaterial.shader=Resources.Load<Shader>("PortalNeutralBacking");
        backingMaterial.renderQueue=2501;
        backingRenderer=backing.AddComponent<MeshRenderer>();backingRenderer.sharedMaterial=backingMaterial;backingRenderer.enabled=false;
        var extension = new GameObject("Portal extension blend");
        extension.transform.SetParent(backing.transform, false);
        extension.AddComponent<MeshFilter>().sharedMesh = backingMesh;
        extensionMaterial = new Material(Resources.Load<Shader>("PortalExtensionBlend"));
        extensionMaterial.mainTexture = tile.material.mainTexture;
        extensionRenderer = extension.AddComponent<MeshRenderer>();
        extensionRenderer.sharedMaterial = extensionMaterial;
        extensionRenderer.enabled = false;
    }

    public void RespawnAt(VesselRespawn respawn)
    {
        if (respawn.GetParameterInt("activateEventFlag") == 105) arrived = true;
    }

    void LateUpdate()
    {
        if (!ready || World.cameraFollow == null) return;
        Camera camera = World.cameraFollow.GetComponent<Camera>();
        float band = Mathf.Min(1f,camera.aspect / LegacyScreenFit.ReferenceAspect);
        float minimum = -100000000f, maximum = 100000000f;
        float edge = arrived ? destinationZ-direction*240f : sourceZ+direction*240f;
        if ((arrived && direction > 0) || (!arrived && direction < 0)) minimum = edge;
        else maximum = edge;
        float cameraZ = camera.transform.position.z;
        if (!arrived && direction*(cameraZ-sourceZ)>-240f)
        {
            if(direction>0)minimum=sourceZ-240f;else maximum=sourceZ+240f;
        }
        // The old forest is not a continuation of the Matrix's extra bands.
        // Fade only the extension as the forest leaves the original view.
        float forestDistance = direction>0 ? cameraZ-forestMax : forestMin-cameraZ;
        float forestAlpha = arrived ? 0f : Mathf.Clamp01((240f-forestDistance)/64f);
        if(backingRenderer!=null)
        {
            backingMaterial.SetFloat("_PortalBand",band);
            // The overlay is exclusive to the portal. Fade it back out once the
            // original entry is beyond even the extended destination view.
            float nearPortal = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(forestMax-96f, forestMax+128f, cameraZ));
            float afterPortal = 1f-Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(destinationZ, destinationZ+512f, cameraZ));
            backingMaterial.SetFloat("_ForestEnd",forestMax);
            backingMaterial.SetFloat("_PortalAlpha",nearPortal);
            backingRenderer.enabled = band < .999f && nearPortal > 0f;
            extensionMaterial.SetFloat("_PortalBand",band);
            extensionMaterial.SetFloat("_ExtensionBlend",nearPortal*afterPortal);
            extensionMaterial.SetFloat("_Arrived",arrived ? 1f : 0f);
            extensionRenderer.enabled = band < .999f && nearPortal*afterPortal > 0f;
        }
        Vector4 tileState=new Vector4(band,forestAlpha,arrived?1:0,direction*(cameraZ-sourceZ)>-240f?1:0);
        if (tileState != lastTileState)
        {
        foreach (var tile in tiles)
        {
            if (tile.renderer == null) continue;
            Vector4 region = new Vector4(-100000000f,100000000f,1f,band);
            if (tile.layer.name.StartsWith("aztek_")) region.z = forestAlpha;
            else if (tile.layer.scrollValue == 1f &&
                !(tile.layer.name == "matrix_overlay" && !arrived) &&
                !(tile.layer.name == "matrix_floor" && arrived))
            { region.x = minimum; region.y = maximum; }
            // Neutral grid/cloud backing can fill the new bands. It does not
            // reveal a path, a portal, vegetation, or another playable area.
            tile.renderer.GetPropertyBlock(properties);
            properties.SetVector(Region,region);tile.renderer.SetPropertyBlock(properties);
        }
        lastTileState=tileState;
        }
        foreach (var renderer in entities)
        {
            if (renderer == null) continue;
            Bounds bounds = renderer.bounds;
            bool otherRegion = bounds.max.z < minimum || bounds.min.z > maximum;
            bool originalView = false;
            if (otherRegion && band < 0.999f)
            {
                float low = float.PositiveInfinity, high = float.NegativeInfinity;
                for (int x=-1;x<=1;x+=2) for (int y=-1;y<=1;y+=2) for (int z=-1;z<=1;z+=2)
                {
                    Vector3 point = camera.WorldToViewportPoint(bounds.center+Vector3.Scale(bounds.extents,new Vector3(x,y,z)));
                    low=Mathf.Min(low,point.y);high=Mathf.Max(high,point.y);
                }
                originalView = high >= .5f-.5f*band && low <= .5f+.5f*band;
            }
            renderer.forceRenderingOff = band < .999f && otherRegion && !originalView;
        }
    }
    void OnDestroy()
    {
        foreach (var renderer in entities) if (renderer != null) renderer.forceRenderingOff = false;
        if(extensionMaterial!=null)Destroy(extensionMaterial);
        if(backingMesh!=null)Destroy(backingMesh);if(backingMaterial!=null)Destroy(backingMaterial);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The forest and Matrix portal shots are separate environments. The original
// narrow view hides their overlap. Switch only scenery at the native whiteout;
// leave the authored background underneath, gameplay and all colliders intact.
public sealed class PortalEnvironment : MonoBehaviour
{
    readonly List<Renderer> forest = new List<Renderer>();
    FlagConditionChanged arrival;
    bool arrived;
    bool hidden;
    bool approaching;
    bool destinationHidden;
    FlagConditionChanged forestTrigger;
    readonly List<Renderer> destination = new List<Renderer>();
    readonly List<Renderer> portalCaps = new List<Renderer>();

    IEnumerator Start()
    {
        arrival = new FlagConditionChanged(105);
        arrival.Callback += Arrive;
        forestTrigger = new FlagConditionChanged(60);
        forestTrigger.Callback += Approach;
        yield return null; // Tile batches combine in Start.
        foreach (ParallaxLayer layer in Object.FindObjectsByType<ParallaxLayer>())
        {
            if (layer.gameObject.scene != gameObject.scene || !(layer.name.StartsWith("aztek_") || layer.name == "matrix_overlay" || layer.name == "matrix_floor" || layer.name.StartsWith("matrix_dof"))) continue;
            foreach (Renderer renderer in layer.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer.GetComponentInParent<CombineChildren>() == null) continue;
                if (renderer.GetComponentInParent<Entity>() != null) continue;
                var batch = renderer.GetComponentInParent<CombineChildren>();
                if (batch.name.StartsWith("ForestPortalCap_")) portalCaps.Add(renderer);
                if (batch.name.StartsWith("MatrixSide_") || layer.name == "matrix_floor" || layer.name.StartsWith("matrix_dof")) destination.Add(renderer);
                else forest.Add(renderer);
            }
        }
        Debug.Log("PORTAL_ENVIRONMENT forestBatches=" + forest.Count);
    }

    void Approach() { approaching = true; }
    void Arrive() { arrived = true; }

    void LateUpdate()
    {
        if (World.cameraFollow == null) return;
        Camera camera = World.cameraFollow.GetComponent<Camera>();
        bool hide = arrived && camera.aspect < LegacyScreenFit.ReferenceAspect - .001f;
        bool before = approaching && !arrived && camera.aspect < LegacyScreenFit.ReferenceAspect - .001f;
        if (before != destinationHidden) {
            destinationHidden = before;
            foreach (Renderer r in portalCaps) if (r != null) r.forceRenderingOff = before;
            foreach (Renderer r in destination) if (r != null) r.forceRenderingOff = before;
        }
        if (hidden == hide) return;
        hidden = hide;
        foreach (Renderer renderer in forest)
            if (renderer != null) renderer.forceRenderingOff = hide;
    }

    void OnDestroy()
    {
        if (arrival != null) arrival.Dispose();
        if (forestTrigger != null) forestTrigger.Dispose();
        foreach (Renderer r in destination) if (r != null) r.forceRenderingOff = false;
        foreach (Renderer r in portalCaps) if (r != null) r.forceRenderingOff = false;
        foreach (Renderer renderer in forest)
            if (renderer != null) renderer.forceRenderingOff = false;
    }
}

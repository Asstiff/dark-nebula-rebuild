using UnityEngine;

// CameraFollow and ParallaxLayer update in LateUpdate at the default order.
// Keep its waiting pose out of the expanded view, then start the original curve
// after both have updated. The old waiting pose can be visible on a tall phone.
[DefaultExecutionOrder(1000)]
public sealed class TallScanEntry : MonoBehaviour
{
    AnimatedEntity scan;
    Renderer[] renderers;

    public void HideUntilTriggered(AnimatedEntity target)
    {
        scan = target;
        if (renderers == null) renderers = target.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
            if (renderer != null) renderer.forceRenderingOff = true;
        enabled = false;
    }

    public void Schedule(AnimatedEntity target)
    {
        HideUntilTriggered(target);
        enabled = true;
    }

    public void Reveal()
    {
        if (renderers != null)
            foreach (Renderer renderer in renderers)
                if (renderer != null) renderer.forceRenderingOff = false;
        scan = null;
        enabled = false;
    }

    void LateUpdate()
    {
        if (scan != null) scan.StartTallScan();
        Reveal();
    }
}

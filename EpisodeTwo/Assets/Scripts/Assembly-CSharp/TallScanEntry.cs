using UnityEngine;

// CameraFollow and ParallaxLayer update in LateUpdate at the default order.
// Start the original scan curve after both have updated, including on Android.
[DefaultExecutionOrder(1000)]
public sealed class TallScanEntry : MonoBehaviour
{
    AnimatedEntity scan;

    public void Schedule(AnimatedEntity target)
    {
        scan = target;
        enabled = true;
    }

    void LateUpdate()
    {
        if (scan != null) scan.StartTallScan();
        scan = null;
        enabled = false;
    }
}

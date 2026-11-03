using UnityEngine;

internal static class RuntimeFrameRate
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Configure()
    {
        Application.targetFrameRate = 60;
    }
}

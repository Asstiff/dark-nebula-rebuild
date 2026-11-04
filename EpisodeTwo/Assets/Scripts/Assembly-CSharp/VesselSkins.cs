using UnityEngine;

public static class VesselSkins
{
    static readonly string[] Assets = { "Default", "Cyborg", "Halcyon", "Trojan", "Palringo" };
    public static int Selected
    {
        get
        {
            int value = Mathf.Clamp(PlayerPrefs.GetInt("Gameplay.Vessel", 0), 0, 4);
            return AvatarUnlocks.IsUnlocked(AvatarUnlocks.Ids[value]) ? value : 0;
        }
        set
        {
            if (value < 0 || value >= Assets.Length || !AvatarUnlocks.IsUnlocked(AvatarUnlocks.Ids[value])) return;
            PlayerPrefs.SetInt("Gameplay.Vessel", value); PlayerPrefs.Save();
        }
    }
    public static tk2dSpriteCollectionData Collection(int index)
    {
        GameObject asset = Resources.Load<GameObject>("VesselSkins/Avatar" + Assets[index] + "Collection");
        return asset != null ? asset.GetComponent<tk2dSpriteCollectionData>() : null;
    }
    public static void Apply(tk2dAnimatedSprite sprite, bool energy)
    {
        if (sprite == null || sprite.anim == null) return;
        VesselSkinBinding binding = sprite.GetComponent<VesselSkinBinding>();
        if (binding == null)
        {
            binding = sprite.gameObject.AddComponent<VesselSkinBinding>();
            binding.originalAnimation = sprite.anim;
            binding.originalCollection = sprite.collection;
            binding.originalSprite = sprite.spriteId;
        }
        sprite.anim = binding.originalAnimation;
        sprite.collection = binding.originalCollection;
        sprite.spriteId = binding.originalSprite;
        sprite.Build();
        if (binding.instance != null) Object.Destroy(binding.instance.gameObject);
        if (Selected == 0) return;
        tk2dSpriteCollectionData collection = Collection(Selected);
        if (collection == null) return;
        // Each player owns a library copy; selecting a skin never mutates the shared source asset.
        tk2dSpriteAnimation library = Object.Instantiate(sprite.anim, sprite.transform);
        sprite.anim = library;
        binding.instance = library;
        foreach (tk2dSpriteAnimationClip clip in library.clips)
        {
            int fixedFrame = clip.name == "VesselExpert" ? 30 : clip.name == "VesselMatrix" ? 29 : 24;
            if (energy) fixedFrame = clip.name == "EnergyRed" ? 26 : clip.name == "EnergyGreen" ? 28 : 27;
            for (int i = 0; i < clip.frames.Length; i++)
            {
                clip.frames[i].spriteCollection = collection;
                clip.frames[i].spriteId = clip.name == "VesselJump" ? Mathf.Min(i, 23) : fixedFrame;
            }
        }
        sprite.collection = collection;
        sprite.spriteId = energy ? 27 : 24;
        sprite.Build();
    }
}

public sealed class VesselSkinBinding : MonoBehaviour
{
    public tk2dSpriteAnimation originalAnimation, instance;
    public tk2dSpriteCollectionData originalCollection;
    public int originalSprite;
}

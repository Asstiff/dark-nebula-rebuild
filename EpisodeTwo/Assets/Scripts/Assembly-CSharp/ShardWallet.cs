using UnityEngine;

// Episode One's free shard economy, stored independently for Episode Two.
public static class ShardWallet
{
    const string BalanceKey = "Gameplay.Shards";
    const string ChanceKey = "Gameplay.ShardSpawnChance";
    public static int Balance
    {
        get
        {
            if (!PlayerPrefs.HasKey(BalanceKey)) { PlayerPrefs.SetInt(BalanceKey,10); PlayerPrefs.Save(); }
            return PlayerPrefs.GetInt(BalanceKey);
        }
    }
    public static bool Spend(int amount)
    {
        if (amount<=0 || Balance<amount) return false;
        PlayerPrefs.SetInt(BalanceKey,Balance-amount); PlayerPrefs.Save(); return true;
    }
    public static void Earn(int amount)
    {
        if(amount<=0) return;
        PlayerPrefs.SetInt(BalanceKey,(int)System.Math.Min(int.MaxValue,(long)Balance+amount)); PlayerPrefs.Save();
    }
    public static void SpawnInLevel()
    {
        // Preserve original probability and one collectible per level entry.
        if(Random.Range(1,101)>=PlayerPrefs.GetInt(ChanceKey,40)) return;
        LifeOrb[] orbs=Object.FindObjectsByType<LifeOrb>(FindObjectsInactive.Include,FindObjectsSortMode.None);
        if(orbs.Length>0) orbs[Random.Range(0,orbs.Length)].MarkShard();
    }
    public static void Collect()
    {
        int chance=PlayerPrefs.GetInt(ChanceKey,40);
        if(chance-1>10) PlayerPrefs.SetInt(ChanceKey,chance-1);
        Earn(1);
        GameHaptics.Play(HapticEvent.CriticalPickup);
        if(World.hud!=null) World.hud.ShowMessage(GameLocalization.Format("SHARD PRICE",1)+"  +1");
    }
}

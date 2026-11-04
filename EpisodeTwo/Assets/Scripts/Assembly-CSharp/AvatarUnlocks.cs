using System;
using UnityEngine;

public static class AvatarUnlocks
{
    // Stable indices are also the saved skin and art indices. Sort only presentation.
    public static readonly string[] Ids = { "DEFAULT_AVATAR", "CYBORG_AVATAR", "HALCYON_AVATAR", "TROJAN_AVATAR", "PALRINGO_AVATAR" };
    public static readonly int[] Prices = { 0, 50, 600, 1000, 300 };
    public static int IndexOf(string id) { return Array.IndexOf(Ids, id); }
    public static int Price(string id) { int i=IndexOf(id); return i<0 ? int.MaxValue : Prices[i]; }
    public static bool IsUnlocked(string id) { return id==Ids[0] || IndexOf(id)>0 && PlayerPrefs.GetInt("Gameplay.Owned."+id,0)==1; }
    public static int Balance { get { return ShardWallet.Balance; } }
    public static string Condition(string id) { return GameLocalization.Format("SHARD PRICE",Price(id)) + "\n" + GameLocalization.Format("SHARD BALANCE",Balance); }
    public static bool TryPurchase(string id)
    {
        if(IndexOf(id)<0 || IsUnlocked(id)) return false;
        if(!ShardWallet.Spend(Price(id))) return false;
        PlayerPrefs.SetInt("Gameplay.Owned."+id,1);
        PlayerPrefs.Save();
        return true;
    }
    public static int[] DisplayOrder()
    {
        int[] order={0,1,2,3,4};
        Array.Sort(order,(a,b)=> { int owned=IsUnlocked(Ids[b]).CompareTo(IsUnlocked(Ids[a])); return owned!=0 ? owned : Prices[a].CompareTo(Prices[b]); });
        return order;
    }
}

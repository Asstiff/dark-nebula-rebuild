using FLD;

// The offline edition has no ad provider, cache, network request or ad wait.
public class DarkNebulaAdsManager
{
    static readonly DarkNebulaAdsManager instance=new DarkNebulaAdsManager();
    public static DarkNebulaAdsManager Instance {get{return instance;}}
    public void InitInterstitialAdsService() {}
    public bool HasCachedInterstitial(string location) {return false;}
    public Async<string> ShowAd(string location) {return Async<string>.MakePrecompleted("Disabled");}
}

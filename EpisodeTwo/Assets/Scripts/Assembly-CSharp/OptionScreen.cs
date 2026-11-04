using UnityEngine;
public class OptionScreen : IngameOptionScreen
{
    public GameObject FlipperDifficulty;
    public GameObject BtnFacebookConnect;
    protected override void Start()
    {
        if(BtnFacebookConnect!=null)BtnFacebookConnect.SetActive(false);
        // These authored notes were parked outside the original panel. They must not
        // enter the safe-area fitter or leak below the extended settings list.
        foreach(string name in new[]{"Text0","Text1","Text2"})
        {
            Transform note=transform.Find(name);
            if(note!=null)note.gameObject.SetActive(false);
        }
        base.Start();
        FlipperDifficulty.GetComponent<FlipperButton>().EventValueChanged += value=>UserSettings.I.SetExpert(value);
        RefreshSettings();
    }
    protected override void RefreshSettings()
    {
        base.RefreshSettings();
        if(!IsInitialized)return;
        if(FlipperDifficulty!=null)FlipperDifficulty.GetComponent<FlipperButton>().Init(UserSettings.I.expert,"EXPERT","NORMAL","DIFFICULTY");
    }
}

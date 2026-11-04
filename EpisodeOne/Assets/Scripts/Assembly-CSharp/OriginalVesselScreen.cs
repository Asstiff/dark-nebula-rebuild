using System;
using UnityEngine;

// Extends the recovered settings controls and vessel artwork; no replacement UI skin.
public sealed class OriginalVesselScreen : MonoBehaviour
{
    IngameOptionScreen owner;
    FlipperButton selector;
    SimpleButton action, back;
    DynamicFont details;
    tk2dSprite preview;
    int[] order;
    int selected;
    bool closing;

    public static void Open(IngameOptionScreen source)
    {
        var root=new GameObject("Vessel selection");
        root.layer=source.gameObject.layer;
        root.transform.SetParent(source.transform.parent,false);
        root.transform.localPosition=source.transform.localPosition;
        root.transform.localRotation=source.transform.localRotation;
        root.transform.localScale=source.transform.localScale;
        var page=root.AddComponent<OriginalVesselScreen>();
        page.Initialize(source);
        source.gameObject.SetActive(false);
    }
    GameObject Copy(GameObject template,string name,float y)
    {
        GameObject copy=Instantiate(template,transform);
        copy.name=name; copy.SetActive(true);
        copy.transform.localPosition=new Vector3(0,y,template.transform.localPosition.z);
        foreach(SimpleButton button in copy.GetComponentsInChildren<SimpleButton>(true))
        { button.targetObject=null;button.messageName=string.Empty;button.enabled=true; }
        return copy;
    }
    void Initialize(IngameOptionScreen source)
    {
        owner=source;
        Transform sourceBackground=source.transform.Find("Background");
        if(sourceBackground!=null)
        {
            GameObject background=Instantiate(sourceBackground.gameObject,transform);
            background.name="Background";
            Vector3 position=background.transform.localPosition;
            position.z=10f;
            background.transform.localPosition=position;
            background.SetActive(true);
        }
        selected=AvatarUnlocks.IndexOf(AvatarManager.Instance.ActiveAvatar.ID);
        selector=Copy(source.FlipperMusic,"Vessel selector",150).GetComponent<FlipperButton>();
        selector.EventChoiceChanged+=Choose;
        action=Copy(source.BtnCalibrateControls,"Purchase or equip",-145).GetComponent<SimpleButton>();
        back=Copy(source.BtnGoBack,"Back",-215).GetComponent<SimpleButton>();
        action.ButtonUpEvent+=PurchaseOrEquip;
        back.ButtonUpEvent+=Return;
        details=Instantiate(selector.Value,transform);
        details.transform.SetParent(transform,false);
        details.SetOriginalPos(new Vector3(0,-85,0));
        details.WrapWidth=280;details.Wrapping=DynamicFont.WrapType.SHRINK;
        var art=new GameObject("Vessel preview");art.layer=source.gameObject.layer;
        art.transform.SetParent(transform,false);art.transform.localPosition=new Vector3(0,35,0);
        preview=tk2dSprite.AddComponent(art,AvatarManager.Instance.AllAvatars.Find(a=>a.ID==AvatarUnlocks.Ids[selected]).SpriteCollection,24);
        var fit=gameObject.AddComponent<MenuContentFit>();fit.Camera=back.viewCamera!=null?back.viewCamera:Camera.main;
        GamePreferences.Changed+=Refresh;
        Refresh();
    }
    void Choose(int position) { selected=order[position];RefreshDetails(); }
    public void Browse(int delta)
    {
        int position=Array.IndexOf(order,selected);
        selected=order[(position+delta+order.Length)%order.Length];Refresh();
    }
    void Refresh()
    {
        order=AvatarUnlocks.DisplayOrder();
        string[] names=new string[order.Length];
        for(int i=0;i<names.Length;i++)names[i]=AvatarUnlocks.Ids[order[i]];
        selector.InitChoices(Array.IndexOf(order,selected),names,"SELECT VESSEL");
        back.Text=GameLocalization.Text("GO BACK");
        RefreshDetails();
    }
    void RefreshDetails()
    {
        string id=AvatarUnlocks.Ids[selected];bool owned=AvatarUnlocks.IsUnlocked(id);
        string info=owned?GameLocalization.Format("SHARD BALANCE",AvatarUnlocks.Balance):AvatarUnlocks.Condition(id);
        details.Text=info;
        string caption=!owned?"BUY":selected==AvatarUnlocks.IndexOf(AvatarManager.Instance.ActiveAvatar.ID)?"EQUIPPED":"EQUIP";
        action.Text=GameLocalization.Text(caption);
        action.enabled=owned || AvatarUnlocks.Balance>=AvatarUnlocks.Price(id);
        tk2dSpriteCollectionData collection=AvatarManager.Instance.AllAvatars.Find(a=>a.ID==id).SpriteCollection;
        preview.SetSprite(collection,24);
        preview.color=owned?Color.white:Color.black;
        float size=collection.spriteDefinitions[24].boundsData[1].x;
        preview.transform.localScale=Vector3.one*(110f/Mathf.Max(.01f,size));
    }
    void PurchaseOrEquip(SimpleButton button)
    {
        string id=AvatarUnlocks.Ids[selected];
        if(!AvatarUnlocks.IsUnlocked(id) && !AvatarUnlocks.TryPurchase(id))return;
        AvatarManager.Instance.ActiveAvatar=AvatarManager.Instance.AllAvatars.Find(a=>a.ID==id);
        if(World.vessel!=null)World.vessel.SetAvatar();
        Refresh();
    }
    void Return(SimpleButton button) { Close(); }
    public void Close()
    {
        if(closing)return;closing=true;
        if(owner!=null){owner.gameObject.SetActive(true);owner.SetEnabled(true);}
        Destroy(gameObject);
    }
    void Update() { if(Input.GetKeyUp(KeyCode.Escape))Close(); }
    void OnDestroy() { GamePreferences.Changed-=Refresh; }
}

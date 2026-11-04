using UnityEngine;

// Recovered label offsets include the old font's line-box padding. Center the
// visible glyphs in their original button art after each language/value refresh.
public static class LegacyControlTextLayout
{
    public static void Bind(DynamicFont label, Renderer art)
    {
        if(label==null || art==null)return;
        System.Action align=()=>
        {
            if(label==null || art==null)return;
            Vector3 position=label.transform.position;
            position.y+=art.bounds.center.y-label.InkCenterWorldY;
            label.transform.position=position;
        };
        label.FontRefreshed+=align;
        align();
    }
}

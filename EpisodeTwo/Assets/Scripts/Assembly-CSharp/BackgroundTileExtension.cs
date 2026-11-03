using System.Collections.Generic;
using UnityEngine;

// Extend only the abstract Matrix backing. Forest structures and scenery stay
// at their authored boundaries; they must not preview six rows early.
public static class BackgroundTileExtension
{
    public struct Tile
    {
        public int x,y;
        public ushort id;
        public Tile(int x,int y,ushort id){this.x=x;this.y=y;this.id=id;}
    }
    public static IEnumerable<Tile> Tiles(TileMap map,string layer,TileMap forest=null)
    {
        bool backing=layer=="matrix_floor" || layer=="matrix_dof01";
        if(!backing)yield break;
        int first=-1,last=-1;
        for(int y=0;y<map.Height;y++)
        {
            int filled=0;for(ushort x=0;x<map.Width;x++)if(map.GetTile(x,(ushort)y)!=TileMap.Empty)filled++;
            if(filled<Mathf.Max(1,Mathf.CeilToInt(map.Width*0.6f)))continue;
            if(first<0)first=y;last=y;
        }
        if(first<0 || last-first<8)yield break;
        int period=layer=="matrix_dof01"?8:1;
        int after=6;
        // Forest and Matrix are separate portal regions, not a gap to bridge.
        // Never pull the next region's floor into the preceding original view.
        if(layer=="matrix_floor" && forest!=null)after=0;
        for(int pad=1;pad<=Mathf.Max(6,after);pad++)for(ushort x=0;x<map.Width;x++)
        {
            ushort top=map.GetTile(x,(ushort)(first+(period-pad%period)%period));
            ushort bottom=map.GetTile(x,(ushort)(last-period+1+(pad-1)%period));
            if(pad<=6 && top!=TileMap.Empty && (first-pad<0 || map.GetTile(x,(ushort)(first-pad))==TileMap.Empty))yield return new Tile(x,first-pad,top);
            if(pad<=after && bottom!=TileMap.Empty && (last+pad>=map.Height || map.GetTile(x,(ushort)(last+pad))==TileMap.Empty))yield return new Tile(x,last+pad,bottom);
        }
    }
    static int DenseFirst(TileMap map)
    {
        for(int y=0;y<map.Height;y++)
        {
            int count=0;for(ushort x=0;x<map.Width;x++)if(map.GetTile(x,(ushort)y)!=TileMap.Empty)count++;
            if(count>=Mathf.Max(1,Mathf.CeilToInt(map.Width*.6f)))return y;
        }
        return -1;
    }
}

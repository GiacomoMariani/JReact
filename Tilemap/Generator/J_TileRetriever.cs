using JReact.Collections;
using UnityEngine;

namespace JReact.Tilemaps.Generator
{
    [CreateAssetMenu(menuName = "Reactive/Tilemap/Tile Retriever", fileName = "TileRetriever")]
    public class J_TileRetriever : J_ItemRetriever<int, J_TileInfo>
    {
        protected override int GetItemId(J_TileInfo tileInfo) => tileInfo.TileCode;

        public void ClearTileList()
        {
            for (int i = 0; i < _items.Count; i++) { _items[i].Clear(); }
        }
    }
}

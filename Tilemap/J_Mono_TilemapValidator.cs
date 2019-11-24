using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps
{
    public class J_Mono_TilemapValidator : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private Tilemap _tilemap;
        public Tilemap RelatedTilemap => _tilemap;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo[] _relatedTiles;
        public J_TileInfo[] AllRelatedTiles => _relatedTiles;
    }
}

using UnityEngine;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps.Split15
{
    [CreateAssetMenu(menuName = "Reactive/Tilemap/GroundTileBase", fileName = "J_GrounTile", order = 0)]
    public sealed class J_SO_GroundTile : TileBase
    {
        /// <summary>
        /// The Sprites used for defining the Pipeline.
        /// </summary>
        [SerializeField] private Sprite _sprite = null;
        public Sprite Preview => _sprite;
        public static string SpriteFieldName => nameof(_sprite);

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) { UpdateTile(position, tilemap, ref tileData); }
        
        private void UpdateTile(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.transform    = GetPosition();
            tileData.color        = Color.white;
            tileData.sprite       = _sprite;
            tileData.flags        = TileFlags.LockTransform | TileFlags.LockColor;
            tileData.colliderType = Tile.ColliderType.None;
        }

        private Matrix4x4 GetPosition() { return Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one); }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) { base.RefreshTile(position, tilemap); }
    }
}

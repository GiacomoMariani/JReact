using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps
{
    [CreateAssetMenu(menuName = "Reactive/Tilemap/Tile Data", fileName = "Tile")]
    public class J_TileInfo : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _tileInfoId;
        public int TileInfoId => _tileInfoId;
        
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _moveMultiplier = 1f;
        public float MoveMultiplier => _moveMultiplier;
        
        [BoxGroup("Setup", true, true, 0), SerializeField] private JCollisionFlag _collisionFlag;
        public JCollisionFlag CollisionFlag => _collisionFlag;

        [BoxGroup("Setup", true, true, 0), SerializeField] private string _tileName;
        public string TileName => _tileName;

        [BoxGroup("Setup", true, true, 0), SerializeField] private Color _tileColor = Color.white;
        public Color TileColor => _tileColor;

        [InfoBox("NULL => Empty tile"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly]
        private TileBase _unityTile;
        public TileBase UnityTile => _unityTile;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsEmptyTile => UnityTile == default;

        public override string ToString() => $"{_tileInfoId}, {_tileName}, {_tileColor}, {IsEmptyTile}";
    }
}

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps
{
    public class J_Mono_TileDrawer : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo _emptyTile;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private Tilemap _ground;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Mono_TilemapValidator[] _allValidators;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<J_TileInfo, Tilemap> _tileToMap = new Dictionary<J_TileInfo, Tilemap>();

        internal void Setup()
        {
            _tileToMap.Clear();
            _allValidators = GetComponentsInChildren<J_Mono_TilemapValidator>(true);
            
            ClearAllTilemaps();

            var validatorsLength = _allValidators.Length;
            for (int i = 0; i < validatorsLength; i++)
            {
                var validator        = _allValidators[i];
                int tilesInValidator = validator.AllRelatedTiles.Length;
                for (int j = 0; j < tilesInValidator; j++)
                {
                    var tileToAdd = validator.AllRelatedTiles[j];
                    if (_tileToMap.ContainsKey(tileToAdd))
                        JLog.Warning($"{name} key {tileToAdd} was already defined on {_tileToMap[tileToAdd].gameObject}. Moved: {validator.RelatedTilemap}");

                    _tileToMap[tileToAdd] = validator.RelatedTilemap;
                }
            }
        }

        private void ClearAllTilemaps()
        {
            var validatorsLength = _allValidators.Length;
            for (int i = 0; i < validatorsLength; i++) _allValidators[i].RelatedTilemap.ClearAllTiles();
        }

        public void DrawTile(J_TileInfo tileInfo, Vector3Int position)
        {
            if (_emptyTile == tileInfo) return;
            _tileToMap[tileInfo].SetTile(position, tileInfo.UnityTile);
        }

        public Vector3 GetWorldPosition(Vector3Int position) => _ground.GetCellCenterWorld(position);
    }
}

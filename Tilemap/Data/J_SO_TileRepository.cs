using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Tilemaps
{
    [CreateAssetMenu(menuName = "Reactive/Tilemap/Tile Repository", fileName = "AllTiles")]
    public sealed class J_SO_TileRepository : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<int, J_TileInfo> _tileInfoMapping;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo _emptyTileInfo;
        public J_TileInfo EmptyTileInfo => _emptyTileInfo;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo[] _validTiles;

        [Button]
        private void InitRepository()
        {
            _tileInfoMapping = new Dictionary<int, J_TileInfo>(_validTiles.Length + 1);
            AddTileInfo(_emptyTileInfo);
            for (int i = 0; i < _validTiles.Length; i++)
            {
                J_TileInfo tileInfo = _validTiles[i];
                AddTileInfo(tileInfo);
            }
        }

        private void AddTileInfo(J_TileInfo tileInfo)
        {
            Assert.IsFalse(_tileInfoMapping.ContainsKey(tileInfo.TileInfoId),
                           $"{tileInfo.TileInfoId} already in dictionary for {tileInfo}");

            _tileInfoMapping.Add(tileInfo.TileInfoId, tileInfo);
        }

        public J_TileInfo GetTileInfoSafe(int index)
        {
            if (_tileInfoMapping == default) { InitRepository(); }

            return GetTileInfo(index);
        }

        public J_TileInfo GetTileInfo(int index) => _tileInfoMapping[index];
    }
}

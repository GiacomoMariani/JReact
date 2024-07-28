using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Tilemaps
{
    public sealed class J_Repo_AllTileInfo : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<int, J_TileInfo> _tileInfoMapping = new Dictionary<int, J_TileInfo>();

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo _emptyTileInfo;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TileInfo[] _validTiles;

        public void InitRepository()
        {
            _tileInfoMapping.Clear();
            AddTileInfo(_emptyTileInfo);
            for (int i = 0; i < _validTiles.Length; i++)
            {
                var tileInfo = _validTiles[i];
                AddTileInfo(tileInfo);
            }
        }

        private void AddTileInfo(J_TileInfo tileInfo)
        {
            Assert.IsFalse(_tileInfoMapping.ContainsKey(tileInfo.TileInfoId));
            _tileInfoMapping.Add(tileInfo.TileInfoId, tileInfo);
        }

        public J_TileInfo GetTileInfo(int index) => _tileInfoMapping[index];
    }
}

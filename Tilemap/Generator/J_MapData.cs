using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Tilemaps.Generator
{
    [CreateAssetMenu(menuName = "Reactive/Tilemap/Map Data", fileName = "J_MapData")]
    public class J_MapData : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Tilemap_LayerCodes _ground;
        public J_Tilemap_LayerCodes Ground => _ground;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Tilemap_LayerCodes[] _overground;
        public J_Tilemap_LayerCodes[] OvergroundLayers => _overground;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length => Ground ? Ground.Length : 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Width => Ground ? Ground.Width : 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Height => Ground ? Ground.Length / Width : 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public J_TileInfo[] _tileCache;

        internal bool SanityChecks()
        {
            if (Ground                       == null ||
                Ground.Length % Ground.Width != 0)
                throw new ArgumentException($"{name} {nameof(Ground)} is null {Ground == null}, " +
                                            $"or not divisible for width {Ground.Width}. Not enough columns");
            else return true;
        }

        public void Setup() => _tileCache = new J_TileInfo[OvergroundLayers.Length];

        internal J_TileInfo GetGroundTile(int index, J_TileRetriever retriever) => retriever.GetItemFromId(Ground.ArrayCode[index]);

        internal J_TileInfo[] GetOvergroundTiles(int index, J_TileRetriever retriever)
        {
            for (int i = 0; i < OvergroundLayers.Length; i++)
                _tileCache[i] = retriever.GetItemFromId(OvergroundLayers[i].ArrayCode[index]);

            return _tileCache;
        }
    }
}

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps.Split15
{
    //info: https://youtu.be/jEWFSv3ivTg?si=p66uYcyHKzc-7xaD&t=246
    public sealed class J_Tilemap15Split : MonoBehaviour, IJ_TileBoardElement
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_15SplitTileLibrary _tile15Split;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Tilemap _displayTilemap;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _horizontalDisplacement = -0.5f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _verticalDisplacement = -0.5f;

        private void Awake() { Init(); }

        private void Init()
        {
            _displayTilemap.transform.localPosition = new Vector3(_horizontalDisplacement, _verticalDisplacement, 0);
        }

        public void BoardGenerated(J_Mono_MainTileBoard board) { RefreshDisplayTilemap(board); }

        // --------------- COMMANDS --------------- //

        [Button]
        private void RefreshDisplayTilemap(J_Mono_MainTileBoard board)
        {
            Init();
            _displayTilemap.ClearAllTiles();

            Vector3Int startPoint = board.StartPoint;
            int        westEdge   = startPoint.x - 1;
            int        eastEdge   = startPoint.x + board.Width + 2;
            int        southEdge  = startPoint.y - 1;
            int        northEdge  = startPoint.y + board.Height + 2;

            for (int x = westEdge; x < eastEdge; x++)
            {
                for (int y = southEdge; y < northEdge; y++)
                {
                    Vector3Int      position = new Vector3Int(x, y, 0);
                    J_SO_GroundTile tile     = _tile15Split.GetTile(position, board);
                    _displayTilemap.SetTile(position, tile);
                }
            }
        }

        private void OnEnable()
        {
            J_Mono_MainTileBoard mainBoard = J_Mono_MainTileBoard.GetInstanceSafe();
            RefreshDisplayTilemap(mainBoard);
            mainBoard.Subscribe(this);
        }

        private void OnDisable()
        {
            if (!J_Mono_MainTileBoard.IsSingletonAlive) { return; }

            J_Mono_MainTileBoard mainBoard = J_Mono_MainTileBoard.GetInstanceSafe();
            mainBoard.Unsubscribe(this);
        }
    }
}

using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.Tilemaps.Debug
{
#if UNITY_EDITOR
    public abstract class J_Tiles_TextViewer : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _layerName = "UserInterface";
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(1,  50)] private int _dynamicPixelPerUnit = 10;
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(50, 200)] private int _referencePixelsPerUnit = 100;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Color _textColor = Color.white;
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0.01f, 1f)] private float _textSize = .15f;

        [FoldoutGroup("State", false, 5), ShowInInspector] protected abstract J_Mono_MapGrid MapGrid { get; }
        [FoldoutGroup("State", false, 5), ShowInInspector] private Canvas _canvas;
        [FoldoutGroup("State", false, 5), ShowInInspector] private CanvasScaler _scaler;
        [FoldoutGroup("State", false, 5), ShowInInspector] private GameObject _canvasGO;

        [FoldoutGroup("State", false, 5), ShowInInspector] private TextMeshProUGUI _textPrefab;
        [FoldoutGroup("State", false, 5), ShowInInspector] private GameObject _textGO;

        [Button(ButtonSizes.Medium)]
        private void DrawTileCoord(J_Mono_MapGrid grid)
        {
            CheckCanvas();
            CheckTextPrefab();

            for (int i = 0; i < MapGrid.TotalCells; i++)
            {
                var tile = MapGrid.GetTile(i);
                var text = Instantiate(_textPrefab, tile.ToWorldPosition(MapGrid), Quaternion.identity, _canvasGO.transform);
                text.text = SetNameForCell(tile);
            }
        }

        protected virtual string SetNameForCell(JTile tile) => $"{tile.cellPosition.x}, {tile.cellPosition.y}";

        [Button(ButtonSizes.Medium)]
        private void RemoveText()
        {
            if (_canvasGO   != null) Destroy(_canvasGO);
            if (_textPrefab != null) Destroy(_textPrefab);
        }

        //get or recreate the canvas

        private void CheckCanvas()
        {
            if (_canvas   != null) return;
            if (_canvasGO != null) Destroy(_canvasGO);
            _canvasGO = new GameObject();
            _canvasGO.transform.SetParent(this.transform);
            _canvas                  = _canvasGO.AddComponent<Canvas>();
            _canvas.renderMode       = RenderMode.WorldSpace;
            _canvas.sortingLayerName = _layerName;

            _scaler                        = _canvasGO.AddComponent<CanvasScaler>();
            _scaler.dynamicPixelsPerUnit   = _dynamicPixelPerUnit;
            _scaler.referencePixelsPerUnit = _referencePixelsPerUnit;
        }

        private void CheckTextPrefab()
        {
            if (_textPrefab != null) return;

            _textGO = new GameObject();
            _textGO.transform.SetParent(_canvas.transform);
            _textPrefab           = _textGO.AddComponent<TextMeshProUGUI>();
            _textPrefab.color     = _textColor;
            _textPrefab.fontSize  = _textSize;
            _textPrefab.alignment = TextAlignmentOptions.Center;
        }
    }
#endif
}

using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace JReact.Tilemaps.Generator
{
    /// <summary>
    /// represent the code of a given layer
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Tilemap/Layer Codes")]
    public class J_Tilemap_LayerCodes : ScriptableObject, iIntArrayGetter
    {
        //some txt might require to be reverted
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _reverseColumns = false;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _reverseLines = false;

        [FoldoutGroup("State", false, 5), SerializeField] private int _width;
        public int Width => _width;
        [FoldoutGroup("State", false, 5), SerializeField] private int[] _arrayCode;
        public int[] ArrayCode => _arrayCode;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Length => ArrayCode?.Length ?? 0;

#if UNITY_EDITOR
        private const char _Separator = ',';
        [BoxGroup("Generation", true, true, 100), SerializeField, AssetsOnly, Required]
        private TextAsset _text;

        [BoxGroup("Generation", true, true, 100), Button(ButtonSizes.Medium)]
        private void SetupCodes()
        {
            _width     = _text.ConvertString().RemoveEmptyLines().GetFirstLine().SplitWith(_Separator).Length - 1;
            _arrayCode = ConvertFrom(_text);
            if (_arrayCode.Length % _width != 0)
                JLog.Warning($"{name} map has {_arrayCode.Length} cells, with width {_width}, but this is not divisible correctly. " +
                             $"This is mostly caused by:\n"                                                                          +
                             $"- The first line is either different than the other lines of the text"                                +
                             $"- There are not enough cells"                                                                         +
                             $"- The {_width} calculation is not correct");

            EditorUtility.SetDirty(this);
        }

        private int[] ConvertFrom(TextAsset textToConvert)
        {
            var chars =
                textToConvert.ConvertString().RemoveSpace().RemoveEndLine().SplitWith(_Separator);

            int[] result                                     = new int[chars.Length];
            for (int i = 0; i < chars.Length; i++) result[i] = chars[i].ToInt();

            if (_reverseColumns) ReverseColumns(ref result, Width);
            if (_reverseLines) ReverseLines(ref result, Width);

            return result;
        }

        private void ReverseColumns(ref int[] array, int width)
        {
            if (array.Length % width != 0)
                JLog.Warning($"{name} given {nameof(array)} are not divisible for {nameof(width)}. Map could have not enough columns");

            for (int i = 0; i < array.Length / width; i++) Array.Reverse(array, i * width, width);
        }

        private void ReverseLines(ref int[] result, int width) { Array.Reverse(result); }
#endif

        private void OnEnable()
        {
            if (ArrayCode == null ||
                !ArrayCode.ArrayIsValid()) JLog.Error($"{name} requires {nameof(ArrayCode)}", JLogTags.GameBoard, this);
        }
    }
}

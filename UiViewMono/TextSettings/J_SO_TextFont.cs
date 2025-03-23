using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Jreact.UiViewMono.TextSettings
{
    [CreateAssetMenu(menuName = "Reactive/UiCustomization/Text Font", fileName = "J_SO_TextFont", order = 0)]
    public sealed class J_SO_TextFont : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private TMP_FontAsset _fontAsset;
        public TMP_FontAsset FontAsset => _fontAsset;
        
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _defaultSize = 32;
        public float DefaultSize => _defaultSize;
    }
}

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiView
{
    /// <summary>
    /// this class is used to show an image
    /// </summary>
    [RequireComponent(typeof(Image))]
    public abstract class J_UiView_ImageElement : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        //reference to the text
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        protected Image _image;

        //a reference to activate and deactivate the image
        private bool _isActive;
        [BoxGroup("Base", true, true, -5), ReadOnly] public bool IsActive
        {
            get => _isActive;
            protected set
            {
                _isActive      = value;
                _image.enabled = value;
            }
        }

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis()     {}
        protected virtual void SanityChecks() => Assert.IsNotNull(_image, $"{gameObject.name} requires a {nameof(_image)}");

        // --------------- COMMANDS --------------- //
        //sets the sprite on the image
        protected virtual void SetImage(Sprite image) => _image.sprite = image;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_image == null) _image = GetComponent<Image>();
        }
#endif
    }
}

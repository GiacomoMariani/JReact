using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Environment.SkyLight
{
    /// <summary>
    /// A soft blob shadow under an object, following the sky light. The ROOT is the artist's anchor:
    /// place it at the object's base and scale it to the footprint (uniform, unrotated). The child VIEW
    /// is owned by this component and spins around its own center: local +Y points along the light and
    /// stretches by the sky ShadowMultiplier, while the ellipse slides away by _heightAboveGround x
    /// ShadowMultiplier plus half its length (all in footprint units - taller objects throw farther),
    /// and its color glides along a Brightness-indexed gradient. Every published light turn eases to
    /// its pose with PrimeTween.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class J_BlobShadow : MonoBehaviour
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, ChildGameObjectsOnly]
        private Transform _view;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, ChildGameObjectsOnly]
        private SpriteRenderer _spriteRenderer;

        // --------------- SHAPE --------------- //
        [BoxGroup("Shape", true, true, 0), SerializeField, Min(0f)]
        [Tooltip("Crown height in footprint units: the shadow slides away by this x ShadowMultiplier, so taller objects throw farther.")]
        private float _heightAboveGround = 0.5f;
        [BoxGroup("Shape", true, true, 0), SerializeField, Min(0f)] private float _tweenDuration = 0.5f;
        [BoxGroup("Shape", true, true, 0), SerializeField] private Ease _ease = Ease.OutQuad;

        // --------------- COLOR --------------- //
        [BoxGroup("Color", true, true, 0), SerializeField]
        [Tooltip("Shadow color sampled by sky-light Brightness (0..1) every light turn.")]
        private Gradient _colorByBrightness;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Vector2 _baseScale;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float ShadowLength { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float BlobShadowCenterHeight { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_SkyLight _skyLight;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tween _rotationTween;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tween _scaleTween;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tween _positionTween;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tween _colorTween;

        private void Awake()
        {
            Assert.IsNotNull(_view, $"{name} needs its view assigned");
            Assert.IsTrue(_view != transform, $"{name} view must be a CHILD: the root keeps the authored anchor");
            _baseScale = new Vector2(_view.localScale.x, _view.localScale.y);
        }

        // --------------- SKY LIGHT --------------- //
        private void HandleSkyLightChanged(J_SkyLight skyLight)
        {
            SkyLightStrength strength = skyLight.SkyLightStrength;

            // No light -> hide the shadow entirely (component stays subscribed, so it returns with the light).
            if (strength == SkyLightStrength.None ||
                strength == SkyLightStrength.Invalid)
            {
                HideShadow();
                return;
            }

            if (!_spriteRenderer.enabled) { _spriteRenderer.enabled = true; }

            Vector2 cast = skyLight.LightDirection;

            ShadowLength           = _baseScale.y                                     * skyLight.ShadowMultiplier;
            BlobShadowCenterHeight = (_heightAboveGround * skyLight.ShadowMultiplier) + ShadowLength * 0.5f;

            // The view spins around its own center (local 0,0): +Y points along the cast, so Y is the stretch axis.
            float      angle    = Mathf.Atan2(cast.y, cast.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            Vector3    scale    = new Vector3(_baseScale.x, ShadowLength, 1f);
            Vector3    position = cast * BlobShadowCenterHeight;
            float      duration = TweenDurationFor(skyLight);

            StopTweens();
            _rotationTween = Tween.LocalRotation(_view, rotation, duration, _ease);
            _scaleTween    = Tween.Scale(_view, scale, duration, _ease);
            _positionTween = Tween.LocalPosition(_view, position, duration, _ease);
            _colorTween    = Tween.Color(_spriteRenderer, _colorByBrightness.Evaluate(skyLight.Brightness), duration, _ease);
        }

        // Follow the light cadence when it publishes faster than the configured easing.
        private float TweenDurationFor(J_SkyLight skyLight)
        {
            float transitionInterval = skyLight.TransitionInterval;
            return transitionInterval > 0f ? Mathf.Min(_tweenDuration, transitionInterval) : _tweenDuration;
        }

        private void StopTweens()
        {
            _rotationTween.Stop();
            _scaleTween.Stop();
            _positionTween.Stop();
            _colorTween.Stop();
        }

        private void HideShadow()
        {
            StopTweens();
            _spriteRenderer.enabled = false;
        }

        // --------------- LIFECYCLE --------------- //
        private void OnEnable()
        {
            J_St_Environment env = J_St_Environment.GetInstanceSafe();
            _skyLight = env.SkyLight;
            HandleSkyLightChanged(_skyLight); // apply the current light now
            _skyLight.Subscribe(HandleSkyLightChanged);
        }

        private void OnDisable()
        {
            HideShadow();
            if (_skyLight != null) { _skyLight.UnSubscribe(HandleSkyLightChanged); }
        }

        private void OnValidate()
        {
            if (_view                == null &&
                transform.childCount > 0) { _view = transform.GetChild(0); }

            if (_view           != null &&
                _spriteRenderer == null) { _spriteRenderer = _view.GetComponent<SpriteRenderer>(); }
        }
    }
}

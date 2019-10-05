using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiView
{
    /// <summary>
    /// an image that can be dragged
    /// </summary>
    public abstract class J_UiView_DraggableImage : J_UiView_ImageElement
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        //a way to store the starting position
        [BoxGroup("Structure", true, true), ReadOnly] private Vector2 _defaultPosition;

        //layer and canvas
        [BoxGroup("View", true, true, 5), ReadOnly] private string _defaultSortingLayer;
        [BoxGroup("View", true, true, 5), ReadOnly] protected abstract string _DragSortingLayer { get; }
        //the canvas related to this image
        [BoxGroup("View", true, true, 5), ReadOnly] private Canvas _thisCanvas;
        protected Canvas ThisCanvas
        {
            get
            {
                if (_thisCanvas == null) _thisCanvas = GetComponent<Canvas>();
                return _thisCanvas;
            }
        }
        //reference to the canvas scaler for a proper delta adjustment
        [BoxGroup("View", true, true, 5), ReadOnly] private CanvasScaler _canvasScaler;
        private CanvasScaler CanvasScaler
        {
            get
            {
                //get the parent and check it recursively
                if (_canvasScaler == null) _canvasScaler = RetrieveParent(transform);
                return _canvasScaler;
            }
        }

        private CanvasScaler RetrieveParent(Transform parentTransform)
        {
            //if we have no parent we have an error
            if (parentTransform.parent == null)
                throw new MissingComponentException("We have not found any element with the given component: CanvasScaler");

            //check if we found the requested component
            var element = parentTransform.GetComponent<CanvasScaler>();
            //if so return it
            if (element != null) return element;
            //otherwise keep searching

            return RetrieveParent(parentTransform.parent);
        }

        // --------------- INITIALIZATION --------------- //
        //storing the position at initialization
        protected override void InitThis()
        {
            base.InitThis();
            _defaultPosition     = _image.rectTransform.localPosition;
            _defaultSortingLayer = GetComponent<Canvas>().sortingLayerName;
            IsActive             = true;
        }

        //the sanity checks
        protected override void SanityChecks()
        {
            base.SanityChecks();
            //the building image view needs not to stop raycast, to make sure we 
            //can check what's behind it also when dragging the image
            var canvasControl = GetComponent<CanvasGroup>();
            Assert.IsNotNull(canvasControl,
                             "The image view of a building needs a canvas control to allow drag on this gameobject: " +
                             gameObject.name);

            Assert.IsFalse(canvasControl.interactable || canvasControl.blocksRaycasts,
                           "The canvas control needs not to block raycast to allow drag on this gameobject: " + gameObject.name);

            Assert.IsNotNull(ThisCanvas, "The image view needs a canvas to control the sorting layer: " + gameObject.name);
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// when we start dragging we move the image above one sorting layer to make sure it appears above the others
        /// </summary>
        public virtual void StartDragging() { ThisCanvas.sortingLayerName = _DragSortingLayer; }

        /// <summary>
        /// this method will let player move the image around
        /// </summary>
        /// <param name="rawDelta">the position where to set the image</param>
        public void AddToPosition(Vector2 rawDelta)
        {
            //getting reference and current resolution
            Vector2 referenceResolution = CanvasScaler.referenceResolution;
            var     currentResolution   = new Vector2(Screen.width, Screen.height);

            //set the value based on the canvas scaler elements
            float widthRatio  = currentResolution.x / referenceResolution.x;
            float heightRatio = currentResolution.y / referenceResolution.y;
            float ratio       = Mathf.Lerp(widthRatio, heightRatio, CanvasScaler.matchWidthOrHeight);

            //calculating the delta from the raw and ratio
            Vector2 adjustedDelta = rawDelta / ratio;

            //setting the new position
            Vector2 nextPosition = (Vector2) _image.rectTransform.localPosition + adjustedDelta;
            _image.rectTransform.localPosition = nextPosition;
        }

        /// <summary>
        /// this happens when the image is dragged out of the ui, so we hide the image view and we show the sprite view
        /// </summary>
        public void Detach() { IsActive = false; }

        /// <summary>
        /// reset the image position at default and restore it if needed
        /// </summary>
        public virtual void ResetPosition()
        {
            _image.rectTransform.localPosition = _defaultPosition;
            ThisCanvas.sortingLayerName           = _defaultSortingLayer;
            ThisCanvas.overrideSorting            = false;
            IsActive                              = true;
        }

        //call the method on enable otherwise the canvas adjustments does not work when the gameobject is disabled
        private void OnEnable() { ResetPosition(); }
    }
}

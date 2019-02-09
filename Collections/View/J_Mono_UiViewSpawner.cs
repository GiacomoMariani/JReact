﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using JReact.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiView.Collections
{
    /// <summary>
    /// this class is used to show some elements on the ui
    /// </summary>
    [RequireComponent(typeof(GridLayoutGroup))]
    public abstract class J_Mono_UiViewSpawner<T> : MonoBehaviour
    {
        #region FIELDS AND PROPERTIES
        //requires a collection to show
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        protected abstract J_ReactiveCollection<T> _CollectionToShow { get; }
        //the view prefab
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract J_Mono_Actor<T> _UiViewPrefab { get; }
        //the tracked elements
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<T, J_Mono_Actor<T>> _trackedElements = new Dictionary<T, J_Mono_Actor<T>>();
        #endregion

        #region INITIALIZATION
        //used for initialization
        private void Awake()
        {
            //check that everything is as expected
            SanityChecks();
        }

        //used to check that every element is valid
        private void SanityChecks()
        {
            Assert.IsNotNull(_UiViewPrefab, $"This object ({gameObject.name}) needs an element for the value elementToCheck");
            Assert.IsNotNull(_CollectionToShow, $"This object ({gameObject.name}) needs an element for the value _collectionToShow");
        }
        #endregion

        #region VIEW UPDATER
        //setup the views to be shown
        protected virtual void SetupViews()
        {
            //make sure all the elements are shown
            for (int i = 0; i < _CollectionToShow.Count; i++)
                AddView(_CollectionToShow[i]);
        }

        //add a view on the ui
        private void AddView(T elementsToBeShown)
        {
            //if the view is already shown we skip this
            if (WantToShowElement(elementsToBeShown)) return;
            //otherwise we setup the view
            //first we instantiate the view on this transform, that should be a layout group
            var newUiView = Instantiate(_UiViewPrefab, transform);
            //then we initiate the uiView with the element to be shown
            newUiView.UpdateElement(elementsToBeShown);
            //we add the element to the dictionary
            _trackedElements[elementsToBeShown] = newUiView;
            //add further adjustments here
            AddedView(elementsToBeShown);
        }

        protected virtual bool WantToShowElement(T elementsToBeShown)
        {
            if (_trackedElements.ContainsKey(elementsToBeShown))
            {
                JConsole.Warning($"{name} has already the {elementsToBeShown.ToString()}");
                return false;
            }
            return true;
        }

        //an helper method if we want to apply further elements
        protected virtual void AddedView(T elementsToBeShown) { }

        //removes one of the views
        private void RemoveView(T elementChanged)
        {
            //destroy the gameobject
            Destroy(_trackedElements[elementChanged].gameObject);
            //remove the key from the dictionary
            _trackedElements.Remove(elementChanged);
        }
        #endregion

        #region LISTENERS
        //start and stop tracking on enable
        private void OnEnable()
        {
            SetupViews();
            _CollectionToShow.SubscribeToCollectionAdd(AddView);
            _CollectionToShow.SubscribeToCollectionRemove(RemoveView);
        }

        private void OnDisable()
        {
            _CollectionToShow.UnSubscribeToCollectionAdd(AddView);
            _CollectionToShow.UnSubscribeToCollectionRemove(RemoveView);
        }
        #endregion
    }
}

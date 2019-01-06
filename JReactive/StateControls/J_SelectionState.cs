﻿using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControls
{
    /// <summary>
    /// this class represent a simple state that contains also a selected item
    /// </summary>
    /// <typeparam name="T">the type of selected object we want</typeparam>
    public abstract class J_SelectionState<T> : J_State where T : class
    {
        #region VALUES AND PROPERTIES
        //reference to the main control using the selection
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_StateControl _stateControl;
        //the state to be sent on deselection
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_State _deselectionState;
        //we add this if we want to deselect the element when we move out of the state (without deselecting)
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _deselectOnExit = true;

        //the selected item
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private iSelectable<T> _selectedItem;
        public iSelectable<T> SelectedItem
        {
            get => _selectedItem;
            private set
            {
                //deselect the previous item if any
                if (_selectedItem == value) return;
                if (_selectedItem != null) _selectedItem.IsSelected = false;
                //select the next item if any
                _selectedItem = value;
                if (_selectedItem != null) _selectedItem.IsSelected = true;
            }
        }
        #endregion

        #region MAIN COMMANDS
        /// <summary>
        /// this is used to select the given item
        /// </summary>
        /// <param name="itemToSelect">the item to select</param>
        public void SelectThis(iSelectable<T> itemToSelect)
        {
            HelperConsole.DisplayMessage($"{name} is selecting {itemToSelect.NameOfThis}",
                                         J_DebugConstants.Debug_State);
            //selecting the element and call the state
            SelectedItem = itemToSelect;
            _stateControl.SetNewState(this);
        }

        /// <summary>
        /// this is used to deselect all elements and reset this
        /// </summary>
        public void Deselect()
        {
            Assert.IsNotNull(SelectedItem, $"{name} is trying to deselect, but nothing is selected");
            HelperConsole.DisplayMessage($"{name} is deselecting element {SelectedItem.NameOfThis}",
                                         J_DebugConstants.Debug_State);
            SelectedItem = null;
            _stateControl.SetNewState(_deselectionState);
        }
        #endregion

        #region STATE CONTROLS
        public override void RaiseExitEvent()
        {
            //deselect if requested
            if (_deselectOnExit && SelectedItem != null) SelectedItem = null;
            base.RaiseExitEvent();
        }
        #endregion

        #region DISABLE AND RESET
        //we reset this on disable
        protected virtual void OnDisable() { ResetThis(); }

        private void ResetThis()
        {
            if (SelectedItem != null) SelectedItem = null;
        }
        #endregion
    }

    public interface iSelectable<out T> where T : class
    {
        string NameOfThis { get; }
        bool IsSelected { get; set; }
        T ThisElement { get; }
    }
}
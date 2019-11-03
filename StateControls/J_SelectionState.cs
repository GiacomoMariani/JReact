using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    /// <summary>
    /// this class represent a simple state that contains also a selected item
    /// </summary>
    /// <typeparam name="T">the type of selected object we want</typeparam>
    public abstract class J_SelectionState<T> : J_State
        where T : class
    {
        // --------------- VALUES AND PROPERTIES --------------- //
        //reference to the main control using the selection
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_SimpleStateControl _stateControl;

        //the state to be sent on deselection
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_State _deselectionState;
        //if we want to deselect the element when we move out of the state (without deselecting)
        [BoxGroup("Setup", true, true), SerializeField] private bool _deselectOnExit = true;

        //the selected item
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iSelectable<T> _selectedItem;
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

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// this is used to select the given item
        /// </summary>
        /// <param name="itemToSelect">the item to select</param>
        public void SelectThis(iSelectable<T> itemToSelect)
        {
            JLog.Log($"{name} is selecting {itemToSelect.NameOfThis}", JLogTags.State, this);
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
            JLog.Log($"{name} is deselecting element {SelectedItem.NameOfThis}", JLogTags.State, this);
            SelectedItem = null;
            _stateControl.SetNewState(_deselectionState);
        }

        protected override void EndThis()
        {
            //deselect if requested
            if (_deselectOnExit && SelectedItem != null) SelectedItem = null;
            base.EndThis();
        }
    }
}

using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiView
{
    /// <summary>
    /// used to get a double click before confirming a button command
    /// </summary>
    public abstract class J_ButtonDoubleClick : J_UiView_ButtonElement
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        //the time span to accept the interval
        [BoxGroup("State", true, true, 5), SerializeField, Range(0.1f, 3.0f)] private float _maxIntervalBetweenPress = 2.0f;

        //the last pressed time
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private float _lastTapTime;

        // --------------- BUTTON INTERFACE --------------- //
        /// <summary>
        /// the button command is implemented here
        /// </summary>
        protected override void ButtonCommand()
        {
            //if tapped within the interval
            if (Time.time - _lastTapTime < _maxIntervalBetweenPress)
            {
                //used to avoid further press
                _lastTapTime = Time.time + _maxIntervalBetweenPress + 1f;
                //confirm the command
                PressConfirmed();
            }
            //otherwise
            else
            {
                //store the time
                _lastTapTime = Time.time;
                //consider this the first press
                FirstPress();
            }
        }

        // --------------- MULTI CLICK COMMANDS --------------- //
        /// <summary>
        /// the main command for the double press
        /// </summary>
        protected abstract void PressConfirmed();

        /// <summary>
        /// this is used if we want to apply something at the first press
        /// </summary>
        protected virtual void FirstPress() {}
    }
}

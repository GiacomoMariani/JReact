using System.Collections.Generic;
using JReact.Movement;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace JReact.Pool.Roamer
{
    /// <summary>
    /// a roamer that randomly moves on a board
    /// </summary>
    [RequireComponent(typeof(J_TransformMover))]
    public sealed class J_Mono_Roamer : MonoBehaviour, iResettable, iDestroyable
    {
        // --------------- CONSTANTS --------------- //
        private const string COROUTINE_Timeout = "COROUTINE_RoamerTimeout";
        private const string COROUTINE_Adjust = "COROUTINE_RoamerAdjust";

        // --------------- SETUP --------------- // F
        [BoxGroup("Setup", true, true), SerializeField] private float _speedMultiplier = 1f;
        //the time to adapt to the new wind speed
        [BoxGroup(                                                              "Setup", true, true), SerializeField, MinMaxSlider(0f, 5f)] private Vector2 _adaptionTimeRange = new Vector2(0f, 5f);
        [FormerlySerializedAs("_prefab"),FormerlySerializedAs("_pool"),BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Mono_Roamer _roamerPrefab;
        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Wind _windControl;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_GameBorders _borders;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_TransformMover _transformMover;
        private J_TransformMover TransformMover
        {
            get
            {
                if (_transformMover == null) _transformMover = GetComponent<J_TransformMover>();
                return _transformMover;
            }
        }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Vector2 _CurrentPosition
            => TransformMover.CurrentPosition;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId = -1;

        /// <summary>
        /// sets up the roamer
        /// </summary>
        /// <param name="wind">the wind that will the direction</param>
        /// <param name="borders">the borders of the gameboard</param>
        /// <param name="speed">the speed multiplier</param>
        /// <param name="scale">the scale of the roamer</param>
        /// <param name="lifeTimeMins">the max amount of time before removing the roamer</param>
        public void Setup(J_Wind wind, J_GameBorders borders, float speed = 1f, float scale = 1f, float lifeTimeMins = 2f)
        {
            Assert.IsNotNull(TransformMover, $"{gameObject.name} requires a  TransformMover");
            _instanceId = GetInstanceID();

            // --------------- REFERENCE --------------- //
            //storing the values
            _windControl     = wind;
            _borders         = borders;
            _speedMultiplier = speed;
            //set the scale
            transform.localScale = new Vector2(scale, scale);

            // --------------- MOVE --------------- //
            StartMoveRoamer(_windControl.WindSpeed * _speedMultiplier);
            if (lifeTimeMins > 0)
                Timing.RunCoroutine(Timeout(lifeTimeMins * JConstants.SecondsInMinute).CancelWith(gameObject),
                                    Segment.SlowUpdate, _instanceId, COROUTINE_Timeout);

            _windControl.Subscribe(FollowWind);
        }

        //moves the roamer
        private void StartMoveRoamer(Vector2 direction)
        {
            TransformMover.StartMovingFixedDirection(direction);
            TransformMover.SubscribeToMovement(CheckIfOut);
        }

        //this is used to track the wind and keep following it
        private void FollowWind(Vector2 windSpeed)
        {
            //calculate adaption time
            float adaptionTime = _adaptionTimeRange.GetRandomValue();
            //calculate the new speed
            Vector2 newSpeed = windSpeed * _speedMultiplier;
            //we run the move coroutine giving it a layer and a tag
            Timing.RunCoroutine(UpdateDirection(newSpeed, adaptionTime).CancelWith(gameObject),
                                Segment.FixedUpdate, _instanceId, COROUTINE_Adjust);
        }

        //used to change the next step
        private IEnumerator<float> UpdateDirection(Vector2 newDirection, float timeToChange)
        {
            //the time passed for this current step
            float elapsedTime = 0f;
            //the starting position is the position of the transform at start
            Vector2 startingPos = _transformMover.MoveDirection;
            //keep moving until we reach the time
            while (elapsedTime < timeToChange)
            {
                //moving with a lerp
                Vector3 currentDirection = Vector3.Lerp(startingPos, newDirection, elapsedTime / timeToChange);
                //set the new direction on the control
                _transformMover.SetFixedDirection(currentDirection);
                //adding the elapsed time
                elapsedTime += Time.fixedDeltaTime;
                //wait a frame
                yield return Timing.WaitForOneFrame;
            }
        }

        // --------------- BORDER CHECKS --------------- //
        private void CheckIfOut(Vector3 currentPosition)
        {
            if (OutOfBorders(currentPosition, _borders)) DestroyThis();
        }

        //check if we are out of the borders
        private bool OutOfBorders(Vector2 positionToCheck, J_GameBorders borders) => positionToCheck.x > borders.RightBorder ||
                                                                                     positionToCheck.x < borders.LeftBorder ||
                                                                                     positionToCheck.y > borders.UpBorder   ||
                                                                                     positionToCheck.y < borders.DownBorder;

        // --------------- TIMEOUT AND DESTROY COMMANDS --------------- //
        //used to keep the cloud alive for an amount of time
        private IEnumerator<float> Timeout(float lifeSpan)
        {
            yield return Timing.WaitForSeconds(lifeSpan);
            DestroyThis();
        }

        public void DestroyThis()
        {
            ResetThis();
            _roamerPrefab.DeSpawn(gameObject);
        }

        public void ResetThis()
        {
            gameObject.SetActive(false);
            TransformMover.StopMoveTransform();
            if (_windControl    != null) _windControl.UnSubscribe(FollowWind);
            if (_transformMover != null) _transformMover.UnSubscribeToMovement(CheckIfOut);
        }
    }
}

using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace JReact.Pool.Roamer
{
    /// <summary>
    /// spawns roamer at given intervals
    /// </summary>
    public sealed class J_RoamerSpawn : MonoBehaviour
    {
        // --------------- MAIN SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_Wind _windControl;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_GameBorders _borders;
        [FormerlySerializedAs("_roamerPool"), BoxGroup("Setup", true, true), SerializeField]
        private J_Mono_Roamer _roamerPrefab;

        // --------------- ROAMER SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private float _adjustmentOnZ;
        [BoxGroup("Setup", true, true), SerializeField] private Vector2 _secondsForSpawn = new Vector2(10f,      30f);
        [BoxGroup("Setup", true, true), SerializeField] private Vector2 _roamerSpeedRange = new Vector2(0.5f,    10);
        [BoxGroup("Setup", true, true), SerializeField] private Vector2 _raomerScale = new Vector2(1f,           1f);
        [BoxGroup("Setup", true, true), SerializeField] private Vector2 _roamerLifetimeMinutes = new Vector2(1f, 1f);

        // --------------- STATE AND BOOK KEEPING --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<J_Mono_Roamer> _roamers = new List<J_Mono_Roamer>();

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private CoroutineHandle _coroutine;

        // --------------- COMMANDS --------------- //
        /// <inheritdoc />
        /// <summary>
        ///  starts spawning roamers
        /// </summary>
        public void ActivateThis()
        {
            SanityChecks();
            _coroutine = Timing.RunCoroutine(RoamersSpawn(), Segment.LateUpdate);
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_roamerPrefab, $"{name} requires a _roamerPool");
            Assert.IsNotNull(_borders,      $"{name} requires a _borders");
        }

        public void EndThis()
        {
            Timing.KillCoroutines(_coroutine);
            ClearRoamers();
        }

        private void ClearRoamers()
        {
            for (int i = 0; i < _roamers.Count; i++)
            {
                if (_roamers[i] != null) _roamers[i].DestroyThis();
            }

            _roamers.Clear();
        }

        // --------------- CONTROLS --------------- //
        //spawn the roamers
        private IEnumerator<float> RoamersSpawn()
        {
            while (true)
            {
                // --------------- FIND THE POSITION --------------- //
                //get the position based on the wind flow
                Vector3 positionOfSpawn = GetSpawnPosition(_windControl.CurrentDirection);

                // --------------- ROAMER CREATION --------------- //
                //instantiate a roamer, on the given position
                J_Mono_Roamer roamer = _roamerPrefab.Spawn();
                roamer.transform.position = positionOfSpawn;
                if (!roamer.gameObject.activeSelf) roamer.gameObject.SetActive(true);
                //injecting the wind and the start speed
                roamer.Setup(_windControl, _borders, _roamerSpeedRange.GetRandomValue(), _raomerScale.GetRandomValue(),
                             _roamerLifetimeMinutes.GetRandomValue());

                // --------------- WAIT --------------- //
                //wait then spawn again
                yield return Timing.WaitForSeconds(_secondsForSpawn.GetRandomValue());
            }
        }

        //find the spawn position based on the wind
        private Vector3 GetSpawnPosition(Direction windDirection)
        {
            switch (windDirection)
            {
                //if the direction is up the roamer will spawn from the bottom
                case Direction.Up: return new Vector3(RandomHorizontalPosition(), _borders.DownBorder, _adjustmentOnZ);

                //if the direction is right the roamer will spawn from the left
                case Direction.Right: return new Vector3(_borders.LeftBorder, RandomVerticalPosition(), _adjustmentOnZ);

                //if the direction is down the roamer will spawn from the top
                case Direction.Down: return new Vector3(RandomHorizontalPosition(), _borders.UpBorder, _adjustmentOnZ);

                //if the direction is left the roamer will spawn from the right
                case Direction.Left: return new Vector3(_borders.RightBorder, RandomVerticalPosition(), _adjustmentOnZ);

                case Direction.None: return new Vector3(0, 0, _adjustmentOnZ);
                default:             throw new ArgumentOutOfRangeException("windDirection", windDirection, null);
            }
        }

        private float RandomHorizontalPosition() => UnityEngine.Random.Range(_borders.LeftBorder, _borders.RightBorder);
        private float RandomVerticalPosition()   => UnityEngine.Random.Range(_borders.DownBorder, _borders.UpBorder);
    }
}

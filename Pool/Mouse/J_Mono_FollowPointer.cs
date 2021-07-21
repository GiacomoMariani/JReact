using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JReact.Pool.Mouse
{
    public class J_Mono_FollowPointer : MonoBehaviour
    {
        private static int _pointerCreated = 0;
        // --------------- SETUP --------------- //
        [FoldoutGroup("State", false, 15), ReadOnly, ShowInInspector] public GameObject CursorTracker { get; private set; }
        [FoldoutGroup("State", false, 15), ReadOnly, ShowInInspector] public bool ActiveCursor => CursorTracker != null;
        [FoldoutGroup("State", false, 15), ReadOnly, ShowInInspector] private CoroutineHandle _handle;
        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 15), ReadOnly, ShowInInspector] private Camera _mainCamera;

        [FoldoutGroup("State - Effects", false, 25), ReadOnly, ShowInInspector] private bool _activeEffect;

        // --------------- STARTUP --------------- //
        /// <summary>
        /// creates a game object following the mouse
        /// </summary>
        /// <param name="mainCamera">we the main camera</param>
        public void EnableTracker(Camera mainCamera, Pointer pointerToFollow)
        {
            if (_handle.IsRunning) { return; }

            CursorTracker ??= new GameObject();
#if UNITY_EDITOR
            CursorTracker.name = $"PointerTracker_{_pointerCreated}";
            _pointerCreated++;
#endif
            // Cursor.visible = hideRealCursor;
            _handle = Timing.RunCoroutine(FollowPointer(mainCamera, pointerToFollow), Segment.LateUpdate,
                                          JCoroutineTags.COROUTINE_MouseFollow);
        }

        private IEnumerator<float> FollowPointer(Camera mainCamera, Pointer pointerToFollow)
        {
            while (true)
            {
                CursorTracker.transform.position = mainCamera.ScreenToWorldPoint(pointerToFollow.position.ReadValue());
                yield return Timing.WaitForOneFrame;
            }
        }

        /// <summary>
        /// disables the cursor
        /// </summary>
        public void DisableFollowPointer() { Timing.KillCoroutines(_handle); }
    }
}

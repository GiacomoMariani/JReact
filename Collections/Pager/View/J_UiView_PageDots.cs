using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.UiView.Collections
{
    public sealed class J_UiView_PageDots : MonoBehaviour
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_Pager _controls;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private Image _pointPrefab;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private Sprite _active;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private Sprite _inactive;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<Image> _points = new List<Image>();

        // --------------- INIT --------------- //
        private void Awake() => transform.ClearTransform();

        // --------------- DOT CHANGES --------------- //
        private void PageUpdate(J_UiView_Page page)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                if (i == page.Identifier) _points[i].sprite = _active;
                else _points[i].sprite                      = _inactive;
            }
        }

        //check if we have enough points or spawn them
        private void PointsUpdate()
        {
            int required = _controls.TotalPages;
            int current  = _points.Count;

            //stop if we have enough
            if (current == required) return;

            if (required < current)
                for (int i = 0; i < current - required; i++)
                    RemoveOneDot(null);

            if (required > current)
                for (int i = 0; i < required - current; i++)
                    AddOneDot(null);
        }

        private void AddOneDot(J_UiView_Page page)
        {
            var dot = Instantiate(_pointPrefab, transform);
            dot.sprite = _inactive;
            _points.Add(dot);
        }

        private void RemoveOneDot(J_UiView_Page page)
        {
            var dot = _points[_points.Count - 1];
            _points.Remove(dot);
            Destroy(dot.gameObject);
        }

        // --------------- UNITY EVENTS --------------- //
        private void OnEnable()
        {
            PointsUpdate();
            _controls.OnPage_Change += PageUpdate;
            _controls.OnPage_Create += AddOneDot;
            _controls.OnPage_Remove += RemoveOneDot;
            PageUpdate(_controls.Current);
        }

        private void OnDisable()
        {
            _controls.OnPage_Change -= PageUpdate;
            _controls.OnPage_Create -= AddOneDot;
            _controls.OnPage_Remove -= RemoveOneDot;
        }
    }
}

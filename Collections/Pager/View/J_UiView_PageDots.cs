using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace JReact.UiView.Collections
{
    public sealed class J_UiView_PageDots : MonoBehaviour
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, Required] private J_PagerEvents _controls;
        [BoxGroup("Setup", true, true), SerializeField, Required] private Image _pointPrefab;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private Sprite _active;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private Sprite _inactive;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<Image> _points = new List<Image>();

        // --------------- INIT --------------- //
        private void Awake() => transform.ClearTransform();

        // --------------- DOT CHANGES --------------- //
        private void PageUpdate(int page)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                _points[i].sprite = i == page
                                        ? _active
                                        : _inactive;
            }
        }

        //check if we have enough points or spawn them
        private void PointsUpdate(int nonRequired)
        {
            int required = _controls.Total;
            int current  = _points.Count;

            //stop if we have enough
            if (current == required) return;

            if (required < current)
                for (int i = 0; i < current - required; i++)
                    RemoveOneDot();

            if (required > current)
                for (int i = 0; i < required - current; i++)
                    AddOneDot();
        }

        private void AddOneDot()
        {
            Image dot = Instantiate(_pointPrefab, transform);
            dot.sprite = _inactive;
            _points.Add(dot);
        }

        private void RemoveOneDot()
        {
            Image dot = _points[_points.Count - 1];
            _points.Remove(dot);
            Destroy(dot.gameObject);
        }

        // --------------- UNITY EVENTS --------------- //
        private void OnEnable()
        {
            PointsUpdate(0);
            PageUpdate(_controls.Current);
            _controls.OnIndexChanged += PageUpdate;
            _controls.OnTotalChanged += PointsUpdate;
        }

        private void OnDisable()
        {
            _controls.OnIndexChanged -= PageUpdate;
            _controls.OnTotalChanged -= PointsUpdate;
        }
    }
}

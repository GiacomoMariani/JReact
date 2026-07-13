using System;
using UnityEngine;

namespace JReact.Tilemaps
{
    /// <summary>
    /// Rendering sorting data for a tilemap layer: the sorting layer name and the order within it.
    /// Lets a project inject its own sorting layers without Jreact depending on any game specific type.
    /// </summary>
    [Serializable]
    public class JSortLayer
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [SerializeField] private string _layer;
        [SerializeField] private int _order;

        public string Layer => _layer;
        public int Order => _order;

        public JSortLayer(string layer, int order)
        {
            _layer = layer;
            _order = order;
        }
    }
}

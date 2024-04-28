using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// Represents a base activator that allows enabling or disabling an associated GameObject.
    /// </summary>
    [Serializable]
    public sealed class JActivable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        /// <summary>
        /// The reference to the GameObject variable _item.
        /// </summary>
        [SerializeField, Required] private GameObject _item;

        /// <summary>
        /// Creates a new instance of <see cref="JActivable"/> using a <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> to create the <see cref="JActivable"/> instance from.</param>
        /// <returns>A new instance of <see cref="JActivable"/> with the specified <paramref name="gameObject"/>.</returns>
        public static JActivable FromGameObject(GameObject gameObject) => new JActivable() { _item = gameObject };
        
        /// <summary>
        /// Gets or sets a value indicating whether the item is enabled.
        /// </summary>
        /// <remarks>
        /// When <see langword="true"/>, the item is active. When <see langword="false"/>, the item is inactive.
        /// </remarks>
        /// <value><see langword="true"/> if the item is enabled; otherwise, <see langword="false"/>.</value>
        public bool IsActive { get => _item.activeSelf; private set => _item.SetActive(value); }

        /// <summary>
        /// Toggles the current state of the IsEnabled property.
        /// </summary>
        /// <returns>Returns the new value of the IsEnabled property after toggling.</returns>
        public bool Toggle() => IsActive = !IsActive;

        /// <summary>
        /// Sets the <see cref="IsActive"/> flag to activate or deactivate the component.
        /// </summary>
        /// <param name="activate">Determines whether to activate or deactivate the component.</param>
        public void SetActive(bool activate) => IsActive = activate;

        public GameObject Get() => _item;

        // --------------- OPERATORS --------------- //
        public static implicit operator bool(JActivable activable) => activable.IsActive;
        public static bool operator true(JActivable  item) => item.IsActive;
        public static bool operator false(JActivable item) => !item.IsActive;
    }
}

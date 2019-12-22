using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Credits
{
    [CreateAssetMenu(menuName = "Reactive/Credits/Credit Entry", fileName = "Entry", order = 2)]
    public class J_CreditSection : J_ReactiveArray<string>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private string _sectionName;
        public string SectionName => _sectionName;
    }
}

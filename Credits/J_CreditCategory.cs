using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Credits
{
    [CreateAssetMenu(menuName = "Reactive/Credits/Credit Category", fileName = "Category", order = 1)]
    public class J_CreditCategory : J_ReactiveArray<J_CreditSection>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private string _categoryName;
        public string CategoryName => _categoryName;
    }
}

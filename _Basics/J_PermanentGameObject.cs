using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public class J_PermanentGameObject : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private void Awake()
        {
            Assert.IsTrue(transform.root == transform, $"{gameObject.FullName()} is not a root.");
            DontDestroyOnLoad(gameObject);
        }
    }
}

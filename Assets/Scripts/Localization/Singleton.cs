using UnityEngine;

namespace Localization
{
    public abstract class Singleton<T> : MonoBehaviour where T : class
    {
        public static T Instance;

        public void Awake()
        {
            Instance = this as T;
        }
    }
}
using UnityEngine;

/// <summary>
/// Singleton base class for MonoBehaviours.
/// Inherit from this to create a singleton: public class MyClass : Singleton<MyClass>
/// </summary>
namespace MyUtility {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;
        private static object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance {
            get {
                if (_applicationIsQuitting) {
                    Debug.LogWarning($"[Singleton] Instance of {typeof(T)} already destroyed. Returning null.");
                    return null;
                }

                lock (_lock) {
                    if (_instance == null) {
                        // Search for existing instance
                        _instance = Object.FindAnyObjectByType<T>();

                        // Create new instance if one doesn't already exist
                        if (_instance == null) {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            // Make instance persistent across scenes
                            DontDestroyOnLoad(singletonObject);
                            
                           
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake() {
            if (_instance == null) {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                transform.SetParent(null);
            } else if (_instance != this) {
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit() {
            _applicationIsQuitting = true;
        }

        protected virtual void OnDestroy() {
            if (_instance == this) {
                _applicationIsQuitting = true;
            }
        }
    }
}


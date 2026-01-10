
using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour {
    [Scene, SerializeField] private string firstLevel;

    public void LoadNewgameScene() {
        var asyncOperation = SceneManager.LoadSceneAsync(firstLevel);
        
        // Prevent automatic scene activation until you're ready
        asyncOperation.allowSceneActivation = false;
        
        // Optional: Monitor loading progress
        StartCoroutine(HandleSceneLoading(asyncOperation));
    }

    private IEnumerator HandleSceneLoading(AsyncOperation asyncOperation) {
        // Wait until scene is fully loaded (0.9 = 90% ready)
        while (asyncOperation.progress < 0.9f) {
            Debug.Log($"Loading progress: {asyncOperation.progress * 100}%");
            yield return null;
        }
        
        // Now activate the scene when ready
        asyncOperation.allowSceneActivation = true;
        
        yield return asyncOperation;
        Debug.Log("Scene loaded successfully!");
    }
    public void QuitGame() {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public  void ResumeGame() {
        var result = SaveManager.Instance.QuickLoad();
        
        var sceneName = SaveManager.Instance.CurrentGameSaveData.SceneName;
        //Load the scene 
        SceneManager.LoadScene(sceneName);
       
    }
    public void OpenURL(string url) {
        Application.OpenURL(url);
    }
    private void Start() {
        SceneManager.sceneUnloaded += (scene) => {
            Debug.Log($"Unloaded scene: {scene.name}");
        };
    }
    private void OnDisable() => Debug.Log("UIManager OnDisable");
    private void OnDestroy() {
        Debug.Log("UIManager OnDestroy");
    }
}


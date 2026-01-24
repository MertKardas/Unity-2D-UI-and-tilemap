
using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour {
    [Scene, SerializeField] private string firstLevel;
    [SerializeField] private Button resumeGameButton;
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
    public void ResumeGame() {
        var result = SaveManager.Instance.QuickLoad();

        if (!result.Success) {
            Debug.LogError($"Failed to load save: {result.ErrorMessage}");
            return;
        }

        var lastSceneName = SaveManager.Instance.CurrentGameSaveData.LastSceneName;
        if (string.IsNullOrEmpty(lastSceneName)) {
            Debug.LogError("No saved scene name found");
            return;
        }

        var asyncOperation = SceneManager.LoadSceneAsync(lastSceneName);
        StartCoroutine(HandleSceneLoading(asyncOperation));
    }
    public void OpenURL(string url) {
        Application.OpenURL(url);
    }
    private void Start() {
        resumeGameButton.interactable = false; 
        var result = SaveManager.Instance.LoadAllMetadata(out var metadatas);
        if(result.Success && metadatas.Length >0) {
            resumeGameButton.interactable = true;
        }
    }
   
}


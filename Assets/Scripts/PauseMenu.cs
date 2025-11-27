using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.WebRequestMethods;
public class PauseMenu : MonoBehaviour
{
    [NaughtyAttributes.Scene, SerializeField] public string mainMenuSceneName;
    private string GitHubURL = "https://github.com/MertKardas";
    private string LinkedInURL = "https://www.linkedin.com/in/mert-karda%C5%9F-b1600a298/";
    public void OpenGitHub() {
        ProcessStartInfo psi = new ProcessStartInfo {
            FileName = GitHubURL,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
    public void OpenLinkedIn() {
        ProcessStartInfo psi = new ProcessStartInfo {
            FileName = LinkedInURL,
            UseShellExecute = true
        };
        UnityEngine.Debug.Log("Opening LinkedIn URL: " + LinkedInURL);
        Process.Start(psi);
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

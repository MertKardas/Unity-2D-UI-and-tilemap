using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    InputSystem_Actions inputActions;
    private GameObject previousMenu; 
    public GameObject currentMenu;
    private AudioSource _audioSource; 
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.UI.Enable();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SwitchMenu(GameObject newMenu)
    {
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
            previousMenu = currentMenu;
        }
        currentMenu = newMenu;
        currentMenu.SetActive(true);
    }
    public void ClickEventTest()
    {
        Debug.Log("Click Event Test");
    }
    public void AudioPlay (AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }
}

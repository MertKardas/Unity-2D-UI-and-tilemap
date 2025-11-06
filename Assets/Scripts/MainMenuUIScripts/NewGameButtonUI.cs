using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class NewGameButtonUI : MenuButtonUI {
    [SerializeField] private GameObject _newGamePanel; 

    public override void ButtonOnClick() {
        base.ButtonOnClick();
        _newGamePanel.SetActive(true);
    }

}

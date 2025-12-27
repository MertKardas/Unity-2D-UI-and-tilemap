using UnityEngine;
using UnityEngine.EventSystems;

public class BackGameButtonUI : MenuButtonUI{
    [SerializeField] private Transform _newGamePanel;  
    protected override void Start() {
        base.Start();
        
    }

    public override void ButtonOnClick() {
        base.ButtonOnClick();
        _newGamePanel.gameObject.SetActive(false);
        
    }
}
   

 
    


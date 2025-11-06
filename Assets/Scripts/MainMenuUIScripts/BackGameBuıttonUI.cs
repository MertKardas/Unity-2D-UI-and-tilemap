using UnityEngine;
using UnityEngine.EventSystems;

public class BackGameBuıttonUI : MenuButtonUI{
    [SerializeField] private Transform _newGamePanel;  
    protected override void Awake() {
        base.Awake();
        
    }

    public override void ButtonOnClick() {
        base.ButtonOnClick();
        _newGamePanel.gameObject.SetActive(false);
        
    }
}
   

 
    


using UnityEngine;
using TMPro;
public class GameSaveTextUI : MonoBehaviour
{
    TMP_Text _saveText;
    void Awake()
    {
        _saveText = GetComponent<TMP_Text>();
    }
    void OnEnable()
    {

        SaveManager.Instance.OnSaveAfter += UpdateSaveText;
    }
    private void OnDisable()
    {
        LeanTween.cancel(gameObject);
        if(SaveManager.Instance != null)
            SaveManager.Instance.OnSaveAfter -= UpdateSaveText;
    }
    private void UpdateSaveText(Result result)
    {
        LeanTween.cancel(gameObject);
        if (result.Success)
        {
            string fullText = "Game saved successfully.";
            _saveText.text = "";
            LeanTween.value(gameObject, 0f, fullText.Length, 0.5f)
                .setOnUpdate((float val) =>
                {
                    _saveText.text = fullText.Substring(0, Mathf.FloorToInt(val));
                }).setOnComplete(() =>
                {
                    LeanTween.delayedCall(0.5f, () =>
                    {
                        _saveText.text = "";
                    });
                });
        }
        else
        {
            _saveText.text = "Game save failed: " + result.ErrorMessage;
        }
    }

}

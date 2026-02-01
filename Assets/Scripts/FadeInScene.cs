using UnityEngine;

public class FadeInScene : MonoBehaviour
{
    private void OnEnable() {
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null) {
            canvasGroup.alpha = 1f;
            LeanTween.alphaCanvas(canvasGroup, 0f, 1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
                gameObject.SetActive(false);
            });
        }
    }
}

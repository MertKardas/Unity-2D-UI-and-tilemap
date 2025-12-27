using UnityEngine;

[CreateAssetMenu(fileName = "UITransition", menuName = "UI/Transitions/New UI Transition")]
public class UITransition : ScriptableObject, IPanelTransition {
    [SerializeField] float time = 0.5f;
    [SerializeField] LeanTweenType fromEaseType = LeanTweenType.easeInOutQuad;
    [SerializeField] LeanTweenType toEaseType = LeanTweenType.easeInOutQuad;
    [SerializeField] bool useUnscaledTime = true;

    public LTSeq Transition(GameObject from, GameObject to)
    {
        // At least one panel must exist
        if (from == null && to == null)
        {
            Debug.LogWarning("UITransition: Both 'from' and 'to' GameObjects are null");
            return LeanTween.sequence();
        }

        // Validate all components upfront
        if (from != null)
        {
            var fromCanvasGroup = from.GetComponent<CanvasGroup>();
            var fromRectTransform = from.GetComponent<RectTransform>();

            if (fromCanvasGroup == null || fromRectTransform == null)
            {
                Debug.LogWarning("UITransition: 'from' panel missing CanvasGroup or RectTransform");
                return LeanTween.sequence();
            }
        }

        if (to != null)
        {
            var toCanvasGroup = to.GetComponent<CanvasGroup>();
            var toRectTransform = to.GetComponent<RectTransform>();

            if (toCanvasGroup == null || toRectTransform == null)
            {
                Debug.LogWarning("UITransition: 'to' panel missing CanvasGroup or RectTransform");
                return LeanTween.sequence();
            }
        }

        var sequence = LeanTween.sequence();

        // Handle fade out / scale down of 'from' panel
        if (from != null)
        {
            var fromCanvasGroup = from.GetComponent<CanvasGroup>();
            var fromRectTransform = from.GetComponent<RectTransform>();

            LeanTween.cancel(from);
            sequence.append(
                () => fromCanvasGroup.interactable = false
            );
            sequence.append(
                LeanTween
                    .alphaCanvas(fromCanvasGroup, 0, time)
                    .setEase(fromEaseType)
                    .setIgnoreTimeScale(useUnscaledTime)
                    .setOnComplete(() => {
                        
                        from.SetActive(false);
                    })
            );

            sequence.insert(
                LeanTween
                    .scale(fromRectTransform, Vector3.zero, time)
                    .setEase(fromEaseType)
                    .setIgnoreTimeScale(useUnscaledTime)
            );
        }

        // Handle fade in / scale up of 'to' panel
        if (to != null)
        {
            var toCanvasGroup = to.GetComponent<CanvasGroup>();
            var toRectTransform = to.GetComponent<RectTransform>();

            LeanTween.cancel(to);

            // Set initial state
            to.SetActive(true);
            toCanvasGroup.alpha = 0f;
            toRectTransform.localScale = Vector3.zero;
            toCanvasGroup.interactable = false;

            sequence.append(
                LeanTween
                    .alphaCanvas(toCanvasGroup, 1, time)
                    .setEase(toEaseType)
                    .setIgnoreTimeScale(useUnscaledTime)
                    .setOnComplete(() => toCanvasGroup.interactable = true)
            );

            sequence.insert(
                LeanTween
                    .scale(toRectTransform, Vector3.one, time)
                    .setEase(toEaseType)
                    .setIgnoreTimeScale(useUnscaledTime)
            );
        }

        return sequence;
    }
}

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

            if (fromCanvasGroup == null)
            {
                Debug.LogWarning("UITransition: 'from' panel missing CanvasGroup");
                return LeanTween.sequence();
            }
        }

        if (to != null)
        {
            var toCanvasGroup = to.GetComponent<CanvasGroup>();

            if (toCanvasGroup == null)
            {
                Debug.LogWarning("UITransition: 'to' panel missing CanvasGroup");
                return LeanTween.sequence();
            }
        }

        var sequence = LeanTween.sequence();

        // Handle fade out of 'from' panel
        if (from != null)
        {
            var fromCanvasGroup = from.GetComponent<CanvasGroup>();

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

        }

        // Handle fade in of 'to' panel
        if (to != null)
        {
            var toCanvasGroup = to.GetComponent<CanvasGroup>();

            LeanTween.cancel(to);

            // Set initial state
            to.SetActive(true);
            toCanvasGroup.alpha = 0f;
            toCanvasGroup.interactable = false;

            sequence.append(
                LeanTween
                    .alphaCanvas(toCanvasGroup, 1, time)
                    .setEase(toEaseType)
                    .setIgnoreTimeScale(useUnscaledTime)
                    .setOnComplete(() => toCanvasGroup.interactable = true)
            );

           
        }

        return sequence;
    }
}

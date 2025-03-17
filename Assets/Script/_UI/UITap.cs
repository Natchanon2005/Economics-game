using UnityEngine;
using DG.Tweening;

public class UITap : MonoBehaviour
{
    [Header("UI Settings")]
    public RectTransform targetObject; // Target UI element to animate
    public float moveDistance = 10f; // Distance to move up
    [SerializeField] private float defaultPosY; // Initial Y position
    public float duration = 0.5f; // Animation duration

    [Header("Behavior Settings")]
    public bool isEnabled; // Determines if the object should be disabled after animation

    private void OnEnable()
    {
        if (!ValidateTargetObject()) return;
        AnimatePosition(moveDistance);
    }

    private void OnDisable()
    {
        if (!ValidateTargetObject()) return;
        AnimatePosition(defaultPosY, () =>
        {
            if (isEnabled)
            {
                gameObject.SetActive(false);
            }
        });
    }

    private void OnDestroy()
    {
        if (targetObject != null)
        {
            DOTween.Kill(targetObject); // Only kill animations related to targetObject
        }
    }

    private bool ValidateTargetObject()
    {
        if (targetObject == null)
        {
            Debug.LogWarning($"{nameof(targetObject)} is not assigned in {nameof(UITap)} script.", this);
            return false;
        }
        return true;
    }

    private void AnimatePosition(float targetY, TweenCallback onComplete = null)
    {
        targetObject.DOAnchorPosY(targetY, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(onComplete);
    }
}

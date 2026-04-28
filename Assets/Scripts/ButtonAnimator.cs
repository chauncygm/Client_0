using UnityEngine;
using GameBase;
using DG.Tweening;

public class ButtonAnimator : MonoBehaviour
{
    [SerializeField] private float pressScale = 0.9f;
    [SerializeField] private float duration = 0.1f;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
        
        // 自动绑定事件，无需在 Inspector 中配置
        var listener = EventTriggerListener.Get(gameObject);
        listener.OnDown = (_, _) => Press();
        listener.OnUp = (_, _) => Release();
    }

    private void Press()
    {
        // 按下缩小（平滑动画）
        transform.DOKill();
        transform.DOScale(_originalScale * pressScale, duration)
                 .SetEase(Ease.OutBack);
    }

    private void Release()
    {
        // 抬起恢复（平滑动画）
        transform.DOKill();
        transform.DOScale(_originalScale, duration)
                 .SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        transform.DOKill(); // 清理 DOTween
    }
}
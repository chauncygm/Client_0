using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    public class SplashUI : UIBase
    {
        
        [SerializeField]
        public RawImage splashImage;
        
        // public override void OnEnter(object param)
        // {
        //     base.OnEnter(param);
        //     splashImage.color = new Color(1, 1, 1, 0);
        //     
        //     Debug.Log($"Splash image: {splashImage.texture}");
        //     var sequence = DOTween.Sequence();
        //     sequence.Append(splashImage.DOFade(1, 2f).SetEase(Ease.OutSine));
        //     sequence.Append(splashImage.DOFade(0, 2.5f).SetEase(Ease.InSine));
        //     sequence.OnComplete(() =>
        //     {
        //         Debug.Log($"Close Splash : {splashImage.name}");
        //         UILoadMgr.Hide(UIDefine.UISplash);
        //         ((Action)param)?.Invoke();
        //     });
        //     
        // }

        public override void OnEnter(object param)
        {
            base.OnEnter(param);
            splashImage.color = new Color(1, 1, 1, 0);
            
            Debug.Log($"Splash image: {splashImage.texture}");
            
            // 使用协程替代DOTween序列
            StartCoroutine(PlaySplashSequence((Action)param));
        }
        
        private System.Collections.IEnumerator PlaySplashSequence(Action onComplete)
        {
            // 淡入效果 (2秒)
            yield return StartCoroutine(FadeImage(0, 1, 2f));
            
            // 淡出效果 (2.5秒)
            yield return StartCoroutine(FadeImage(1, 0, 2.5f));
            
            Debug.Log($"Close Splash : {splashImage.name}");
            UILoadMgr.Hide(UIDefine.UISplash);
            onComplete?.Invoke();
        }
        
        private System.Collections.IEnumerator FadeImage(float fromAlpha, float toAlpha, float duration)
        {
            var elapsed = 0f;
            var startColor = splashImage.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
                splashImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
            
            // 确保最终值准确
            splashImage.color = new Color(startColor.r, startColor.g, startColor.b, toAlpha);
        }

    }
}
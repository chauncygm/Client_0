using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Scripts.UI
{
    public class SplashUI : UIBase
    {
        
        [SerializeField]
        public RawImage splashImage;
        
        public override void OnEnter(object param)
        {
            base.OnEnter(param);
            splashImage.color = new Color(1, 1, 1, 0);
            
            Debug.Log($"Splash image: {splashImage.texture}");
            var sequence = DOTween.Sequence();
            sequence.Append(splashImage.DOFade(1, 2f).SetEase(Ease.OutSine));
            sequence.Append(splashImage.DOFade(0, 2.5f).SetEase(Ease.InSine));
            sequence.OnComplete(() =>
            {
                Debug.Log($"Close Splash : {splashImage.name}");
                UILoadMgr.Hide(UIDefine.UISplash);
                ((Action)param)?.Invoke();
            });
            
        }

    }
}
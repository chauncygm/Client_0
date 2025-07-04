using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.UI
{
    public class SplashPanel : UIFormLogic
    {

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            var rawImage = GetComponentInChildren<RawImage>(true);
            rawImage.color = new Color(1, 1, 1, 0);
            
            Debug.Log($"Splash image: {rawImage.texture}");
            var sequence = DOTween.Sequence();
            sequence.Append(rawImage.DOFade(1, 2f).SetEase(Ease.OutSine));
            sequence.Append(rawImage.DOFade(0, 2.5f).SetEase(Ease.InSine));
            sequence.OnComplete(() =>
            {
                Debug.Log($"Close Splash : {rawImage.name}");
                Base.GameEntry.UI.CloseUIForm(UIForm);
            });
        }

    }
}
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMain
{
    public class UISplash : UIBase
    {
        public override string Name()
        {
            return UIDefine.UISplash;
        }

        public override void OnEnter(params object[] param)
        {
            var onPlayComplete = ((Action)param[0]);
            var anim = GetComponent<Animation>();
            if (anim && anim.Play())
            {
                WaitForAnimationComplete(anim, onPlayComplete).Forget();
            }
            else
            {
                onPlayComplete?.Invoke();
            }
        }

        private static async UniTaskVoid WaitForAnimationComplete(Animation anim, Action onPlayComplete)
        {
            await UniTask.WaitForSeconds(anim.clip.length);
            UILoadMgr.Hide(UIDefine.UISplash);
            onPlayComplete?.Invoke();
        }
    }
}
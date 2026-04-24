using UnityEngine.UI;
using System;

namespace GameMain
{
    public enum MessageShowType
    {
        Tips = 0,
        OkOrCancel = 1,
        RetryOrQuitTips = 2,
        Quit = 3,
    }

    public class UILoadTip : UIBase
    {
        public Text labelDesc;
        
        public Button btnLeft;
        public Button btnCenter;
        public Button btnRight;
        public Text btnLeftText;
        public Text btnCenterText;
        public Text btnRightText;

        private Action _onOk;
        private Action _onCancel;
        public MessageShowType showType;

        private void OnEnable()
        {
            btnLeft.onClick.AddListener(OnGameUpdate);
            btnCenter.onClick.AddListener(OnGameUpdate);
            btnRight.onClick.AddListener(OnGameIgnore);
        }

        public override string Name()
        {
            return UIDefine.UILoadTip;
        }

        public override void OnEnter(params object[] data)
        {
            if (data.Length < 4)
            {
                throw new ArgumentException("params length must be more than 4");
            }
            labelDesc.text = data[0].ToString();
            showType = (MessageShowType)data[1];
            _onOk = (Action)data[2];
            _onCancel = (Action)data[3];
            
            btnRight.gameObject.SetActive(false);
            btnCenter.gameObject.SetActive(false);
            btnLeft.gameObject.SetActive(false);
            
            switch (showType)
            {
                case MessageShowType.Tips:
                    btnCenterText.text = "确定";
                    btnCenter.gameObject.SetActive(true);
                    break;
                case MessageShowType.OkOrCancel:
                    btnLeftText.text = "确定";
                    btnRightText.text = "取消";
                    btnLeft.gameObject.SetActive(true);
                    btnRight.gameObject.SetActive(true);
                    break;
                case MessageShowType.RetryOrQuitTips:
                    btnLeftText.text = "重试";
                    btnRightText.text = "退出";
                    btnLeft.gameObject.SetActive(true);
                    btnRight.gameObject.SetActive(true);
                    break;
                case MessageShowType.Quit: 
                    btnCenterText.text = "退出";
                    btnCenter.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnGameUpdate()
        {
            _onOk?.Invoke();
            UILoadMgr.Hide(UIDefine.UILoadTip);
        }

        private void OnGameIgnore()
        {
            _onCancel?.Invoke();
            UILoadMgr.Hide(UIDefine.UILoadTip);
        }

        private void OnDisable()
        {
            btnLeft.onClick.RemoveAllListeners();
            btnCenter.onClick.RemoveAllListeners();
            btnRight.onClick.RemoveAllListeners();
        }
    }
}
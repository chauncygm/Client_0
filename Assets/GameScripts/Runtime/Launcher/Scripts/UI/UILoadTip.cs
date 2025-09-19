using UnityEngine.UI;
using System;
using UnityEngine.Serialization;

namespace GameMain
{
    public enum MessageShowType
    {
        None = 0,
        OneButton = 1,
        TwoButton = 2,
        ThreeButton = 3,
    }

    public class UILoadTip : UIBase
    {
        public Button btnUpdate;
        public Button btnIgnore;
        public Button btnPackage;
        public Text labelDesc;

        private Action _onOk;
        private Action _onCancel;
        public MessageShowType showType = MessageShowType.None;

        void Start()
        {
            btnUpdate.onClick.AddListener(OnGameUpdate);
            btnIgnore.onClick.AddListener(OnGameIgnore);
            btnPackage.onClick.AddListener(OnInvoke);
        }

        public override void OnEnter(object data)
        {
            btnIgnore.gameObject.SetActive(false);
            btnPackage.gameObject.SetActive(false);
            btnUpdate.gameObject.SetActive(false);
            switch (showType)
            {
                case MessageShowType.OneButton:
                    btnUpdate.gameObject.SetActive(true);
                    break;
                case MessageShowType.TwoButton:
                    btnUpdate.gameObject.SetActive(true);
                    btnIgnore.gameObject.SetActive(true);
                    break;
                case MessageShowType.ThreeButton:
                    btnIgnore.gameObject.SetActive(true);
                    btnPackage.gameObject.SetActive(true);
                    btnUpdate.gameObject.SetActive(true);
                    break;
                case MessageShowType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            labelDesc.text = data.ToString();
        }

        private void OnGameUpdate()
        {
            if (_onOk == null)
            {
                labelDesc.text = "<color=#BA3026>该按钮不应该存在</color>";
            }
            else
            {
                _onOk();
                _OnClose();
            }
        }

        private void OnGameIgnore()
        {
            if (_onCancel == null)
            {
                labelDesc.text = "<color=#BA3026>该按钮不应该存在</color>";
            }
            else
            {
                _onCancel();
                _OnClose();
            }
        }

        private void OnInvoke()
        {
            if (_onOk == null)
            {
                labelDesc.text = "<color=#BA3026>该按钮不应该存在</color>";
            }
            else
            {
                _onOk();
                _OnClose();
            }
        }

        private void _OnClose()
        {
            UILoadMgr.Hide(UIDefine.UILoadTip);
        }

        /// <summary>
        /// 显示提示框，目前最多支持三个按钮
        /// </summary>
        /// <param name="desc">描述</param>
        /// <param name="showType">类型（MessageShowType）</param>
        /// <param name="style">StyleEnum</param>
        /// <param name="onOk">点击事件</param>
        /// <param name="onCancel">取消事件</param>
        /// <param name="onPackage">更新事件</param>
        public static void ShowMessageBox(string desc, 
            MessageShowType showType = MessageShowType.OneButton,
            LoadStyle.StyleEnum style = LoadStyle.StyleEnum.Style_Default,
            Action onOk = null,
            Action onCancel = null,
            Action onPackage = null)
        {
            UILoadMgr.Show(UIDefine.UILoadTip, desc);
            var ui = UILoadMgr.GetActiveUI(UIDefine.UILoadTip) as UILoadTip;
            if (!ui) return;
            ui._onOk = onOk;
            ui._onCancel = onCancel;
            ui.showType = showType;
            ui.OnEnter(desc);

            var loadStyleUI = ui.GetComponent<LoadStyle>();
            if (loadStyleUI)
            {
                loadStyleUI.SetStyle(style);
            }
        }
    }
}
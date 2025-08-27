using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public abstract class UIWidget : UIBase
    {
        /// <summary>
        /// 窗口组件的实例资源对象。
        /// </summary>
        public override GameObject GameObject { protected set; get; }

        /// <summary>
        /// 窗口组件矩阵位置组件。
        /// </summary>
        public override RectTransform RectTransform { protected set; get; }
        
        /// <summary>
        /// 窗口位置组件。
        /// </summary>
        public override Transform Transform { protected set; get; }

        /// <summary>
        /// 窗口组件名称。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string name { protected set; get; } = string.Empty;

        /// <summary>
        /// UI类型。
        /// </summary>
        public override UIType Type => UIType.Widget;

        /// <summary>
        /// 所属的窗口。
        /// </summary>
        public UIWindow OwnerWindow
        {
            get
            {
                var parentUI = Parent;
                while (parentUI != null)
                {
                    if (parentUI.Type == UIType.Window)
                    {
                        return parentUI as UIWindow;
                    }

                    parentUI = parentUI.Parent;
                }

                return null;
            }
        }
        
        /// <summary>
        /// 窗口可见性
        /// </summary>
        public bool Visible
        {
            get => GameObject.activeSelf;

            set
            {
                GameObject.SetActive(value);
                OnSetVisible(value);
            }
        }

        internal bool InternalUpdate()
        {
            if (!IsPrepare)
            {
                return false;
            }

            List<UIWidget> listNextUpdateChild = null;
            if (ListChild is { Count: > 0 })
            {
                listNextUpdateChild = ListUpdateChild;
                var updateListValid = UpdateListValid;
                List<UIWidget> listChild;
                if (!updateListValid)
                {
                    if (listNextUpdateChild == null)
                    {
                        listNextUpdateChild = new List<UIWidget>();
                        ListUpdateChild = listNextUpdateChild;
                    }
                    else
                    {
                        listNextUpdateChild.Clear();
                    }

                    listChild = ListChild;
                }
                else
                {
                    listChild = listNextUpdateChild;
                }

                foreach (var uiWidget in listChild.Where(uiWidget => uiWidget != null))
                {
                    TProfiler.BeginSample(uiWidget.name);
                    var needValid = uiWidget.InternalUpdate();
                    TProfiler.EndSample();

                    if (!updateListValid && needValid)
                    {
                        listNextUpdateChild.Add(uiWidget);
                    }
                }

                if (!updateListValid)
                {
                    UpdateListValid = true;
                }
            }

            TProfiler.BeginSample("OnUpdate");
            bool needUpdate;
            if (listNextUpdateChild is not { Count: > 0 })
            {
                HasOverrideUpdate = true;
                OnUpdate();
                needUpdate = HasOverrideUpdate;
            }
            else
            {
                OnUpdate();
                needUpdate = true;
            }
            TProfiler.EndSample();

            return needUpdate;
        }

        #region Create

        /// <summary>
        /// 创建窗口内嵌的界面。
        /// </summary>
        /// <param name="parentUI">父节点UI。</param>
        /// <param name="widgetRoot">组件根节点。</param>
        /// <param name="visible">是否可见。</param>
        /// <returns></returns>
        public bool Create(UIBase parentUI, GameObject widgetRoot, bool visible = true)
        {
            return CreateImp(parentUI, widgetRoot, false, visible);
        }

        /// <summary>
        /// 根据资源名创建
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="parentUI"></param>
        /// <param name="parentTrans"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public bool CreateByPath(string resPath, UIBase parentUI, Transform parentTrans = null, bool visible = true)
        {
            var goInst = GameModule.Resource.LoadGameObject(resPath, parent: parentTrans);
            if (goInst == null)
            {
                return false;
            }

            if (!Create(parentUI, goInst, visible))
            {
                return false;
            }

            goInst.transform.localScale = Vector3.one;
            goInst.transform.localPosition = Vector3.zero;
            return true;
        }

        /// <summary>
        /// 根据prefab或者模版来创建新的 widget。
        /// <remarks>存在父物体得资源故不需要异步加载。</remarks>
        /// </summary>
        /// <param name="parentUI">父物体UI。</param>
        /// <param name="goPrefab">实例化预制体。</param>
        /// <param name="parentTrans">实例化父节点。</param>
        /// <param name="visible">是否可见。</param>
        /// <returns>是否创建成功。</returns>
        public bool CreateByPrefab(UIBase parentUI, GameObject goPrefab, Transform parentTrans, bool visible = true)
        {
            if (parentTrans == null)
            {
                parentTrans = parentUI.RectTransform;
            }

            return CreateImp(parentUI, Object.Instantiate(goPrefab, parentTrans), true, visible);
        }

        private bool CreateImp(UIBase parentUI, GameObject widgetRoot, bool bindGo, bool visible = true)
        {
            if (!CreateBase(widgetRoot, bindGo))
            {
                return false;
            }

            RestChildCanvas(parentUI);
            Parent = parentUI;
            Parent.ListChild.Add(this);
            Parent.SetUpdateDirty();
            ScriptGenerator();
            BindMemberProperty();
            RegisterEvent();
            OnCreate();
            OnRefresh();
            IsPrepare = true;

            if (!visible)
            {
                GameObject.SetActive(false);
            }
            else
            {
                if (!GameObject.activeSelf)
                {
                    GameObject.SetActive(true);
                }
            }

            return true;
        }

        protected bool CreateBase(GameObject go, bool bindGo)
        {
            if (go == null)
            {
                return false;
            }

            name = GetType().Name;
            Transform = go.GetComponent<Transform>();
            RectTransform = Transform as RectTransform;
            GameObject = go;
            Log.Assert(RectTransform != null, $"{go.name} ui base element need to be RectTransform");
            return true;
        }

        protected void RestChildCanvas(UIBase parentUI)
        {
            if (parentUI == null || parentUI.GameObject == null)
            {
                return;
            }

            var parentCanvas = parentUI.GameObject.GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                return;
            }

            if (GameObject == null) return;
            
            var listCanvas = GameObject.GetComponentsInChildren<Canvas>(true);
            foreach (var childCanvas in listCanvas)
            {
                childCanvas.sortingOrder = parentCanvas.sortingOrder + childCanvas.sortingOrder % UISystem.WindowDeep;
            }
        }

        #endregion

        #region Destroy

        /// <summary>
        /// 组件被销毁调用。
        /// <remarks>请勿手动调用！</remarks>
        /// </summary>
        internal void OnDestroyWidget()
        {
            Parent?.SetUpdateDirty();
            
            RemoveAllUIEvent();

            foreach (var uiChild in ListChild)
            {
                uiChild.OnDestroy();
                uiChild.OnDestroyWidget();
            }

            if (GameObject != null)
            {
                Object.Destroy(GameObject);
            }
        }

        /// <summary>
        /// 主动销毁组件。
        /// </summary>
        public void Destroy()
        {
            if (Parent == null) return;
            
            Parent.ListChild.Remove(this);
            OnDestroy();
            OnDestroyWidget();
        }

        #endregion
    }
}
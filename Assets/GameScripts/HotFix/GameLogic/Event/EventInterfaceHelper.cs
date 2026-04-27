using System;
using GameBase;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventInterfaceImpAttribute : BaseAttribute
    {
        public EEventGroup EventGroup { get; }

        public EventInterfaceImpAttribute(EEventGroup group)
        {
            EventGroup = group;
        }
    }

    public class EventInterfaceHelper
    {
        public static void Init()
        {
            RegisterEventInterface_Logic.Register(GameEvent.EventMgr);
            RegisterEventInterface_UI.Register(GameEvent.EventMgr);
        }
    }
}
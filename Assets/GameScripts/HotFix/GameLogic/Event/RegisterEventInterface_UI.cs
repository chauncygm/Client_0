using System;
using GameBase;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    class RegisterEventInterface_UI
    {
        public static void Register(EventMgr mgr)
        {
            var dispatcher = mgr.Dispatcher;

            var types = CodeTypes.Instance.GetTypes(typeof(EventInterfaceImpAttribute));

            foreach (Type type in types)
            {
                var attrs = type.GetCustomAttributes(typeof(EventInterfaceImpAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                EventInterfaceImpAttribute httpHandlerAttribute = (EventInterfaceImpAttribute)attrs[0];

                if (httpHandlerAttribute.EventGroup != EEventGroup.GroupUI)
                {
                    continue;
                }

                var obj = Activator.CreateInstance(type, dispatcher);

                mgr.RegWrapInterface(obj.GetType().GetInterfaces()[0]?.FullName, obj);
            }
        }
    }
}
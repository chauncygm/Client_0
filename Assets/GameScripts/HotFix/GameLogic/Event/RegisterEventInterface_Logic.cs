using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    class RegisterEventInterface_Logic
    {
        public static void Register(EventMgr mgr)
        {
            var dispatcher = mgr.Dispatcher;

            var types = CodeTypes.Instance.GetTypes(typeof(EventInterfaceImpAttribute));

            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes(typeof(EventInterfaceImpAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                var httpHandlerAttribute = (EventInterfaceImpAttribute)attrs[0];

                if (httpHandlerAttribute.EventGroup != EEventGroup.GroupLogic)
                {
                    continue;
                }

                var obj = Activator.CreateInstance(type, dispatcher);
                mgr.RegWrapInterface(obj.GetType().GetInterfaces()[0].FullName, obj);
            }
        }
    }
}
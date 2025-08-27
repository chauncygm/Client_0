using UnityGameFramework.Runtime;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupLogic)]
    interface IBagLogicEvent
    {
        void OnResourceChange(int resourceId, int num);
    }
}
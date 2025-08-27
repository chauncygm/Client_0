using System;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupLogic)]
    interface IActorLogicEvent
    {
        void OnMainPlayerNameChange();

        void OnMainPlayerLevelChange();

        void OnMainPlayerLoginSuccess();
    }
}
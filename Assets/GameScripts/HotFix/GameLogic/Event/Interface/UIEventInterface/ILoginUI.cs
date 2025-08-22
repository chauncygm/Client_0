using UnityGameFramework.Runtime;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupUI)]
    public interface ILoginUI
    {
        public void OnRoleLogin(long uid);

        public void OnRoleLoginOut();
    }
}
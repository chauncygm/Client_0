using UnityGameFramework.Runtime;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupUI)]
    public interface ILoginUI
    {
        public void OnRoleLogin(long uid);

        public void OnSelectServer(string serverName);

        public void OnRoleLoginOut();
    }
}
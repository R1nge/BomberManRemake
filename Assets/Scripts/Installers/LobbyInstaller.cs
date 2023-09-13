using Lobby;
using Zenject;

namespace Installers
{
    public class LobbyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Lobby.Lobby>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PowerupSelection>().FromNew().AsSingle();
        }
    }
}
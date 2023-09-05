using Zenject;

namespace Installers
{
    public class LobbyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Lobby.Lobby>().FromComponentsInHierarchy().AsSingle();
        }
    }
}
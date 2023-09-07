using Game;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<SpawnerOnGrid>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PlayerSpawnerFPS>().FromComponentInHierarchy().AsSingle();
            Container.Bind<RoundManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
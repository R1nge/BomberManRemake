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
            Container.Bind<PlayerSpawner>().FromComponentInHierarchy().AsSingle();
            Container.Bind<RoundManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
using Game;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateControllerView>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<SpawnerOnGrid>().FromComponentsInHierarchy().AsSingle();
        }
    }
}
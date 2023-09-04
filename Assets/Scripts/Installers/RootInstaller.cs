using Game;
using Zenject;

namespace Installers
{
    public class RootInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MapSettings>().FromNew().AsSingle();
        }
    }
}
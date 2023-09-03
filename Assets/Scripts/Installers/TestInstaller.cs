using Zenject;

namespace Installers
{
    public class TestInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<TestSingleton>().FromNew().AsSingle();
        }
    }
}
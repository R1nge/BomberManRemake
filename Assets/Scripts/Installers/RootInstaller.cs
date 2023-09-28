using Game;
using Misc;
using Skins.Bombs;
using Skins.Players;
using Zenject;

namespace Installers
{
    public class RootInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameSettings>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayFabManager>().FromNew().AsSingle();
            Container.Bind<Wallet>().FromNew().AsSingle();
            Container.Bind<SkinManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<BombSkinManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<SaveManager>().FromNew().AsSingle();
            Container.Bind<ExitGame>().FromNew().AsSingle();
        }
    }
}
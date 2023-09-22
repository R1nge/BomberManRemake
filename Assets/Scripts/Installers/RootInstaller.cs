﻿using Game;
using Misc;
using Skins;
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
            Container.Bind<PlayFabManager>().FromNew().AsSingle();
            Container.Bind<SaveManager>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<Wallet>().FromNew().AsSingle();
            Container.Bind<SkinManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<BombSkinManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
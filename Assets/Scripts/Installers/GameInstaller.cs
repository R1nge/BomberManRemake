﻿using Game;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateController>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<SpawnerOnGrid>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<PlayerSpawner>().FromComponentInHierarchy().AsSingle();
        }
    }
}
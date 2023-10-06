﻿using Game;
using Game.StateMachines;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<SpawnerOnGrid>().FromComponentInHierarchy().AsSingle();
            
            //I can safely get rid of them 
            
            //
            Container.Bind<PlayerSpawnerFPS>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PlayerSpawnerTPS>().FromComponentInHierarchy().AsSingle();
            //
            Container.Bind<SpawnerManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<MapSelector>().FromComponentInHierarchy().AsSingle();
            Container.Bind<NetworkObjectPool>().FromComponentInHierarchy().AsSingle();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Platform Spawner")]
    [SerializeField] private PlatformGenerationSettings platformSettings;
    [SerializeField] private Transform platformRoot;

    [Header("Levels")]
    [SerializeField] List<LevelDifficultyConfig> levelConfigs;
    public override void InstallBindings()
    {
        //GameLoop
        Container.BindInterfacesAndSelfTo<GameService>().FromComponentInHierarchy().AsSingle().NonLazy();

        //Camera
        Container.BindInterfacesAndSelfTo<CameraService>().FromComponentInHierarchy().AsSingle().NonLazy();

        //UI
        Container.BindInterfacesTo<UIService>().FromComponentInHierarchy().AsSingle().NonLazy();

        // Input
        Container.BindInterfacesTo<InputService>().AsSingle().NonLazy();

        /* Level difficulty */
        Container.Bind<ILevelDifficultyService>().To<LevelDifficultyService>().AsSingle();
        Container.Bind<List<LevelDifficultyConfig>>().FromInstance(levelConfigs).AsSingle();


        // Platform Generator
        Container.Bind<Transform>().WithId("PlatformRoot").FromInstance(platformRoot).AsSingle();
        Container.Bind<PlatformGenerationSettings>().FromInstance(platformSettings).AsSingle();
        Container.Bind<IPlatformGenerator>().To<PlatformGeneratorService>().AsSingle();

       

        











    }
}
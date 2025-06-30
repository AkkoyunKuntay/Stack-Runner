using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private PlatformGenerationSettings platformSettings;
    [SerializeField] private Transform platformRoot;
    public override void InstallBindings()
    {
        // Input
        Container.Bind<IInputService>().To<InputService>().AsSingle();

        // Platform Generator
        Container.Bind<Transform>().WithId("PlatformRoot").FromInstance(platformRoot).AsSingle();
        Container.Bind<PlatformGenerationSettings>().FromInstance(platformSettings).AsSingle();
        Container.Bind<IPlatformGenerator>().To<PlatformGeneratorService>().AsSingle();



    }
}
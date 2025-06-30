using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Input
        Container.Bind<IInputService>().To<InputService>().AsSingle();

       
    }
}
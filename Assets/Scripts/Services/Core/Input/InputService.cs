using UnityEngine;
using Zenject;

public interface IInputService
{
    bool IsTapDown { get; }
   

}

public class InputService : IInputService,IInitializable
{
    public bool IsTapDown => Input.GetMouseButtonDown(0);
    public void Initialize() { Debug.Log("[InputService] has been initialized."); }

}

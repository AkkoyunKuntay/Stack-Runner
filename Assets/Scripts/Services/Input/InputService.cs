using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputService
{
    bool IsTapDown { get; }
    public void Initialize() { Debug.Log("[InputService] has been initialized."); }

}

public class InputService : IInputService
{
    public bool IsTapDown => Input.GetMouseButtonDown(0);

    
}

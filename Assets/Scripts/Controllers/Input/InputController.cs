using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InputController : MonoBehaviour
{
    
    [Inject] IInputService _inputInputService;

    private void Awake()
    {
        _inputInputService.Initialize();
    }
    // Update is called once per frame
    void Update()
    {
        if(_inputInputService.IsTapDown)
        {
            Debug.Log("[Input Given] Tap Down !");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InputController : MonoBehaviour
{
    
    [Inject] IInputService _inputInputService;
    [Inject] IPlatformGeneratorService _platformGenerator;

    private void Awake()
    {

    }
    // Update is called once per frame
    void Update()
    {
       
    }
}

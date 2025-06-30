using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlatformSpawner : MonoBehaviour
{
    [Inject] IPlatformGenerator _platformGenerator;
    [Inject] IInputService _inputService;

    // Start is called before the first frame update
    void Awake()
    {
        _platformGenerator.Initialize();
        _platformGenerator.SpawnFirst();
    }

    // Update is called once per frame
    void Update()
    {
        if (_inputService.IsTapDown)
        {
            _platformGenerator.SpawnNext(Random.value > .5f);
        }
    }
}

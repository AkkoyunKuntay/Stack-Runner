using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPlatformGenerator
{
    public void Initialize() { Debug.Log("[PlatformGeneratorService] has been initialized."); }
    PlatformView SpawnFirst();
    PlatformView SpawnNext(bool spawnRight);
    bool IsPerfectCut(float delta);
    float CurrentDirectionSign { get; }   // +1 = right, -1 = left
}
public class PlatformGeneratorService : IPlatformGenerator
{
    private readonly Transform _root;               // platform root reference
    private readonly Queue<PlatformView> _pool = new();
    private readonly PlatformGenerationSettings _settings;
    private readonly DiContainer _container;   // to instantiate prefab with zenject

    private PlatformView _lastPlatform;
    private int _dirSign = 1; // +1 => right to left , -1 => left to right

    [Inject]
    public PlatformGeneratorService(PlatformGenerationSettings settings, DiContainer container, [Inject(Id = "PlatformRoot")] Transform root)
    {
        _container = container;
        _settings = settings;
        _root = root;
        PrewarmPool();
    }

    public PlatformView SpawnFirst()
    {
        Vector3 pos = _root.position;
        _lastPlatform = GetFromPool();
        _lastPlatform.Init(_settings.platformWidth, _settings.platformDepth, pos, true);

        return _lastPlatform;
    }

    public PlatformView SpawnNext(bool spawnRight)
    {
        _dirSign = spawnRight ? 1 : -1;

        // ① X, köke göre sabit ±offset
        float x = _root.position.x + _dirSign * _settings.lateralOffset;

        // ② Z, bir öncekinin derinliği kadar ileri
        float z = _lastPlatform.transform.position.z + _settings.platformDepth;

        Vector3 spawnPos = new Vector3(x, _root.position.y, z);

        _lastPlatform = GetFromPool();
        _lastPlatform.Init(_settings.platformWidth,
                   _settings.platformDepth,
                   spawnPos, spawnRight);
        return _lastPlatform;
    }

    public bool IsPerfectCut(float delta) =>
        Mathf.Abs(delta) <= _settings.perfectTolerance;

    public float CurrentDirectionSign => _dirSign;


    #region ObjectPool Helpers
    private void PrewarmPool()
    {
        for (int i = 0; i < _settings.initialPoolSize; i++)
            ReturnToPool(CreateNew());
    }

    private PlatformView GetFromPool()
    {
        PlatformView view = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        view.gameObject.SetActive(true);
        return view;
    }


    private void ReturnToPool(PlatformView platform)
    {
        platform.gameObject.SetActive(false);
        _pool.Enqueue(platform);
    }

    private PlatformView CreateNew()
    {
        var obj = _container.InstantiatePrefab(_settings.platformPrefab);
        obj.SetActive(false);
        return obj.GetComponent<PlatformView>();
    }

    #endregion
}


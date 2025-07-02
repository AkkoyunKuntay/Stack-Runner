using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPlatformGenerator
{
    PlatformView SpawnFirst();                              
    PlatformView SpawnNext(bool spawnRight, float width);  
    void SpawnFinal(float width);                   
    bool IsPerfectCut(float delta);
    int CurrentDirectionSign { get; }
    Transform FinalTransform { get; }
    void ResetForNewLevel(PlatformView newBase);          
}

public class PlatformGeneratorService : IPlatformGenerator
{
    [Header("DI")]
    readonly PlatformGenerationSettings _platformGenerationSettings;
    readonly DiContainer _container;
    readonly Transform _root;
    readonly ILevelDifficultyService _levelService;

    [Header("Platform Pool")]
    readonly Queue<PlatformView> _pool = new();
    PlatformView _lastPlatform;
    int _dirSign = 1;
    int _spawned = 0;

    PlatformView _finalPlatform;
    public PlatformView FinalPlatform => _finalPlatform;
    public Transform FinalTransform => _finalPlatform ? _finalPlatform.transform : null;
    


    [Inject]
    public PlatformGeneratorService(
        PlatformGenerationSettings settings,
        DiContainer container,
        [Inject(Id = "PlatformRoot")] Transform root,
        ILevelDifficultyService difficulty)
    {
        _platformGenerationSettings = settings;
        _container = container;
        _root = root;
        _levelService = difficulty;

        Prewarm();
        Debug.Log("[PlatformGenerator] has been initialized.");
    }

    public void ResetForNewLevel(PlatformView newBase)        // ★ EKLE
    {
        _lastPlatform = newBase;     // bundan sonra SpawnNext bunun üzerinden hesaplar
        _spawned = 1;           // base sayıldı
    }
    public PlatformView SpawnFirst()
    {
        _spawned = 1;

        _lastPlatform = InstantiatePrefab(_platformGenerationSettings.startPrefab.gameObject, _root.position);
        _lastPlatform.Init(_platformGenerationSettings.platformWidth, _platformGenerationSettings.platformDepth, _root.position, true);
        return _lastPlatform;
    }

    public PlatformView SpawnNext(bool spawnRight, float width)
    {
        _spawned++;

        _dirSign = spawnRight ? 1 : -1;
        float x = _root.position.x + _dirSign * _platformGenerationSettings.lateralOffset;
        float z = _lastPlatform.transform.position.z + _platformGenerationSettings.platformDepth;
        Vector3 desiredPos = new(x, _root.position.y, z);

        _lastPlatform = GetFromPool();
        _lastPlatform.Init(width, _platformGenerationSettings.platformDepth, desiredPos, spawnRight);
        return _lastPlatform;
    }

    public void SpawnFinal(float width)
    {
        float z = _root.position.z + _platformGenerationSettings.platformDepth *
                  _levelService.NormalPlatformCount;

        Vector3 desiredPos = new(_root.position.x, _root.position.y, z);

        _finalPlatform = InstantiatePrefab(_platformGenerationSettings.finalPrefab.gameObject, desiredPos);

        _finalPlatform.Init(width, _platformGenerationSettings.platformDepth, desiredPos, true);

        _finalPlatform.isFinal = true;
        
    }

    public int CurrentDirectionSign => _dirSign;
    public bool IsPerfectCut(float d) => Mathf.Abs(d) <= _platformGenerationSettings.perfectTolerance;

    PlatformView InstantiatePrefab(GameObject prefab, Vector3 pos)
    {
        var go = _container.InstantiatePrefab(prefab, pos, Quaternion.identity, _root);
        return go.GetComponent<PlatformView>();
    }

    void Prewarm()
    {
        for (int i = 0; i < _platformGenerationSettings.initialPoolSize; i++)
            ReturnToPool(Create());
    }
    PlatformView GetFromPool()
    {
        var v = _pool.Count > 0 ? _pool.Dequeue() : Create();
        v.gameObject.SetActive(true);
        return v;
    }
    void ReturnToPool(PlatformView v) { v.gameObject.SetActive(false); _pool.Enqueue(v); }
    PlatformView Create()
    {
        var go = _container.InstantiatePrefab(_platformGenerationSettings.platformPrefab.gameObject, _root);
        go.SetActive(false);
        return go.GetComponent<PlatformView>();
    }
}

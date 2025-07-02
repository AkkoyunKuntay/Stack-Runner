using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPlatformGeneratorService
{
    PlatformView SpawnFirstPlatform();                             
    PlatformView SpawnNextPlatform(bool spawnRight, float width);  
    PlatformView SpawnFinalPlatform(float width);                  
    void ResetForNewLevel(PlatformView newBase);          

    Transform FinalTransform { get; }                              
    int CurrentDirectionSign { get; }                       
    bool IsPerfectCut(float delta);                           
}

public class PlatformGeneratorService : IPlatformGeneratorService
{
    [Header("DI")]
    readonly PlatformGenerationSettings _s;
    readonly Transform _root;        
    readonly DiContainer _c;
    readonly ILevelDifficultyService _d;

    [Header("Object Pool")]
    readonly Queue<PlatformView> _pool = new();
    PlatformView _last;                
    PlatformView _final;               
    int _dir = 1;                      
    int _spawned = 0;

    #region Constructor
    [Inject]
    public PlatformGeneratorService(
        PlatformGenerationSettings settings,
        [Inject(Id = "PlatformRoot")] Transform root,
        [Inject] DiContainer container,
        ILevelDifficultyService difficulty)
    {
        _s = settings;
        _root = root;
        _c = container;
        _d = difficulty;

        PrewarmPool();
        Debug.Log("[PlatformGeneratorService] has been initialized.");
    }
    #endregion
 
    public PlatformView SpawnFirstPlatform()
    {
        _spawned = 1;
        _last = InstantiatePrefab(_s.startPrefab.gameObject, Vector3.zero);
        _last.Init(_s.platformWidth, _s.platformDepth, Vector3.zero, true);
        return _last;
    }

    public PlatformView SpawnNextPlatform(bool spawnRight, float width)
    {
        _spawned++;
        _dir = spawnRight ? 1 : -1;

        float x = _root.position.x + _dir * _s.lateralOffset;
        float z = _last.transform.position.z + _s.platformDepth;
        Vector3 pos = new(x, _root.position.y, z);

        _last = GetFromPool();
        _last.Init(width, _s.platformDepth, pos, spawnRight);
        return _last;
    }

    public PlatformView SpawnFinalPlatform(float width)
    {
        float z = _last.transform.position.z + _s.platformDepth * _d.NormalPlatformCount;

        Vector3 pos = new(_root.position.x, _root.position.y, z);

        _final = InstantiatePrefab(_s.finalPrefab.gameObject, pos);
        _final.isFinal = true;
        _final.Init(width, _s.platformDepth, pos, true);

        return _final;
    }

    public void ResetForNewLevel(PlatformView newBase)
    {
        _last = newBase;
        _spawned = 1;
    }

    public Transform FinalTransform => _final ? _final.transform : null;
    public int CurrentDirectionSign => _dir;
    public bool IsPerfectCut(float d) => Mathf.Abs(d) <= _s.perfectTolerance;

    #region ObjectPooling
    void PrewarmPool()
    {
        for (int i = 0; i < _s.initialPoolSize; i++)
            ReturnToPool(CreateNew());
    }

    PlatformView GetFromPool()
    {
        var v = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        v.gameObject.SetActive(true);
        return v;
    }

    void ReturnToPool(PlatformView v)
    {
        v.gameObject.SetActive(false);
        _pool.Enqueue(v);
    }

    PlatformView CreateNew() => InstantiatePrefab(_s.platformPrefab.gameObject, _root.position);

    PlatformView InstantiatePrefab(GameObject prefab, Vector3 pos)
    {
        var go = _c.InstantiatePrefab(prefab, pos, Quaternion.identity, _root);
        return go.GetComponent<PlatformView>();
    }
    #endregion
}
using System;
using UnityEngine;
using Zenject;

public class PlatformController : MonoBehaviour
{
    [Inject] IPlatformGenerator _platformGeneratorService;
    [Inject] IInputService _inputService;
    [Inject] IGameFlowService _gameService;
    [Inject] ICameraService _cameraService;
    [Inject] ILevelDifficultyService _levelService;

    PlatformView basePlatform;
    PlatformView currentPlatform;
    int cutsDone = 0;

    public static event Action<Transform> OnBasePlatformChanged;


    void Awake() => StartLevel();
    void OnEnable() => _levelService.OnLevelAdvanced += StartLevel;
    void OnDisable() => _levelService.OnLevelAdvanced -= StartLevel;

    void StartLevel()
    {
        ClearAll();

        // ❶ önceki bölümün finali sahnede mi?
        if (_platformGeneratorService.FinalTransform != null &&
            _platformGeneratorService.FinalTransform.gameObject.activeSelf)
        {
            basePlatform = _platformGeneratorService.FinalTransform.GetComponent<PlatformView>();

            basePlatform.isFinal = false;                     // artık normal platform
            _platformGeneratorService.ResetForNewLevel(basePlatform);        // ★ generator’a yeni base’i bildir
        }
        else
        {
            basePlatform = _platformGeneratorService.SpawnFirst();           // ilk defa oyuna girerken
        }

        _platformGeneratorService.SpawnFinal(basePlatform.Width);            // yeni final
        cutsDone = 0;

        currentPlatform = _platformGeneratorService.SpawnNext(true, basePlatform.Width);
        Move(currentPlatform);
    }


    void Update()
    {
        if (!_gameService.IsLevelActive || !_inputService.IsTapDown) return;

        currentPlatform.GetComponent<PlatformModel>().Stop();

        if (!CutSucceeded(basePlatform, currentPlatform))
        { _gameService.EndGame(false); return; }

        cutsDone++;

        
        if (cutsDone >= _levelService.NormalPlatformCount - 1)
        {
            // base hâlâ currentPlatform; final platformu hedef göster
            // oyuncuya final hedefini ver
            OnBasePlatformChanged?.Invoke(_platformGeneratorService.FinalTransform);   // ← DOĞRU

            return;                               
        }

        basePlatform = currentPlatform;
        OnBasePlatformChanged?.Invoke(basePlatform.transform);

        currentPlatform = _platformGeneratorService.SpawnNext(UnityEngine.Random.value > .5f, basePlatform.Width);
        Move(currentPlatform);
    }

    void Move(PlatformView v)
    {
        var m = v.GetComponent<PlatformModel>();
        m.Init(_platformGeneratorService.CurrentDirectionSign);
        m.Play();
    }

    bool CutSucceeded(PlatformView prev, PlatformView cur)
    {
        float dx = cur.transform.position.x - prev.transform.position.x;
        float hang = Mathf.Abs(dx);
        if (hang >= prev.Width) return false;

        float newW = prev.Width - hang;
        cur.Resize(newW);

        float newX = prev.transform.position.x + dx * 0.5f;
        cur.transform.position = new Vector3(newX, cur.transform.position.y, cur.transform.position.z);

        cur.SpawnDebris(hang, dx >= 0 ? 1 : -1);
        return true;
    }

    void ClearAll()
    {
        foreach (var v in FindObjectsOfType<PlatformView>())
            if (!v.isFinal)                       
                v.gameObject.SetActive(false);
    }

}

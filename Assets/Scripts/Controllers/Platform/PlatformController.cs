using UnityEngine;
using Zenject;

public class PlatformController : MonoBehaviour
{
    [Inject] IPlatformGeneratorService _platformGeneratorService;
    [Inject] IInputService _inputService;
    [Inject] IGameFlowService _gameService;
    [Inject] ICameraService _cameraService;
    [Inject] ILevelDifficultyService _levelService;

    public static System.Action<Transform> OnBasePlatformChanged;

    PlatformView basePlatform;
    PlatformView currentPlatform;
    int cuts = 0;

    private void Awake() => StartLevel();
    private void OnEnable() => _levelService.OnLevelAdvanced += StartLevel;
    private void OnDisable() => _levelService.OnLevelAdvanced -= StartLevel;

    private void StartLevel()
    {
        ClearScene();

        if (_platformGeneratorService.FinalTransform && _platformGeneratorService.FinalTransform.gameObject.activeSelf)
        {
            basePlatform = _platformGeneratorService.FinalTransform.GetComponent<PlatformView>();
            basePlatform.isFinal = false;
            _platformGeneratorService.ResetForNewLevel(basePlatform);
        }
        else
        {
            basePlatform = _platformGeneratorService.SpawnFirstPlatform();
        }

        _platformGeneratorService.SpawnFinalPlatform(basePlatform.Width);
        OnBasePlatformChanged?.Invoke(basePlatform.transform);

        cuts = 0;
        currentPlatform = _platformGeneratorService.SpawnNextPlatform(true, basePlatform.Width);
        Move(currentPlatform);
    }

    private void Update()
    {
        if (!_gameService.IsLevelActive || !_inputService.IsTapDown) return;

        currentPlatform.GetComponent<PlatformModel>().Stop();

        if (!CutSucceeded(basePlatform, currentPlatform)) { _gameService.EndGame(false); return; }

        cuts++;
        if (cuts >= _levelService.NormalPlatformCount - 1)
        {
            OnBasePlatformChanged?.Invoke(_platformGeneratorService.FinalTransform);  
            return;                                              
        }

        basePlatform = currentPlatform;
        OnBasePlatformChanged?.Invoke(basePlatform.transform);

        currentPlatform = _platformGeneratorService.SpawnNextPlatform(Random.value > .5f, basePlatform.Width);
        Move(currentPlatform);
    }

    private void Move(PlatformView v)
    {
        var m = v.GetComponent<PlatformModel>();
        m.Init(_platformGeneratorService.CurrentDirectionSign);
        m.Play();
    }

    private bool CutSucceeded(PlatformView prev, PlatformView cur)
    {
        float dx = cur.transform.position.x - prev.transform.position.x;
        if (Mathf.Abs(dx) >= prev.Width) return false;

        float newW = prev.Width - Mathf.Abs(dx);
        cur.Resize(newW);
        cur.transform.position =
            new Vector3(prev.transform.position.x + dx * 0.5f,
                        cur.transform.position.y,
                        cur.transform.position.z);

        cur.SpawnDebris(Mathf.Abs(dx), dx >= 0 ? 1 : -1);
        return true;
    }

    private void ClearScene()
    {
        foreach (var platformView in FindObjectsOfType<PlatformView>())
            if (!platformView.isFinal) platformView.gameObject.SetActive(false);
    }
}

using UnityEngine;
using Zenject;

public class PlatformController : MonoBehaviour
{
    [Inject] IPlatformGenerator _platformGenerator;
    [Inject] IInputService _inputService;
    [Inject] IGameStateService _gameStateService;

    [Header("Debug")]
    [SerializeField] private PlatformView basePlatform;
    [SerializeField] private PlatformView currentPlatform;

    // Start is called before the first frame update
    void Awake()
    {
        basePlatform = _platformGenerator.SpawnFirst();
        currentPlatform = _platformGenerator.SpawnNext(Random.value > .5f, basePlatform.Width);
        StartMoving(currentPlatform);
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameStateService.currentState != GameState.GamePlay) return;
        if (!_inputService.IsTapDown) return;

        
        currentPlatform.GetComponent<PlatformModel>().Stop();
        
        if (!TryCut(basePlatform, currentPlatform))
        {
            Debug.Log("GAME OVER");
            _gameStateService.SetState(GameState.Failed);
            return;
        }
        
        basePlatform = currentPlatform;
        currentPlatform = _platformGenerator.SpawnNext(Random.value > .5f, basePlatform.Width);
        StartMoving(currentPlatform);
    }

    void StartMoving(PlatformView view)
    {
        var ctrl = view.GetComponent<PlatformModel>();
        ctrl.Init(_platformGenerator.CurrentDirectionSign);
        ctrl.Play();
    }

    public bool TryCut(PlatformView previousPlatform, PlatformView currentPlatform)
    {
        float delta = currentPlatform.transform.position.x - previousPlatform.transform.position.x;
        float hangover = Mathf.Abs(delta);
        float prevWidth = previousPlatform.Width;

        if (hangover >= prevWidth)   
            return false;

        float newWidth = prevWidth - hangover;
        currentPlatform.Resize(newWidth);

        /* Edge Matching */
        float newCenterX = previousPlatform.transform.position.x + (delta * 0.5f);
        currentPlatform.transform.position = new Vector3(
            newCenterX,
            currentPlatform.transform.position.y,
            currentPlatform.transform.position.z);

        /* Debris Spawning */
        int dir = delta >= 0 ? 1 : -1; 
        currentPlatform.SpawnDebris(hangover, dir);

        /* Perfect control */
        bool isPerfect = _platformGenerator.IsPerfectCut(delta);
        if (isPerfect) 
        {
            //TODO: ScoreService increase score for perfectCut 
            Debug.Log("PERFECT!");
        }




        return true;              
    }


}

using Zenject;
using UnityEngine;

public interface IAudioService
{
    void PlayCut(bool isPerfect);
}

public class AudioService : MonoBehaviour, IAudioService, IInitializable
{
    [Header("Assets")]
    [SerializeField] AudioClip cutSfx;

    [Header("Pitch settings")]
    [SerializeField] float basePitch = 1f;
    [SerializeField] float stepPitch = .05f;
    [SerializeField] float maxPitch = 1.7f;

    AudioSource _src;
    float _currentPitch;

    [Inject] IGameFlowService _gameService;

    public void Initialize()
    {
        _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.clip = cutSfx;

        _currentPitch = basePitch;
        _gameService.LevelStartedEvent += ResetPitch;
    }

    public void PlayCut(bool isPerfect)
    {
        if (isPerfect) 
        {
            _currentPitch = Mathf.Min(_currentPitch + stepPitch, maxPitch); 
        }          
           
        else
        {
            ResetPitch();  
        }  
        _src.pitch = _currentPitch;
        _src.PlayOneShot(cutSfx);
    }
    public void ResetPitch()
    {
        _currentPitch = basePitch;
    }
}


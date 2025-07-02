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

    public void Initialize()
    {
        _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.clip = cutSfx;

        _currentPitch = basePitch;
    }

    public void PlayCut(bool isPerfect)
    {
        if (isPerfect) 
        {
            _currentPitch = Mathf.Min(_currentPitch + stepPitch, maxPitch); // başarılı & zamanlaması iyi
            Debug.Log("başarılı & zamanlaması iyi ses");
        }          
           
        else
        {
            _currentPitch = basePitch;// kötü zamanlama, pitch reset
            Debug.Log("kötü zamanlama sesi");
        }  
        

        _src.pitch = _currentPitch;
        //_src.PlayOneShot(cutSfx);
    }
}


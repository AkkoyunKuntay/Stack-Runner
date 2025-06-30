using DG.Tweening;
using UnityEngine;

public class PlatformModel : MonoBehaviour
{
    [SerializeField] private float maxOffset = 2f;       
    [SerializeField] private float movementDuration = 1f;   

    private int _dirSign;     
    private Tween _moveTween;   

    public void Init(int dirSign)
    {
        _dirSign = dirSign;
        _moveTween = transform.DOMoveX(-_dirSign * maxOffset, movementDuration)
                      .SetLoops(-1, LoopType.Yoyo)
                      .SetEase(Ease.Linear)
                      .SetAutoKill(false)
                      .Pause();
    }

    public void Play() => _moveTween.Play();
    public void Stop() => _moveTween.Pause();

    
    private void OnDestroy() 
    {
        if (_moveTween != null && _moveTween.IsActive())
            _moveTween.Kill();
    }
}

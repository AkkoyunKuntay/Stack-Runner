using Cinemachine;
using UnityEngine;
using Zenject;
using DG.Tweening;


public enum CamType { Start, Gameplay, Win, Fail }
public interface ICameraService
{
    void Shake(float duration);
    void OrbitAround(Transform target, float duration);
}

public class CameraService : MonoBehaviour, ICameraService,IInitializable
{
    [Header("VCams")]
    [SerializeField] CinemachineVirtualCamera startCam;
    [SerializeField] CinemachineVirtualCamera gameplayCam;
    [SerializeField] CinemachineVirtualCamera winCam;
    [SerializeField] CinemachineVirtualCamera failCam;

    [Header("Debug")]
    [SerializeField] CinemachineVirtualCamera currentCamera;

    [Header("Impulse")]
    [SerializeField] CinemachineImpulseSource impulse;

    [SerializeField] int activePriority = 50;
    [SerializeField] int idlePriority = 10;

    [Inject] IGameFlowService _gameFlowService;

    void IInitializable.Initialize()
    {
        currentCamera = startCam;
        SetActiveCamera(CamType.Start);

        _gameFlowService.LevelStartedEvent += OnLevelStartedEvent;
        _gameFlowService.LevelSuccessEvent += OnLevelSuccessEvent;
        _gameFlowService.LevelFailedEvent += OnLevelFailedEvent;

        Debug.Log("[CameraService] has been initialized.");
    }
    private void OnLevelStartedEvent() => SetActiveCamera(CamType.Gameplay);
    private void OnLevelSuccessEvent() => SetActiveCamera(CamType.Win);
    private void OnLevelFailedEvent() => SetActiveCamera(CamType.Fail);
    private void SetActiveCamera(CamType type)
    {
        currentCamera.Priority = idlePriority;

        switch (type)
        {
            case CamType.Start:
                currentCamera = startCam;
                break;
            case CamType.Gameplay:
                currentCamera = gameplayCam;
                break;
            case CamType.Win:
                currentCamera = winCam;
                break;
            case CamType.Fail:
                currentCamera = failCam;
                break;
            default:
                break;
        }

        currentCamera.Priority = activePriority;

    }

  
    #region ICameraService

    public void OrbitAround(Transform target, float duration)
    {
        var v = currentCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        if (v == null) return;
        float start = v.m_XAxis.Value;
        DOTween.To(() => v.m_XAxis.Value,
                   x => v.m_XAxis.Value = x,
                   start + 360,
                   duration).SetEase(Ease.Linear);
    }
    public void Shake(float duration)
    {
        Debug.Log("Shake");
    }

    #endregion
}



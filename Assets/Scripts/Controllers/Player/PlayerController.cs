using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    Rigidbody rb;
    Transform target;

    [Inject] IGameFlowService _gameService;
    [Inject] ILevelDifficultyService _levelService;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PlatformController.OnBasePlatformChanged += targetTransform => target = targetTransform;
    }
    void OnDestroy() =>
        PlatformController.OnBasePlatformChanged -= targetTransform => target = targetTransform;

    void Update()
    {
        if (!_gameService.IsLevelActive || target == null) return;

        Vector3 dest = new Vector3(target.position.x, transform.position.y, target.position.z);

        Vector3 dir = (dest - transform.position).normalized;
        transform.position = Vector3.MoveTowards(
                                transform.position, dest, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent.TryGetComponent(out PlatformView platform)) return;
        if (!platform.isFinal) return;

        _gameService.EndGame(true);
        
    }
}

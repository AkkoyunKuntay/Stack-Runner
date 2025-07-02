using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    Animator animator;
    Rigidbody rb;
    Transform target;
    Vector3 fallTarget;
    [SerializeField]bool falling;

    [Inject] IGameFlowService _gameService;
    [Inject] ILevelDifficultyService _levelService;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        PlatformController.OnBasePlatformChanged += targetTransform => target = targetTransform;
        _gameService.LevelStartedEvent += OnLevelStarted;
        _gameService.LevelFailedEvent += OnLevelFailed;
        _gameService.LevelSuccessEvent += OnLevelSuccess;
    }

    private void OnLevelFailed()
    {
        fallTarget = transform.position + Vector3.forward;
        rb.isKinematic = false;
        transform.GetComponent<Collider>().enabled = false;
        falling = true;
    }

    void OnDestroy() =>
        PlatformController.OnBasePlatformChanged -= targetTransform => target = targetTransform;

    void Update()
    {
        if (falling)
        {
            // Sadece XZ düzleminde ileri yürüt, Y’yi fiziğe bırak
            Vector3 desiredDestination = new(fallTarget.x, transform.position.y, fallTarget.z);

            transform.position = Vector3.MoveTowards(
                                    transform.position, desiredDestination, speed * Time.deltaTime);
            return;    // normal MoveTowards çalışmasın
        }


        if (!_gameService.IsLevelActive || target == null) return;

        Vector3 dest = new Vector3(target.position.x, transform.position.y, target.position.z);

        Vector3 dir = (dest - transform.position).normalized;
        transform.position = Vector3.MoveTowards(
                                transform.position, dest, speed * Time.deltaTime);
    }

    private void OnLevelStarted()
    {
        animator.SetBool("Win",false);
    }
    private void OnLevelSuccess()
    {
        animator.SetBool("Win",true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent.TryGetComponent(out PlatformView platform)) return;
        if (!platform.isFinal) return;

        _gameService.EndGame(true);
    }
}

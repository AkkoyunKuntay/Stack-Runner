using UnityEngine;
using DG.Tweening;
using Zenject;
using static UnityEditor.PlayerSettings;
using System.Collections;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 4f;          
    [SerializeField] float groundRay = 0.6f;    

    Rigidbody rb;
    Transform target;                           

    [Inject] IGameFlowService _gameFlowService;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        PlatformController.OnBasePlatformChanged += OnBaseChanged;
    }
    void OnDestroy()
    {
        PlatformController.OnBasePlatformChanged -= OnBaseChanged;
    }

    void OnBaseChanged(Transform newBase)
    {
        target = newBase;
    }

    void Update()
    {
        if (!_gameFlowService.IsLevelActive || target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        if (!Physics.Raycast(transform.position, Vector3.down, groundRay))
        {
            _gameFlowService.EndGame(false);
        }
    }
    IEnumerator WaitAndWin()
    {
        //yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle"));
        yield return 2f;
        _gameFlowService.EndGame(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent.TryGetComponent<PlatformView>(out PlatformView platform))
        {
            if (platform.isFinal) StartCoroutine(WaitAndWin());
        }
    }
}

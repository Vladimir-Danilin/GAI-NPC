using UnityEngine;
using UnityEngine.AI;

public class NPCAI : MonoBehaviour
{
    protected NavMeshAgent _navMeshAgent;
    protected Animator _animator;
    [Header("Настроки передвижения")]
    [SerializeField]
    protected float movementSpeed = 10f;
    [SerializeField]
    protected float changePositionTime = 5f;
    [SerializeField]
    protected float moveDistance = 10f;

    protected virtual void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = movementSpeed;
        _animator = GetComponent<Animator>();
        InvokeRepeating(nameof(Move), changePositionTime, changePositionTime);
    }

    protected virtual void Update()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_navMeshAgent.velocity);
        float forwardSpeed = localVelocity.z;
        float strafeSpeed = localVelocity.x;

        _animator.SetFloat("v", forwardSpeed / movementSpeed);
        _animator.SetFloat("h", strafeSpeed / movementSpeed);
    }

    protected Vector3 RandomNavSphere(float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);

        return navHit.position;
    }

    protected virtual void Move()
    {
        _navMeshAgent.SetDestination(RandomNavSphere(moveDistance));
    }
}

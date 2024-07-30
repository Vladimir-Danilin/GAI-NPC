using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : NPCAI
{
    [SerializeField]
    public float healthPoint = 100;
    [SerializeField]
    private bool predator;
    [SerializeField]
    private Transform[] targets;
    [SerializeField]
    private float findRange = 10f;
    [SerializeField]
    private float attackRange = 4f;
    [SerializeField]
    private float attackCooldown = 2f;
    [SerializeField]
    private float attackDamage = 50;

    private float lastAttackTime;

    protected override void Start()
    {
        base.Start();
        lastAttackTime = -attackCooldown;
    }

    protected override void Update()
    {
        if (targets != null && predator)
        {
            foreach (Transform target in targets)
            {
                if (target != null)
                {
                    if (Vector3.Distance(transform.position, target.position) <= findRange)
                    {
                        _navMeshAgent.ResetPath();
                        _navMeshAgent.SetDestination(target.position);

                        if (Vector3.Distance(transform.position, target.position) <= attackRange)
                        {
                            Attack(target);
                        }
                    }
                }
            }
        }
        _animator.SetFloat("Speed", _navMeshAgent.velocity.magnitude / movementSpeed);
    }

    protected override void Move()
    {
        base.Move();
    }

    void Attack(Transform target)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (target != null)
            {
                target.gameObject.GetComponent<AnimalAI>().healthPoint -= attackDamage;
                Debug.Log("Атакую цель!");

                lastAttackTime = Time.time;
                if (target.gameObject.GetComponent<AnimalAI>().healthPoint <= 0)
                {
                    Destroy(target.gameObject);
                }
                if (target.IsDestroyed())
                {
                    InvokeRepeating(nameof(Move), changePositionTime, changePositionTime);
                }
            }
        }
    }

    List<NavMeshAgent> FindAgentsWithTypeID(int agentTypeID)
    {
        NavMeshAgent[] allAgents = GetComponents<NavMeshAgent>();
        List<NavMeshAgent> agentsWithTargetID = new List<NavMeshAgent>();

        foreach (var agent in allAgents)
        {
            if (agent.agentTypeID == agentTypeID)
            {
                agentsWithTargetID.Add(agent);
            }
        }
        return agentsWithTargetID;
    }
}

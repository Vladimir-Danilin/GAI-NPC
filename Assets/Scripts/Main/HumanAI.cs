using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class HumanAI : NPCAI
{
    [Header("Элементы UI")]
    [Tooltip("Текстовый элемент для отображения текста диалога.")]
    [SerializeField] private Text dialogText;

    [Tooltip("Элемент изображения для отображения заднего фона диалога.")]
    [SerializeField] private RawImage dialogImage;

    [Header("Настройки обнаружения")]
    [Tooltip("Включить или отключить коллайдер обнаружения.")]
    [SerializeField] private bool detectionColliderChecker;

    [Tooltip("Включите или отключите угол обнаружения.")]
    [SerializeField] private bool detectionAngle = false;

    [Tooltip("Радиус области обнаружения.")]
    [SerializeField] private float detectionRadius = 10f;

    [Tooltip("расстояние для остановки после обнаружения.")]
    [SerializeField] private float stoppingDistance = 3f;

    [Header("Диалоговые данные")]
    [Tooltip("Массив текстов диалогов.")]
    [SerializeField] private string[] texts;

    [Tooltip("Массив аудиоклипов речи.")]
    [SerializeField] private AudioClip[] speeches;

    [Tooltip("Ссылка на объект диалоговой системы.")]
    [SerializeField] private GameObject dialogSystemObject;
    
    [Header("Настройка враждебности")]
    [SerializeField] private bool enemy = false;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackDamage = 50;

    private float healthPoint = 100f;
    private float lastAttackTime;
    private SphereCollider detectionCollider;
    private bool checkDialog = false;
    private float scaledResult;
    private Inventory inventory = new Inventory();

    protected override void Start()
    {
        base.Start();
        if (detectionColliderChecker == true)
        {
            detectionCollider = gameObject.AddComponent<SphereCollider>();
            detectionCollider.isTrigger = true;
            detectionCollider.radius = detectionRadius;
        }

        float normalizedDetectionRadius = detectionRadius / 20.0f;
        float normalizedStoppingDistance = stoppingDistance / 10.0f;
        float normalizedMovementSpeed = movementSpeed / 20.0f;

        scaledResult = 1 + ((normalizedDetectionRadius + normalizedStoppingDistance + normalizedMovementSpeed) / 3 * 2);
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other is CapsuleCollider)
        {
            NavMeshAgent otherAgent = other.GetComponent<NavMeshAgent>();
            if (otherAgent != null && otherAgent.agentTypeID == _navMeshAgent.agentTypeID)
            {
                StartCoroutine(DialogNPC(other));
            }
            else if(enemy && other.tag == "Player")
            {
                var target = other.transform;
                if (Vector3.Distance(transform.position, target.position) <= detectionRadius)
                {
                    _navMeshAgent.ResetPath();
                    _navMeshAgent.SetDestination(target.position);

                    if (Vector3.Distance(transform.position, target.position) <= detectionRadius)
                    {
                        if (Time.time - lastAttackTime >= attackCooldown)
                        {
                            if (target != null)
                            {
                                target.gameObject.GetComponent<HumanAI>().healthPoint -= attackDamage;
                                Debug.Log("Атакую цель!");

                                lastAttackTime = Time.time;
                                if (target.gameObject.GetComponent<HumanAI>().healthPoint <= 0)
                                {
                                    Destroy(other.transform.gameObject);
                                }
                                if (target.IsDestroyed())
                                {
                                    InvokeRepeating(nameof(Move), changePositionTime, changePositionTime);
                                }
                            }
                        }
                    }
                }
            }
        }
        if (other.gameObject.CompareTag("Item"))
        {
            TakeItem(other);
        }
    }

    async void TakeItem(Collider other)
    {
        CancelInvoke(nameof(Move));
        _navMeshAgent.ResetPath();
        _navMeshAgent.SetDestination(other.transform.position);

        await Task.Delay(2000);

        AddItemToInventory(other.name, other.gameObject.ToString());
        Destroy(other.gameObject);

        InvokeRepeating(nameof(Move), changePositionTime, changePositionTime);
    }

    public void AddItemToInventory(string itemName, string itemLink)
    {
        Item newItem = new Item(itemName, itemLink);
        inventory.AddItem(newItem);
        Debug.Log($"Picked up: {itemName}");
    }

    protected override void Move()
    {
        base.Move();
    }

    private void MoveTowards(Transform target)
    {
        _navMeshAgent.SetDestination(target.position - new Vector3(stoppingDistance, stoppingDistance, 0));
    }

    private IEnumerator DialogNPC(Collider other)
    {
        Vector3 directionToOther = other.transform.position - transform.position;
        float angle = 0;

        if (!detectionAngle)
            angle = 90;
        else
            angle = Vector3.Angle(transform.forward, directionToOther);

        if (angle <= 180 / 2 && !checkDialog)
        {
            checkDialog = true;

            CancelInvoke(nameof(Move));
            other.gameObject.GetComponent<HumanAI>().CancelInvoke(nameof(Move));

            MoveTowards(other.transform);

            yield return new WaitForSeconds(scaledResult + 1);

            other.transform.LookAt(transform);
            transform.LookAt(other.transform);

            if (dialogImage != null)
                dialogImage.gameObject.SetActive(true);

            var dialogSystem = dialogSystemObject.GetComponent<DialogSystem>();
            
            dialogSystem.lines = texts;
            dialogSystem.dialogText = dialogText;
            dialogSystem.dialogImage = dialogImage;
            dialogSystem.speeches = speeches;
            dialogSystem.navMeshAgent = _navMeshAgent;

            dialogSystem.StartDialog();

            int lenghtTexts = 0;
            foreach (var text in texts)
                lenghtTexts += text.Length;

            yield return new WaitForSeconds(texts.Length + lenghtTexts / 10);
            
            InvokeRepeating(nameof(Move), changePositionTime, changePositionTime);
            other.gameObject.GetComponent<HumanAI>().InvokeRepeating(nameof(Move), changePositionTime, changePositionTime);
        }
        yield break;
    }
}


public class Inventory
{
    public List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
}

[System.Serializable]
public class Item
{
    public string itemName;
    public string itemLink;

    public Item(string name, string link)
    {
        itemName = name;
        itemLink = link;
    }
}
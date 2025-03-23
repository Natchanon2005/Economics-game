using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Customer : MonoBehaviour
{
    [Header("Order")]
    public string drinkName;
    public int amount;
    public bool HasBeenServed { get; set; } = false; // Track if the customer has been served
    [Space]
    [Space]
    private NavMeshAgent agent;
    private Vector3 startPosition;
    private bool inQueue = false;
    public NavMeshAgent navMeshAgent;
    public CustomerManager customerManager;
    public TMP_Text orderText; // Add this field to reference the UI text for the order

    void Start()
    {
        string[] drinks = { "Black Dragon Brew", "Moonlight Caramel Latte", "Frosted Vanilla Heaven", "Liberica Lava Burst" };
        drinkName = drinks[Random.Range(0, drinks.Length)];
        amount = Random.Range(1, 5);

        agent = navMeshAgent;
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on Customer object.");
            return;
        }
        startPosition = transform.position;
        ConfigureNavMeshAgent(); // Configure the agent for better avoidance
    }

    private void ConfigureNavMeshAgent()
    {
        if (agent != null)
        {
            agent.avoidancePriority = Random.Range(20, 80); // Assign random priority to reduce collisions
            agent.radius = 0.35f; // Adjust the radius for better pathfinding in narrow spaces
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance; // Use high-quality avoidance
        }
    }

    public void GoToQueue(Vector3 queuePosition)
    {
        agent = navMeshAgent;
        if (agent == null) return; // Prevent null reference
        agent.SetDestination(queuePosition);
        inQueue = true;
    }

    public void LeaveQueue()
    {
        if (!inQueue) return; // Skip if not in queue
        HasBeenServed = true;
        inQueue = false;
        agent?.SetDestination(startPosition); // Use null-conditional operator
        customerManager?.NotifyQueueSpotAvailable(this); // Notify manager if available
        Destroy(gameObject, 5f);
    }

    public bool IsInQueue()
    {
        return inQueue && agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    public void UpdateOrderText()
    {
        if (orderText != null)
        {
            orderText.text = $"Order: {drinkName} x{amount}";
        }
    }
}

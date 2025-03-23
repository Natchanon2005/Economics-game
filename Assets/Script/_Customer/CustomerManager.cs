using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    public List<Transform> queuePoints;
    public GameObject customerPrefab;
    public float spawnInterval = 5f;
    public TMP_Text Order;

    private List<Customer> customers = new List<Customer>();

    void Start()
    {
        InvokeRepeating(nameof(SpawnCustomer), 0f, spawnInterval);
    }

    void SpawnCustomer()
    {
        if (customers.Count >= queuePoints.Count)
        {
            Debug.Log("Queue is full. Cannot spawn new customer.");
            return;
        }

        GameObject newCustomer = Instantiate(customerPrefab, transform.position, Quaternion.identity);
        var navMeshAgent = newCustomer.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("Spawned customerPrefab is missing a NavMeshAgent component.");
            Destroy(newCustomer); // Prevent further issues
            return;
        }

        Customer customerComponent = newCustomer.GetComponent<Customer>();
        customerComponent.customerManager = this;
        customerComponent.orderText = Order;
        AddCustomer(customerComponent);
    }

    void AssignQueuePositions()
    {
        int count = Mathf.Min(customers.Count, queuePoints.Count);
        for (int i = 0; i < count; i++)
        {
            customers[i].GoToQueue(queuePoints[i].position);
        }
    }

    public void CallNextCustomer()
    {
        if (customers.Count > 0 && customers[0].IsInQueue())
        {
            customers[0].LeaveQueue();
        }
    }

    public void AddCustomer(Customer customer)
    {
        customers.Add(customer);
        if (customers.Count <= queuePoints.Count)
        {
            customer.GoToQueue(queuePoints[customers.Count - 1].position);
        }
    }

    public void NotifyQueueSpotAvailable(Customer customer)
    {
        if (customers.Remove(customer))
        {
            AssignQueuePositions();
        }
    }
}

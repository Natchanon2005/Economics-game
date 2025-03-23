using UnityEngine;
using DG.Tweening;
using System.Collections.Generic; // Ensure DOTween is imported

public class DeliveryManager : MonoBehaviour
{
    public GameObject objectPrefab; // Prefab to spawn
    public Transform targetPoint; // Target point to move toward
    public List<Vector3> spawnPoint; // Spawn point
    private List<Delivery> deliveries = new List<Delivery>(); // List to store multiple deliveries
    [SerializeField] private Inventory inventory;

    public void SpawnDelivery(ItemData itemData, int amount)
    {
        // Randomly select a spawn point from the list
        Vector3 selectedSpawnPoint = spawnPoint[Random.Range(0, spawnPoint.Count)];
        selectedSpawnPoint.y = targetPoint.position.y; // Set y to match targetPoint.y

        // Instantiate the object at the selected spawn point
        GameObject obj = Instantiate(objectPrefab, selectedSpawnPoint, Quaternion.identity);

        // Ensure each object has its own Delivery component
        Delivery delivery = obj.GetComponent<Delivery>();
        if (delivery == null)
        {
            delivery = obj.AddComponent<Delivery>();
        }

        delivery.DeliveryProduct(itemData, amount, inventory);
        delivery.deliveryManager = this;
        delivery.targetPoint = targetPoint;

        // Add the delivery to the list
        deliveries.Add(delivery);

        // Ensure the object faces the target point
        obj.transform.LookAt(new Vector3(targetPoint.position.x, obj.transform.position.y, targetPoint.position.z));

        // Get the Animator component from the object
        Animator animator = obj.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("idle", false); // Set idle to false when starting movement
        }

        // Smoothly rotate to face the target point and then move
        obj.transform.DOLookAt(new Vector3(targetPoint.position.x, obj.transform.position.y, targetPoint.position.z), 0.5f).OnComplete(() =>
        {
            // Move the object to the target point
            obj.transform.DOMove(targetPoint.position - (targetPoint.position - obj.transform.position).normalized, 18f).SetEase(Ease.Linear).OnUpdate(() =>
            { // Continuously align to the ground during movement
            }).OnComplete(() =>
            {
                if (animator != null)
                {
                    animator.SetBool("idle", true); // Set idle to true upon reaching the target
                    delivery.MoveToTarget(); // Move the object back to the spawn point
                }

                // Wait for 1 second before moving back
                DOVirtual.DelayedCall(1f, () =>
                {
                    if (animator != null)
                    {
                        animator.SetBool("idle", false); // Set idle to false before moving back
                    }

                    // Smoothly rotate to face the spawn point and then move back
                    obj.transform.DOLookAt(new Vector3(selectedSpawnPoint.x, obj.transform.position.y, selectedSpawnPoint.z), 0.5f).OnComplete(() =>
                    {
                        obj.transform.DOMove(selectedSpawnPoint, 18f).SetEase(Ease.Linear).OnUpdate(() =>
                        {
                        }).OnComplete(() =>
                        {
                            // Remove the delivery from the list and destroy the object
                            deliveries.Remove(delivery);
                            Destroy(obj);
                            DOTween.Kill(obj.transform);
                        });
                    });
                });
            });
        });
    }
}

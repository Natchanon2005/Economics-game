using UnityEngine;
using DG.Tweening; // Ensure DOTween is imported

public class Delivery : MonoBehaviour
{
    public GameObject objectPrefab; // Prefab to spawn
    public Transform targetPoint; // Target point to move toward
    public float spawnRange = 5f; // Range for random spawn position
    public float moveDuration = 2f; // Duration to move to the target
    public float fallDistance = 3f; // Distance to fall
    public float fallDuration = 1f; // Duration of the fall
    [SerializeField] private Rigidbody rb;

    public void SpawnAndMoveObject()
    {
        // Generate a random spawn position around the target point
        Vector3 spawnPosition = targetPoint.position + new Vector3(
            Random.Range(10, spawnRange),
            5,
            0
        );

        // Instantiate the object
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        // Move the object toward the target point
        spawnedObject.transform.DOMove(targetPoint.position, moveDuration).OnComplete(() =>
        {
            // Simulate falling after reaching the target
            spawnedObject.transform.DOMoveY(targetPoint.position.y - fallDistance, fallDuration).OnComplete(() =>
            {
                // Enable Rigidbody after falling
                Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false; // Enable physics
                }
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

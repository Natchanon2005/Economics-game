using UnityEngine;
using DG.Tweening;

public class Delivery : MonoBehaviour
{
    [SerializeField] private GameObject productPrefab; // Prefab to spawn
    [SerializeField] private Transform productSpawnPoint; // Spawn point
    [SerializeField] private GameObject parenct; // Parent object
    [SerializeField] public Transform targetPoint; // Target point to move toward
    [SerializeField] public DeliveryManager deliveryManager;
    private GameObject product;

    public void DeliveryProduct()
    {
        // Create a new product instance
        GameObject newProduct = Instantiate(productPrefab, productSpawnPoint.position, Quaternion.identity);
        newProduct.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Scale the object to half its size
        newProduct.transform.SetParent(parenct.transform); // Set parent to parenct

        // Update the reference to the latest product
        product = newProduct;
    }

    public void MoveToTarget()
    {
        if (product == null)
        {
            Debug.LogWarning("No product to move. Call DeliveryProduct first.");
            return;
        }

        GameObject mainParent = GameObject.Find("Env");
        product.transform.SetParent(mainParent.transform);

        // Ensure the product stays above the ground
        Vector3 targetPosition = targetPoint.position;
        targetPosition.y = 1.3f; // Ensure y is not below 0

        // Reset any existing Rigidbody to avoid duplicates
        Rigidbody existingRb = product.GetComponent<Rigidbody>();
        if (existingRb != null)
        {
            Destroy(existingRb);
        }

        // หมุน + เคลื่อนไหวพร้อมกันแบบเนียน ๆ
        product.transform.DORotateQuaternion(Quaternion.Euler(0, targetPoint.rotation.eulerAngles.y, 0), 1f)
            .SetEase(Ease.InQuad);

        product.transform.DOMove(targetPosition, 1f)
            .SetEase(Ease.InOutQuad);

        product.transform.DOScale(1.5f, 1f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => 
            {
                Rigidbody rb = product.AddComponent<Rigidbody>();
                rb.useGravity = true;
                DOTween.Kill(product.transform);
            });
    }
}
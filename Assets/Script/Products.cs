using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Products : MonoBehaviour
{
    [Header("Product Settings")]
    public List<Product> products = new List<Product>();
    public Product currentProduct;

    private bool isTransitioning = false; // Flag to prevent rapid calls

    private void Start()
    {
        if (products.Count > 0)
        {
            SetProduct(0);  // ตั้งค่าเริ่มต้นเป็นตัวแรก
        }
    }

    public void SetProduct(int index)
    {
        if (isTransitioning || index < 0 || index >= products.Count) return;

        if (currentProduct != null && currentProduct != products[index])
        {
            StopAllCoroutines(); // Stop any ongoing fade-out coroutines
            StartCoroutine(FadeOutAndDisable(currentProduct));
        }
    }

    public void OpenMenu()
    {
        if (isTransitioning) return;

        if (!gameObject.activeInHierarchy) return; // Ensure the GameObject is active

        StartCoroutine(HandleMenuTransition(() =>
        {
            foreach (var product in products)
            {
                if (!product.gameObject.activeSelf) // เช็กก่อนเปิด
                {
                    product.gameObject.SetActive(true);
                    product.FadeIn(0.5f);
                }
            }
        }));
    }

    public void ExitMenu()
    {
        if (isTransitioning) return;

        if (!gameObject.activeInHierarchy) return; // Ensure the GameObject is active

        StartCoroutine(HandleMenuTransition(() =>
        {
            foreach (var product in products)
            {
                StartCoroutine(FadeOutAndDisable(product));
            }
        }));
    }

    private IEnumerator HandleMenuTransition(System.Action action)
    {
        isTransitioning = true;
        action.Invoke();
        yield return new WaitForSeconds(0.5f); // Wait for transitions to complete
        isTransitioning = false;
    }

    private IEnumerator FadeOutAndDisable(Product product)
    {
        if (product == null) yield break;

        product.FadeOut(0.5f);
        yield return new WaitForSeconds(0.5f);
        if (product != null)
        {
            product.gameObject.SetActive(false);
        }
    }
}

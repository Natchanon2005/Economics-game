using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI; // Add this for TextMeshPro support

public class Product : MonoBehaviour 
{
    [SerializeField] public string productName;
    [SerializeField] public string productDescription;
    [SerializeField] public Sprite productImage;
    [SerializeField] public float productPrice;
    [SerializeField] private TMP_Text productNameText;
    [SerializeField] private TMP_Text productDescriptionText;
    [SerializeField] private TMP_Text productPriceText;
    [SerializeField] private RawImage _productImage;

    public void Initialize(string productName, string productDescription, Sprite productImage, float productPrice)
    {
        this.productName = productName;
        this.productDescription = productDescription;
        this.productImage = productImage;
        this.productPrice = productPrice;
        
    }

    public void Start()
    {
        if (productNameText != null) productNameText.text = this.productName;
        if (productDescriptionText != null) productDescriptionText.text = this.productDescription;
        if (productPriceText != null) productPriceText.text = this.productPrice.ToString("F2");
        if (_productImage != null) _productImage.texture = this.productImage.texture;
    }

    public void FadeIn(float duration)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true); // Ensure the GameObject is active
        }
        CanvasGroup canvasGroup = GetOrAddCanvasGroup();
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(1, duration);
        }
    }

    public void FadeOut(float duration)
    {
        CanvasGroup canvasGroup = GetOrAddCanvasGroup();
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0, duration).OnComplete(() => gameObject.SetActive(false)); // Deactivate after fade-out
        }
    }

    private CanvasGroup GetOrAddCanvasGroup()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        return canvasGroup;
    }
}
using UnityEngine;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] public ItemCategory itemCategory;

    public void ChangeCategory()
    {
        shopManager.ChangeCategory(itemCategory);
    }
}
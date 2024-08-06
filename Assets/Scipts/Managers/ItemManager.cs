using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [Header("Texts")]
    public TextMeshProUGUI ItemCountText;
    public TextMeshProUGUI totalPriceText;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI PriceText;

    [Header("Image")]
    public RawImage icon;

    [Header("Counts")]
    public int ItemCount = 1;
    public float TotalPrice;


    [Header("Buttons")]
    public GameObject PlusButton;
    public GameObject MinesButton;
    public GameObject BuyButton;

    public int Money = 100;
    public ShopItems ShopItems;

    [Header("Buy System")]
    private List<GameObject> shoppingCart = new List<GameObject>(); // Sepet listesi
    public Vector3 instantiatePos;

    private void Awake()
    {
        icon.texture = ShopItems.Icon.texture;
        NameText.text = ShopItems.Name;
        TotalPrice = ShopItems.Price;

        PriceText.text = "$" + ShopItems.Price.ToString();
        totalPriceText.text = "$" + TotalPrice.ToString();


    }
    public void IncreaseItemCount()
    {
        ItemCount++;
        TotalPrice += ShopItems.Price;
        ItemCountText.text = ItemCount.ToString();
        totalPriceText.text = "$" + TotalPrice.ToString();
    }

    public void DecreaseItemCount()
    {
        if (ItemCount > 0)
        {
            ItemCount--;
            TotalPrice -= ShopItems.Price;
            ItemCountText.text = ItemCount.ToString();
            totalPriceText.text = "$" + TotalPrice.ToString();

        }
    }
    public void Buy()
    {
        GameObject selectedObject = ShopItems.Item;
        for (int i = 0; i < ItemCount; i++)
        {
            shoppingCart.Add(selectedObject);
            GameObject spawnedObject = Instantiate(selectedObject, instantiatePos, Quaternion.identity);
            if(spawnedObject.GetComponent<FurnitureBox>() != null)
            {
                FurnitureBox furnitureBox = spawnedObject.GetComponent<FurnitureBox>();
                furnitureBox.SetFurniture(GameManager.instance.Furnitures[ShopItems.index]);
            }

        }
        shoppingCart.Clear();
        Debug.Log("Ürünler Geldi");
    }
}

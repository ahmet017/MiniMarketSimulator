using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ShopItems.asset", menuName = "MiniFoodMarket/ShopItems")]
public class ShopItems : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    public float Price;
    public GameObject Item;
    public int index;
}

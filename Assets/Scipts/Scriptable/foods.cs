using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Foods.asset", menuName = "MiniFoodMarket/Food")]
public class Foods : ScriptableObject
{
    public string FoodName;
    public float FoodPrice;
    public Vector3 scale;
    public int MaxItemCountOnShelf;
    public float ItemSpacing;
    public float Shelfwidth;
}

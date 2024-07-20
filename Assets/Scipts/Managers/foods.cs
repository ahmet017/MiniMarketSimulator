using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Foods.asset", menuName = "MiniFoodMarket/Food")]
public class Foods : ScriptableObject
{
    public string FoodName;
    public float FoodPrice;
}

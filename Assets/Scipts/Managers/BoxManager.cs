using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    public static BoxManager instance;
    public Transform[] urunYerleri; // Çocuk objelerin transformlarýný tutmak için dizi

    private List<int> bosYerler = new List<int>(); // Boþ yerlerin indekslerini tutmak için liste
    public int ItemCount;
    void Start()
    {
        for (int i = 0; i < urunYerleri.Length; i++)
        {
            bosYerler.Add(i);
        }

        if(instance == null)
            instance = this;
    }
    public int yerIndex;
    public void UrunEkle()
    {

        if (bosYerler.Count > 0)
        {
            // Her çaðrýda yeni bir indeks seç
            //int randomIndex = UnityEngine.Random.Range(0, bosYerler.Count);
            yerIndex = bosYerler[ItemCount];
            ItemCount++;
            // Boþ yer listesinden kaldýr
            //bosYerler.RemoveAt(randomIndex);
        }
        else
        {
            Debug.LogWarning("Tüm yerler dolu!");
        }
    }

    public void UrunKaldir()
    {
        if(ItemCount > 0)
            ItemCount--;

    }
}

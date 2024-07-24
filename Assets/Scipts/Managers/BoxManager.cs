using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    public static BoxManager instance;
    public Transform[] urunYerleri; // �ocuk objelerin transformlar�n� tutmak i�in dizi

    private List<int> bosYerler = new List<int>(); // Bo� yerlerin indekslerini tutmak i�in liste
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
            // Her �a�r�da yeni bir indeks se�
            //int randomIndex = UnityEngine.Random.Range(0, bosYerler.Count);
            yerIndex = bosYerler[ItemCount];
            ItemCount++;
            // Bo� yer listesinden kald�r
            //bosYerler.RemoveAt(randomIndex);
        }
        else
        {
            Debug.LogWarning("T�m yerler dolu!");
        }
    }

    public void UrunKaldir()
    {
        if(ItemCount > 0)
            ItemCount--;

    }
}

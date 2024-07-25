using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    public static BoxManager instance;
    public Transform[] urunYerleri; // �ocuk objelerin transformlar�n� tutmak i�in dizi
    public Foods Item;
    public Animator animator;

    private List<int> bosYerler = new List<int>(); // Bo� yerlerin indekslerini tutmak i�in liste
    private int ItemCount;
    void Awake()
    {
        for (int i = 0; i < urunYerleri.Length; i++)
        {
            bosYerler.Add(i);
        }
        animator = GetComponent<Animator>();
        if(instance == null)
            instance = this;
        ItemCount = bosYerler.Count;
    }
    public int yerIndex;
    public void UrunEkle()
    {
        if (bosYerler.Count > 0)
        {
            yerIndex = bosYerler[ItemCount];
            ItemCount++;
            Debug.Log(ItemCount);
        }
        else
        {
            Debug.LogWarning("T�m yerler dolu!");
        }
    }

    public void UrunKaldir()
    {
        if (ItemCount > 0)
            ItemCount--;
        Debug.Log(ItemCount);

    }
}

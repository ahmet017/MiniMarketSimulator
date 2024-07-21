using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    public Transform shelf; // the shelf game object
    public float shelfWidth = 1.0f; // the width of the shelf
    public float shelfHeight = 1.0f; // the height of the shelf
    public float itemSpacing = 0.86f; // the spacing between items on the shelf

    private List<GameObject> itemsOnShelf = new List<GameObject>(); // list of items currently on the shelf

    private void Awake()
    {
        shelf = this.transform;
    }

    public void PlaceItemOnShelf(GameObject item)
    {
        itemsOnShelf.Clear();
        foreach (Transform child in shelf)
        {
            if (child.gameObject.activeInHierarchy)
            {
                itemsOnShelf.Add(child.gameObject);
            }

        }

        float xPosition = ((itemsOnShelf.Count - 1) * itemSpacing) - (shelfWidth / 2) + itemSpacing;
        Vector3 position = new Vector3(xPosition, -0.15f, 0);
        item.layer = 0;
        if (item.transform.parent != null)
            item.transform.parent = null;
        item.transform.parent = shelf;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localPosition = new Vector3(xPosition,-0.4f,0);
        item.transform.localScale =new Vector3(0.2f, 0.5f, 0.5f);
    }


    public void GetItem(Transform selecteditem)
    {
        if (transform.childCount > 0)
        {
            // Get the last child (bottom-most item)
            selecteditem = transform.GetChild(transform.childCount - 1);
            selecteditem.parent = null; // Set parent to null to pick up the item
        }
        else
        {
            // Handle the case where there are no items on the shelf (optional)
            Debug.Log("There are no items on the shelf to pick up.");
        }
    }

}
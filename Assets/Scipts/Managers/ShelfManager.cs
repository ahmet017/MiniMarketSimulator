using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShelfManager : MonoBehaviour
{
    public Foods ActiveFood;
    public Transform shelf; // the shelf game object
    public float shelfHeight = 1.0f; // the height of the shelf
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
                ActiveFood = child.GetComponent<ItemId>().Item;
            }
        }

        // If there are no child objects, set ActiveFood to null
        if (itemsOnShelf.Count == 0)
        {
            ActiveFood = null;
        }
        Foods SelectedItem = item.GetComponent<ItemId>().Item;
        if ((SelectedItem == ActiveFood || ActiveFood == null) && itemsOnShelf.Count < SelectedItem.MaxItemCountOnShelf)
        {
            
            int currentRow = itemsOnShelf.Count / (SelectedItem.MaxItemCountOnShelf/ 2);
            int itemsInRow = itemsOnShelf.Count % (SelectedItem.MaxItemCountOnShelf / 2);
            float xPosition = SelectedItem.Shelfwidth / 2 - (itemsInRow * SelectedItem.ItemSpacing / 2) - (itemsInRow * SelectedItem.ItemSpacing / 2);
            //Debug.Log(xPosition + "ve " + ((itemsOnShelf.Count) * SelectedItem.ItemSpacing));

            item.layer = 0;
            item.transform.parent = shelf;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localPosition = new Vector3(xPosition, -0.58f, (currentRow % 2 == 0) ? -0.28f : 0.28f);
            item.transform.localScale = SelectedItem.scale;

            if(item.tag != "Item")
            {
                FirstPersonController.Instance.DropItem(0);
            }

            item.gameObject.tag = "Untagged";
        }

        else
        {
            Debug.Log("raf dolu");          
        }
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] Objects;
    public static GameManager instance;
    public float Money;
    public TextMeshProUGUI MoneyText;
    public bool[] orderPosIsFull;
    public GameObject NPC;
    [SerializeField] private Vector3 InstantiatePos;
    private void Awake()
    {
        Money = PlayerPrefs.GetFloat("Money");
        MoneyText = GameObject.Find("MoneyCountText").GetComponent<TextMeshProUGUI>();
        MoneyText.text = "$ " + Money.ToString();

        Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);
        //NpcSpawner();
        if(instance == null )
            instance = this;
    }
    private void Update()
    {
        MouseActivator();
    }
    public void ChangeMoney()
    {
        PlayerPrefs.SetFloat("Money", Money);
        MoneyText.text = "$ " + Money.ToString();

    }

    public void NpcSpawner()
    {
        StartCoroutine(CreateNewNPC());
    }
    public IEnumerator CreateNewNPC()
    {
        yield return new WaitForSeconds(2);

        if (orderPosIsFull[0] == false)
        {
            Instantiate(NPC, InstantiatePos, Quaternion.identity);
            orderPosIsFull[0] = true;
        }
        else if (orderPosIsFull[1] == false)
        {
            Instantiate(NPC, InstantiatePos, Quaternion.identity);
            orderPosIsFull[1] = true;
        }
        StartCoroutine(CreateNewNPC());

    }

    public void OrderPos1()
    {
        orderPosIsFull[0] = false;
    }
    public void OrderPos2()
    {
        orderPosIsFull[1] = false;
    }
    private bool mouseActive;
    public Texture2D customCursor;
    public GameObject Phone;
    public bool PhoneBool = false;
    public void MouseActivator()
    {
        // Check if the specified key is pressed
        if (Input.GetKeyDown(KeyCode.P)) // Replace "YourKey" with the desired key
        {
            PhoneBool = !PhoneBool;
            Phone.SetActive(PhoneBool);

            // Toggle mouse active state
            mouseActive = !mouseActive;

            // Enable/disable cursor visibility based on mouseActive state
            Cursor.visible = mouseActive;
            Cursor.lockState = CursorLockMode.None;
            // Enable/disable input for mouse movement and clicks
            //Input.simulateMousePosition = !mouseActive;
        }

    }
}

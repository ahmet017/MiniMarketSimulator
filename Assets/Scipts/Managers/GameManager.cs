using StarterAssets;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
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

    public GameObject MonitorCamera;
    public GameObject targetCameraPos;

    public GameObject[] Furnitures;

    private void Awake()
    {
        //Money = PlayerPrefs.GetFloat("Money");
        //MoneyText = GameObject.Find("MoneyCountText").GetComponent<TextMeshProUGUI>();
        //MoneyText.text = "$ " + Money.ToString();

        Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);
        //NpcSpawner();
        if (instance == null)
            instance = this;
    }
    private void Update()
    {
        MouseActivator();

        if (MonitorCamera.activeSelf)
        {
            MonitorCamera.transform.position = Vector3.Lerp(MonitorCamera.transform.position, targetCameraPos.transform.position, 0.05f);
            MonitorCamera.transform.rotation = Quaternion.Slerp(MonitorCamera.transform.localRotation, targetCameraPos.transform.localRotation, 0.05f);
        }
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
            MonitorCamera.SetActive(!MonitorCamera.activeSelf);
            MonitorCamera.transform.position = FirstPersonController.Instance.CinemachineCameraTarget.transform.position;


            //PhoneBool = !PhoneBool;
            //Phone.SetActive(PhoneBool);
            mouseActive = !mouseActive;

            Cursor.visible = mouseActive;
            Cursor.lockState = CursorLockMode.None;

        }

    }


    public void testButton()
    {
        Debug.Log("click");
    }
}

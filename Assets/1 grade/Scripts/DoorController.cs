using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool trigger1Active = false;
    private bool trigger2Active = false;
    private bool trigger3Active = false;
    private bool doorOpened = false;

    public GameObject door;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        door.SetActive(true);
    }

    void Update()
    {
        if (!doorOpened && trigger1Active && trigger2Active && trigger3Active)
        {
            Debug.Log("문 열림");
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        door.SetActive(false);
    }

    public void SetTrigger1(bool active)
    {
        trigger1Active = active;
    }

    public void SetTrigger2(bool active)
    {
        trigger2Active = active;
    }

    public void SetTrigger3(bool active)
    {
        trigger3Active = active;
    }
}

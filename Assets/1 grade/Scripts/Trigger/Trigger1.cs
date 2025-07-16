using Mono.Cecil;
using UnityEngine;

public class Trigger1 : MonoBehaviour
{
    public DoorController doorController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            doorController.SetTrigger1(true);
            gameObject.SetActive(false);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

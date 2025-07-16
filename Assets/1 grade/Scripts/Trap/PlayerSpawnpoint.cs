using UnityEngine;

public class PlayerSpawnpoint : MonoBehaviour
{
    private Vector3 spawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
    }
}

using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    TestMove test;
    private void Start()
    {
        test = GetComponent<TestMove>();
    }
    private void Update()
    {
        test.MovementHandle();
    }
}

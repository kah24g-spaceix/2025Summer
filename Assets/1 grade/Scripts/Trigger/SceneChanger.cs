using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public SceneFader sceneFader;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sceneFader.FadeOutAndLoadScene("Stage3"); // "NextSceneName" should be replaced with the actual scene name you want to load
        }
    }
}

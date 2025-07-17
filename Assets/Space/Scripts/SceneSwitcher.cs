using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public Button yourButton;
    public SceneFader sceneFader;
    public string sceneName;

    void Start()
    {
        yourButton = GetComponent<Button>();
        if (yourButton != null)
        {
            yourButton.onClick.AddListener(AttemptSceneChange);
        }
    }

    void AttemptSceneChange()
    {
        if (Random.value <= 0.005f) // 0.5% chance
        {
            sceneFader.FadeOutAndLoadScene(sceneName);
        }
    }
}

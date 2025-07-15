using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject Menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnResumeButtonClicked()
    {
        Menu.SetActive(false);
    }

    private void OnOptionsButtonClicked()
    {
        
    }

    private void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}


using UnityEngine;
using UnityEngine.InputSystem;
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
        Menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Menu.SetActive(!Menu.activeSelf);
            Time.timeScale = 0;
        }
    }

    private void OnResumeButtonClicked()
    {
        Menu.SetActive(false);
        Time.timeScale = 1;
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


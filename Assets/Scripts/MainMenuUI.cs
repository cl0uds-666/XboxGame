using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;

    private void Update()
    {
        if (creditsPanel == null || !creditsPanel.activeSelf)
        {
            return;
        }

        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            CloseCredits();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1); // your game scene index
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0); // main menu scene index
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame called!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}

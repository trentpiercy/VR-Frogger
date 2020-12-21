using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject mainMenu;
    public JumpDetection jumpDetection;
    public GameObject heightCalibratedText;

    public static bool gameIsPaused = true;
    public delegate void PauseAction();
    public static event PauseAction OnPauseEvent;
    public delegate void ResumeAction();
    public static event ResumeAction OnResumeEvent;

    public bool pauseTest = false;

    private void Start()
    {
        if (jumpDetection == null)
        {
            jumpDetection = FindObjectOfType<JumpDetection>();
        }

        pauseMenu.SetActive(false);
        heightCalibratedText.SetActive(false);
    }

    void Update()
    {

        if (pauseTest)
        {
            ToggleMenu();
            pauseTest = false;
        }
    }

    public void ToggleMenu()
    {
        if (gameIsPaused)
            Resume();
        else Pause();
    }

    public void Pause()
    {
        //Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        gameIsPaused = true;
        OnPauseEvent?.Invoke();
    }

    public void Resume()
    {
        //Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        gameIsPaused = false;
        OnResumeEvent?.Invoke();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void CalibrateHeight()
    {
        jumpDetection.CalibrateHeight();
        StartCoroutine(showCalibratedText());
    }

    IEnumerator showCalibratedText()
    {
        heightCalibratedText.SetActive(true);
        yield return new WaitForSeconds(3f);
        heightCalibratedText.SetActive(false);
    }

    public void Restart()
    {
        Resume();
        DetectCollision.InvokeCollisionEvent();
    }

    public void MainMenuButton()
    {
        SceneManager.UnloadSceneAsync(MainMenu.activeLevelIndex);
        DetectCollision.InvokeCollisionEvent();
        gameIsPaused = true;
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
}

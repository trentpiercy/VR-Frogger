using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public JumpDetection jumpDetection;
    public GameObject heightCalibratedText;
    public GameObject calibrateWarnText;

    public static bool gameIsPaused = false;
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

        Pause();
        calibrateWarnText.SetActive(true);
        heightCalibratedText.SetActive(false);
    }

    void Update()
    {

        if (pauseTest)
        {
            Pause();
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
        calibrateWarnText.SetActive(false);
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
}

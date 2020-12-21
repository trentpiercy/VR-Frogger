using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject calibrateMenu;
    public GameObject mainMenu;
    public GameObject heightCalibratedText;
    public JumpDetection jumpDetection;

    public static int activeLevelIndex;

    void Start()
    {
        if (jumpDetection == null)
        {
            jumpDetection = FindObjectOfType<JumpDetection>();
        }

        calibrateMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void FirstCalibrateHeight()
    {
        calibrateMenu.SetActive(false);
        mainMenu.SetActive(true);
        CalibrateHeight();
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

    public void LoadLevel(int index)
    {
        activeLevelIndex = index;
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        mainMenu.SetActive(false);
        PauseMenu.gameIsPaused = false;
    }
}

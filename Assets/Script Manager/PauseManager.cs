using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;

    [Header("Loading Transition")]
    //public GameObject canvasUi;
    public Slider loadingBar;
    public GameObject loadingPanel;
    private float targetProgress;
    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
            loadingBar.value = 0f;
        }
    }

    public void TogglePause()
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }

    public void Cancel()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        //canvasUi.SetActive(false);
        loadingPanel.SetActive(true);
        StartCoroutine(LoadMenuScene());
    }
    private IEnumerator LoadMenuScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Night Light Demo");

        targetProgress = 0f;

        while (!asyncLoad.isDone)
        {
            targetProgress = asyncLoad.progress;
            float progress = Mathf.Lerp(loadingBar.value, targetProgress, Time.deltaTime * 5f);
            //Debug.Log("Loading progress: " + progress);
            loadingBar.value = progress;

            yield return null;
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Loading Panel")]
    public Slider loadingBar;
    public GameObject loadingPanel;
    private float targetProgress;

    void Start()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }
    public void TimeModebtn()
    {
        GameManagers.instance.timeMode = true;
        GameManagers.instance.scoreMode = false;
        GameManagers.instance.survivalMode = false;
        GameManagers.instance.SaveGameModes();

        //Debug.Log("timeMode value after setting: " + GameManagers.instance.timeMode);
        StartLoadingTDMRoomScene();
    }
    public void ScoreModebtn()
    {
        GameManagers.instance.scoreMode = true;
        GameManagers.instance.timeMode = false;
        GameManagers.instance.survivalMode = false;
        GameManagers.instance.SaveGameModes();
        StartLoadingTDMRoomScene();
    }
    public void SurvivalModebtn()
    {
        GameManagers.instance.survivalMode = true;
        GameManagers.instance.timeMode = false;
        GameManagers.instance.scoreMode = false;
        GameManagers.instance.SaveGameModes();
        StartLoadingTDMRoomScene();
    }

    public void StartLoadingTDMRoomScene()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadTDMRoomScene());
    }

    private IEnumerator LoadTDMRoomScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TDMRoom");

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

    public void QuitApplication()
    {
        Application.Quit();
    }
}

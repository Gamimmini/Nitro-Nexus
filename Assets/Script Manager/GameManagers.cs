using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagers : MonoBehaviour
{
    public ScoreManager scoreManager;
    public static GameManagers instance;

    [Header("Modes")]
    public bool timeMode;
    public bool scoreMode;
    public bool survivalMode;

    [Header("Survival Mode")]
    public int survivalScoreLimit = 11;


    [Header("Time Mode")]
    public float gameTimeInSeconds = 300f;
    public Image countdownFillImage;
    private float initialGameTimeInSeconds;
    public Text countdownText;
    public GameObject countdownOject;

    [Header("End Game Panel")]
    public GameObject endGamePanel;

    [Header("Loading Panel")]
    private float targetProgress;
    public Slider loadingBar;
    public GameObject loadingPanel;
    private void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        if (scoreManager != null)
        {
            LoadGameModes();
            initialGameTimeInSeconds = gameTimeInSeconds;
            loadingPanel.SetActive(false);
            endGamePanel.SetActive(false);
            if (!timeMode)
            {
                countdownOject.SetActive(false);
            }
            else
            {
                countdownOject.SetActive(true);
            }
        }
        if (loadingBar != null)
        {
            loadingBar.value = 0f;
        }

    }

    private void Update()
    {
        if (scoreMode && scoreManager != null)
        {
            StartCoroutine(WinOrLose());
        }

        if (timeMode && scoreManager != null)
        {
            gameTimeInSeconds -= Time.deltaTime;

            if (countdownFillImage != null)
            {
                float fillAmount = Mathf.Clamp01(gameTimeInSeconds / initialGameTimeInSeconds);
                countdownFillImage.fillAmount = fillAmount;
            }

            if (countdownText != null)
            {
                int minutes = Mathf.FloorToInt(gameTimeInSeconds / 60);
                int seconds = Mathf.CeilToInt(gameTimeInSeconds % 60);
                countdownText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            }

            if (gameTimeInSeconds <= 0f)
            {
                StartCoroutine(EndGameByTime());
            }
        }

        if (survivalMode && scoreManager != null)
        {
            if (scoreManager.kills >= survivalScoreLimit || scoreManager.enemyKills >= survivalScoreLimit)
            {
                if (scoreManager.kills >= scoreManager.enemyKills)
                {
                    WinGame("Blue Team Victory", "Kills", scoreManager.kills);
                }
                else
                {
                    WinGame("Red Team Victory", "enemyKills", scoreManager.enemyKills);
                }
            }
        }
    }

    public void SaveGameModes()
    {
        PlayerPrefs.SetInt("ScoreMode", scoreMode ? 1 : 0);
        PlayerPrefs.SetInt("TimeMode", timeMode ? 1 : 0);
        PlayerPrefs.SetInt("SurvivalMode", survivalMode ? 1 : 0);
    }

    public void LoadGameModes()
    {
        scoreMode = PlayerPrefs.GetInt("ScoreMode", 0) == 1;
        timeMode = PlayerPrefs.GetInt("TimeMode", 0) == 1;
        survivalMode = PlayerPrefs.GetInt("SurvivalMode", 0) == 1;
    }
    IEnumerator WinOrLose()
    {
        if (scoreManager != null)
        {
            scoreManager.playerKillCounter.text = "" + scoreManager.kills;
            scoreManager.enemyKillCounter.text = "" + scoreManager.enemyKills;

            if (scoreManager.kills >= 500)
            {
                WinGame("Blue Team Victory", "Kills", scoreManager.kills);
            }
            else if (scoreManager.enemyKills >= 500)
            {
                WinGame("Red Team Victory", "enemyKills", scoreManager.enemyKills);
            }
        }

        yield return null;
    }

    IEnumerator EndGameByTime()
    {
        if (scoreManager != null)
        {
            if (scoreManager.kills > scoreManager.enemyKills)
            {
                WinGame("Blue Team Victory", "Kills", scoreManager.kills);
            }
            else if (scoreManager.enemyKills > scoreManager.kills)
            {
                WinGame("Red Team Victory", "enemyKills", scoreManager.enemyKills);
            }
            else
            {
                WinGame("It's a Draw!", "Total Kills", scoreManager.enemyKills);
            }
        }


        yield return null;
    }

    void WinGame(string victoryText, string playerPrefsKey, int score)
    {
        if (scoreManager != null)
        {
            scoreManager.mainText.text = victoryText;
            PlayerPrefs.SetInt(playerPrefsKey, score);

            if (endGamePanel != null)
            {
                endGamePanel.SetActive(true);
            }
            Time.timeScale = 0f;
        }

    }
    public void ReplayGame()
    {
        Time.timeScale = 1f;
        loadingPanel.SetActive(true);
        //SceneManager.LoadScene("TDMRoom");
        StartCoroutine(Restart());
    }
    private IEnumerator Restart()
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
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
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
    IEnumerator RestartSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : GamiManagers
{
    [Header("Scene Manager")]
    public int kills;
    public int enemyKills;
    public Text playerKillCounter;
    public Text enemyKillCounter;
    public Text mainText;

    protected override void Awake()
    {
        base.Awake();
        if(PlayerPrefs.HasKey("Kills"))
        {
            kills = PlayerPrefs.GetInt("0");
        }    
        else if(PlayerPrefs.HasKey("enemyKills"))
        {
            enemyKills = PlayerPrefs.GetInt("0");
        }
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPlayerKillCounter();
        this.LoadEnemyKillCounter();
        //this.LoadMainText();
    }


    
    protected virtual void LoadPlayerKillCounter()
    {
        if (this.playerKillCounter != null) return;

        GameObject playerScoreObject = GameObject.Find("PlayerScore");

        if (playerScoreObject != null)
        {
            Transform playerScoreCounterTransform = playerScoreObject.transform.Find("PlayerScoreCounter");

            if (playerScoreCounterTransform != null)
            {
                this.playerKillCounter = playerScoreCounterTransform.GetComponent<Text>();

                if (this.playerKillCounter == null)
                {
                    Debug.LogError("Text component not found in PlayerScoreCounter.");
                }
            }
            else
            {
                Debug.LogError("PlayerScoreCounter not found as a child of PlayerScore.");
            }
        }
        else
        {
            Debug.LogError("PlayerScore not found in the hierarchy.");
        }
    }

    protected virtual void LoadEnemyKillCounter()
    {
        if (this.enemyKillCounter != null) return;

        GameObject enemyScoreObject = GameObject.Find("EnemyScore");

        if (enemyScoreObject != null)
        {
            Transform enemyScoreCounterTransform = enemyScoreObject.transform.Find("EnemyScoreCounter");

            if (enemyScoreCounterTransform != null)
            {
                this.enemyKillCounter = enemyScoreCounterTransform.GetComponent<Text>();

                if (this.enemyKillCounter == null)
                {
                    Debug.LogError("Text component not found in EnemyScoreCounter.");
                }
            }
            else
            {
                Debug.LogError("EnemyScoreCounter not found as a child of EnemyScore.");
            }
        }
        else
        {
            Debug.LogError("EnemyScore not found in the hierarchy.");
        }
    }

    protected virtual void LoadMainText()
    {
        if (this.mainText != null) return;

        GameObject winLoseBoardObject = GameObject.Find("WinLoseBoard");

        if (winLoseBoardObject != null)
        {
            Transform mainTextTransform = winLoseBoardObject.transform.Find("MainText");

            if (mainTextTransform != null)
            {
                this.mainText = mainTextTransform.GetComponent<Text>();

                if (this.mainText == null)
                {
                    Debug.LogError("Text component not found in MainText.");
                }
            }
            else
            {
                Debug.LogError("MainText not found as a child of WinLoseBoard.");
            }
        }
        else
        {
            Debug.LogError("WinLoseBoard not found in the hierarchy.");
        }
    }
    void Update()
    {
        UpdateKillCounters();
    }
    public void UpdateKillCounters()
    {
        if (playerKillCounter != null)
        {
            playerKillCounter.text = kills.ToString();
        }

        if (enemyKillCounter != null)
        {
            enemyKillCounter.text = enemyKills.ToString();
        }
    }

    /*
    private void Update()
    {
        StartCoroutine(WinOrLose());
    }
    IEnumerator WinOrLose()
    {
        playerKillCounter.text = "" + kills;
        enemyKillCounter.text = "" + enemyKills;

        if(kills >= 500)
        {
            mainText.text = "Blue Team Victory";
            PlayerPrefs.SetInt("Kills ", kills);
            Time.timeScale = 0f;
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene("TDMRoom");
        }    
        else if (enemyKills >=500)
        {
            mainText.text = "Red Team Victory";
            PlayerPrefs.SetInt("enemyKills", enemyKills);
            Time.timeScale = 0f;
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene("TDMRoom");
        }    
    }    
    */
}

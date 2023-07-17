using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject startBtn;
    [SerializeField] GameObject winGameUI;
    [SerializeField] GameObject loseGameUI;
    [SerializeField] TextMeshProUGUI goldTMPWinPanel;
    [SerializeField] TextMeshProUGUI levelTMPMain;
    [SerializeField] TextMeshProUGUI levelTMPWinScreen;

    int currentGold;
    public int goldAtLevelWin;

    bool isGameEnd;

    protected override void Awake()
    {
        base.Awake();
        int lastCompleteLevel = PlayerPrefs.GetInt("lastCompleteLevel", 0);
        int targetLevel = lastCompleteLevel + 1;
        int activeLevel = int.Parse(SceneManager.GetActiveScene().name.Split(" ")[1]);

        if (activeLevel != targetLevel)
            SceneManager.LoadScene("Scene " + targetLevel);

        levelTMPMain.SetText("LEVEL " + activeLevel.ToString());
    }

    public void StartGame()
    {
        levelTMPMain.gameObject.SetActive(false);
        Market.Instance.gameObject.SetActive(false);
        startBtn.SetActive(false);
        EnemySpawner.Instance.StartSpawn();
        Tower.Instance.GetReadyToStart();
        MergeTable.Instance.CloseTable();

        foreach (var mergeTableArea in MergeTable.Instance.mergeTableAreas)
        {
            if (mergeTableArea.Gun != null)
            {
                mergeTableArea.Gun.isActive = false;
                mergeTableArea.Gun.transform.position = mergeTableArea.Gun._startPos;
            }
                
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EndGame()
    {
        if (isGameEnd)
            return;

        isGameEnd = true;
        foreach(var gun in FindObjectsOfType<GunBase>())
            gun.enabled = false;
        if (Tower.Instance.Health <= 0)
        {
            Invoke(nameof(LoseGame), 1f);
        }
        else
        {
            SaveWinGame();
            Invoke(nameof(WinGame), 1f);
        }
    }

    private void LoseGame()
    {
        AudioManager.Instance.PlayEndGame(false);
        loseGameUI.SetActive(true);
    }

    private void SaveWinGame()
    {
        AudioManager.Instance.PlayEndGame(true);
        Tower.Instance.SaveTower();
        currentGold = PlayerManager.Instance.Gold;
        PlayerManager.Instance.LevelEndGold = goldAtLevelWin + currentGold;
        PlayerManager.Instance.SavePlayer();
        Market.Instance.SaveMarket();
        MergeTable.Instance.SaveMergeTable();
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Split(" ")[1]);
        levelTMPWinScreen.SetText("LEVEL " + currentLevel.ToString() + " COMPLETE!");
        PlayerPrefs.SetInt("lastCompleteLevel", currentLevel);
        PlayerPrefs.Save();
    }

    private void WinGame()
    {
        winGameUI.SetActive(true);
        StartCoroutine(GoldChangeAnim(currentGold));
    }

    IEnumerator GoldChangeAnim(int startGold)
    {
        goldTMPWinPanel.SetText(startGold.ToString());
        int targetGold = PlayerManager.Instance.LevelEndGold;
        while (startGold < targetGold)
        {
            startGold++;
            goldTMPWinPanel.SetText(startGold.ToString());
            yield return new WaitForSeconds(0.04f);
        }
    }

    public void NextLevel()
    {
        int activeLevel = int.Parse(SceneManager.GetActiveScene().name.Split(" ")[1]);
        int targetLevel = activeLevel + 1;
        SceneManager.LoadScene("Scene " + targetLevel); 
    }
}

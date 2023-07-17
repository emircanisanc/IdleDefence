using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : Singleton<PlayerManager>
{
    const int START_GOLD = 20;

    [SerializeField] TextMeshProUGUI goldTMP;

    public int Gold {
        get {
            return gold;
        }
        set {
            gold = value;
            goldTMP.SetText(gold.ToString());
        }
    }
    int gold;

    public int LevelEndGold { get; set; }


    protected override void Awake()
    {
        base.Awake();
        LoadPlayer();
        goldTMP.SetText(gold.ToString());
    }

    public void SavePlayer()
    {
        PlayerPrefs.SetInt("playerGold", LevelEndGold);
    }

    public void LoadPlayer()
    {
        gold = PlayerPrefs.GetInt("playerGold", START_GOLD);
    }

}

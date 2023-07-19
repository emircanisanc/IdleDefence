using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Tower : Singleton<Tower>
{
    List<TowerPart> towerParts;
    [SerializeField] GameObject explosionParticle;
    [SerializeField] GameObject fireParticle;
    [SerializeField] Transform towerTopPart;
    [SerializeField] Transform camOrigin;
    [SerializeField] GameObject towerPartPF;
    [SerializeField] Slider towerHealthFillBar;
    [SerializeField] TextMeshProUGUI towerLevelText;
    [SerializeField] TextMeshProUGUI towerHealthText;
    [SerializeField] MarketItemListSO marketItemListSO;
    [SerializeField] Transform towerHealthCanvas;

    int level = 1;
    public int healthMax = 100;
    public int healthMultiplierPerLevel;

    public int Health => health;

    int maxHealth;
    int health;

    Vector3 topPartLocalPos;    

    protected override void Awake()
    {
        base.Awake();
        towerHealthFillBar.value = 1;

        topPartLocalPos = towerTopPart.localPosition;
        LoadTower();
    }

    public void GetReadyToStart()
    {
        RestoreHealth();
        foreach (var towerPart in towerParts)
        {
            foreach (var gunPart in towerPart.gunParts)
            {
                if (gunPart.Gun != null)
                    gunPart.Gun.isActive = false;
            }
        }
    }


    public void RestoreHealth()
    {
        maxHealth = healthMax + healthMultiplierPerLevel * (level - 1);
        health = maxHealth;

        towerHealthText.SetText(health.ToString() + " / " + maxHealth.ToString());
    }

    public void ApplyDamage(int damage)
    {
        if (health <= 0)
            return;

        health -= damage;
        if ((float) health / (float)maxHealth <= 0.5f)
        {
            fireParticle.SetActive(true);
        }
        towerHealthFillBar.value = (float) health / (float)maxHealth;
        towerHealthText.SetText(health.ToString() + " / " + maxHealth.ToString());
        AudioManager.Instance.PlayTowerDamageSound(transform.position);
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        foreach(var part in towerParts)
        {
            foreach (var gunPart in part.gunParts)
            {
                if (gunPart.Gun)
                {
                    gunPart.Gun.gameObject.SetActive(false);
                }
            }
        }
        AudioManager.Instance.PlayTowerExplosionSound(transform.position);
        Sequence sequence = DOTween.Sequence();
        explosionParticle.transform.SetParent(null);
        //sequence.Append(transform.DOShakePosition(1f, 0.4f, 5));
        sequence.Append(transform.DOMoveY(transform.position.y - 10, 5f));
        GameManager.Instance.EndGame();
        explosionParticle.SetActive(true);
        GameManager.Instance.EndGame();
    }

    public void Upgrade()
    {
        GameObject towerPartObj = Instantiate(towerPartPF, towerParts[towerParts.Count - 1].towerMergePoint);
        //towerPartObj.transform.localScale *= transform.localScale.x;
        towerPartObj.transform.localEulerAngles = Vector3.zero;
        towerPartObj.transform.localPosition = Vector3.zero;
        TowerPart towerPart = towerPartObj.GetComponent<TowerPart>();
        towerParts.Add(towerPart);
        level++;
        camOrigin.position = new Vector3(0f, (level - 1) * 0.84f, (level - 1) * -0.72f);
        towerHealthCanvas.localScale *= 1.3f;
        towerLevelText.SetText(level.ToString());
        towerTopPart.SetParent(towerPart.towerTopPoint);
        towerTopPart.localPosition = topPartLocalPos;

        RestoreHealth();
    }

    public void SaveTower()
    {
        int towerPartsCount = towerParts.Count;
        PlayerPrefs.SetInt("towerPartCount", towerPartsCount);

        for (int towerPartIndex = 0; towerPartIndex < towerPartsCount; towerPartIndex++)
        {
            TowerPart towerPart = towerParts[towerPartIndex];
            int gunPartCount = towerPart.gunParts.Length;
            for (int gunPartIndex = 0; gunPartIndex < gunPartCount; gunPartIndex++)
            {
                GunBase gun = towerPart.gunParts[gunPartIndex].Gun;
                string saveString;
                if (gun != null)
                    saveString = gun.type + " " + gun.level;
                else
                    saveString = "null";
                PlayerPrefs.SetString(towerPartIndex + "gunPartAt" + gunPartIndex, saveString);
            }
        }

        PlayerPrefs.SetInt("towerLevelint", level);
    }

    public void LoadTower()
    {
        level = PlayerPrefs.GetInt("towerLevelint", 1);
        camOrigin.position = new Vector3(0f, (level - 1) * 0.84f, (level - 1) * -0.72f);
        towerParts = new List<TowerPart>();
        towerParts.Add(GetComponentInChildren<TowerPart>());
        int towerPartsCount = PlayerPrefs.GetInt("towerPartCount", 1);

        for (int towerPartIndex = 0; towerPartIndex < towerPartsCount; towerPartIndex++)
        {
            if (towerPartIndex > 0)
            {
                GameObject obj = Instantiate(towerPartPF, towerParts[towerPartIndex - 1].towerMergePoint);
                TowerPart towerPart = obj.GetComponent<TowerPart>();
                towerParts.Add(towerPart);
                towerPart.transform.localEulerAngles = Vector3.zero;
                for (int gunPartIndex = 0; gunPartIndex < 2; gunPartIndex++)
                {
                    string loadString = PlayerPrefs.GetString(towerPartIndex + "gunPartAt" + gunPartIndex, "null");
                    if (loadString != "null")
                    {
                        string gunType = loadString.Split(" ")[0];
                        int gunLevel = int.Parse(loadString.Split(" ")[1]);

                        GunItem gunData = marketItemListSO.gunItems.Find(x => x.typeName == gunType 
                        && x.level == gunLevel);

                        GameObject gunObj = Instantiate(gunData.prefab);
                        towerPart.gunParts[gunPartIndex].Gun = gunObj.GetComponent<GunBase>();
                        towerPart.gunParts[gunPartIndex].Gun.dropArea = towerPart.gunParts[gunPartIndex];
                        towerPart.gunParts[gunPartIndex].Gun.SetAttackMode();
                    }
                }
                towerTopPart.SetParent(towerPart.towerTopPoint);
                towerTopPart.localPosition = topPartLocalPos;
            }
            else
            {
                TowerPart towerPart = towerParts[0];
                for (int gunPartIndex = 0; gunPartIndex < 2; gunPartIndex++)
                {
                    string loadString = PlayerPrefs.GetString(towerPartIndex + "gunPartAt" + gunPartIndex, "null");
                    if (loadString != "null")
                    {
                        string gunType = loadString.Split(" ")[0];
                        int gunLevel = int.Parse(loadString.Split(" ")[1]);

                        GunItem gunData = marketItemListSO.gunItems.Find(x => x.typeName == gunType 
                        && x.level == gunLevel);

                        GameObject gunObj = Instantiate(gunData.prefab);
                        towerPart.gunParts[gunPartIndex].Gun = gunObj.GetComponent<GunBase>();
                        towerPart.gunParts[gunPartIndex].Gun.dropArea = towerPart.gunParts[gunPartIndex];
                        towerPart.gunParts[gunPartIndex].Gun.SetAttackMode();
                    }
                }
            }
        }

        RestoreHealth();
    }
}

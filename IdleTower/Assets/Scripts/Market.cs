using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Market : Singleton<Market>
{
    public MarketItemListSO marketItemList;
    public bool isMarketBtnVisible = false;
    
    Button[] buttons;

    int[] marketItemBuyAmount;
    int towerLevel;

    protected override void Awake()
    {
        base.Awake();
        buttons = GetComponentsInChildren<Button>();
        LoadMarket();
    }

    public void BuyTowerUpgrade()
    {
        int price = marketItemList.towerGoldsPerBuy[towerLevel];
        if (PlayerManager.Instance.Gold >= price)
        {
            PlayerManager.Instance.Gold -= price;
            towerLevel++;
            Tower.Instance.Upgrade();
            buttons[0].GetComponentInChildren<TextMeshProUGUI>().SetText(marketItemList.towerGoldsPerBuy[towerLevel].ToString());
        }
    }

    public void LoadMarket()
    {
        towerLevel = PlayerPrefs.GetInt("towerLevel", 0);
        buttons[0].GetComponentInChildren<TextMeshProUGUI>().SetText(marketItemList.towerGoldsPerBuy[towerLevel].ToString());
        marketItemBuyAmount = new int[marketItemList.marketItems.Count];
        int length = isMarketBtnVisible ? (buttons.Length - 1) : buttons.Length;
        for (int i = 0; i < length; i++)
        {
            marketItemBuyAmount[i] = PlayerPrefs.GetInt("gun" + i + "buyAmount", 0);
            MarketItem marketItem = marketItemList.marketItems[i];
            var btnIndex = isMarketBtnVisible ? (i + 1) : i;
            buttons[btnIndex].GetComponentInChildren<TextMeshProUGUI>().SetText(marketItem.goldsPerBuy[marketItemBuyAmount[i]].ToString());
        }
    }

    public void SaveMarket()
    {
        for (int i = 0; i < marketItemBuyAmount.Length; i++)
        {
            PlayerPrefs.SetInt("gun" + i + "buyAmount", marketItemBuyAmount[i]);
        }
        PlayerPrefs.SetInt("towerLevel", towerLevel);
    }

    public void BuyGun(int index)
    {
        MarketItem marketItem = marketItemList.marketItems[index];
        int sameGunCount = marketItemBuyAmount[index];
        if (PlayerManager.Instance.Gold >= marketItem.goldsPerBuy[sameGunCount])
        {
            MergeTableArea mergeTableArea;
            if (MergeTable.Instance.FirstEmpty(out mergeTableArea))
            {
                PlayerManager.Instance.Gold -= marketItem.goldsPerBuy[sameGunCount];
                marketItemBuyAmount[index] = sameGunCount + 1;
                GameObject obj = Instantiate(marketItem.prefab);
                GunBase gun = obj.GetComponent<GunBase>();
                gun.dropArea = mergeTableArea;
                mergeTableArea.Gun = gun;
                var btnIndex = isMarketBtnVisible ? index + 1 : index;
                buttons[btnIndex].GetComponentInChildren<TextMeshProUGUI>().SetText(marketItemList.marketItems[index].goldsPerBuy[marketItemBuyAmount[index]].ToString());
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GunList")]
public class MarketItemListSO : ScriptableObject
{
    public List<MarketItem> marketItems;
    public List<GunItem> gunItems;
    public int[] towerGoldsPerBuy;

}

[System.Serializable]
public class MarketItem
{
    public GameObject prefab;
    public int[] goldsPerBuy;
}


[System.Serializable]
public class GunItem
{
    public string typeName;
    public int level;
    public GameObject prefab;
}
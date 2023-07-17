using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeTable : Singleton<MergeTable>
{
    [HideInInspector] public List<MergeTableArea> mergeTableAreas;
    [SerializeField] MarketItemListSO marketItemListSO;
    [SerializeField] GameObject tableGFX;

    protected override void Awake()
    {
        base.Awake();
        mergeTableAreas = new List<MergeTableArea>(GetComponentsInChildren<MergeTableArea>());
        LoadMergeTable();
    }

    public bool FirstEmpty(out MergeTableArea mergeTableArea)
    {
        mergeTableArea = mergeTableAreas.Find(x => x.Gun == null);
        
        return mergeTableArea != null;
    }

    public void CloseTable()
    {
        tableGFX.SetActive(false);
        foreach (var mergeTableArea in mergeTableAreas)
        {
            if (mergeTableArea.Gun != null)
                mergeTableArea.Gun.gameObject.SetActive(false);
        }
    }

    public void SaveMergeTable()
    {
        for(int mergeTableIndex = 0; mergeTableIndex < mergeTableAreas.Count; mergeTableIndex++)
        {
            MergeTableArea mergeTableArea = mergeTableAreas[mergeTableIndex];
            string saveString;
            if (mergeTableArea.Gun == null)
            {
                saveString = "null";
            }
            else
            {
                saveString = mergeTableArea.Gun.type + " " + mergeTableArea.Gun.level;
            }
            PlayerPrefs.SetString("mergeTableArea" + mergeTableIndex, saveString);
        }
    }

    public void LoadMergeTable()
    {
        for(int mergeTableIndex = 0; mergeTableIndex < mergeTableAreas.Count; mergeTableIndex++)
        {
            MergeTableArea mergeTableArea = mergeTableAreas[mergeTableIndex];
            string loadString = PlayerPrefs.GetString("mergeTableArea" + mergeTableIndex, "null");
            if (loadString != "null")
            {
                string gunType = loadString.Split(" ")[0];
                int gunLevel = int.Parse(loadString.Split(" ")[1]);
                GunItem gunData = marketItemListSO.gunItems.Find(x => x.level == gunLevel && x.typeName == gunType);
                
                GameObject gunObj = Instantiate(gunData.prefab);
                mergeTableArea.Gun = gunObj.GetComponent<GunBase>();
                mergeTableArea.Gun.dropArea = mergeTableArea;
            }
        }
    }
}

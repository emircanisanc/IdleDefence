using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : DragAndDropAbstract
{
    public int level = 1;
    public string type = "Gun";
    public GameObject nextLevelPrefab;

    [HideInInspector] public IDropArea dropArea;
    bool isAttackModeActive = false;

    protected override void CheckIsMatch()
    {
        var camTransform = Camera.main.transform;
        var hits = Physics.RaycastAll(camTransform.position, (transform.position - camTransform.position).normalized);
        foreach(var hit in hits)
        {
            if (hit.collider.TryGetComponent<GunPart>(out var gunPart))
            {
                if (gunPart.Gun == null)
                {
                    if (dropArea) {
                        dropArea.Gun = null;
                    }
                    //isActive = false;
                    gunPart.Gun = this;
                    dropArea = gunPart;
                    _startPos = gunPart.transform.position;
                    SetAttackMode();
                    return;
                }
                else if (gunPart.Gun.level == level && gunPart.Gun != this && gunPart.Gun.type == type)
                {
                    if (dropArea) {
                        dropArea.Gun = null;
                    }
                    gunPart.Gun.LevelUp();
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    transform.position = _startPos;
                    return;
                }
            }

            if (hit.collider.TryGetComponent<MergeTableArea>(out var mergeTableArea))
            {
                if (mergeTableArea.Gun == null)
                {
                    if (dropArea) {
                        dropArea.Gun = null;
                    }
                    mergeTableArea.Gun = this;
                    dropArea = mergeTableArea;
                    _startPos = mergeTableArea.transform.position;
                    return;
                }
                else if (mergeTableArea.Gun != this && mergeTableArea.Gun.level == level && mergeTableArea.Gun.type == type)
                {
                    if (dropArea) {
                        dropArea.Gun = null;
                    }
                    mergeTableArea.Gun.LevelUp();
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    transform.position = _startPos;
                    return;
                }
            }
        }

       transform.position = _startPos;
    }


    void Update()
    {
        if (!isAttackModeActive)
            return;

        HandleGunUpdate();
    }

    protected virtual void HandleGunUpdate()
    {

    }


    public virtual void LevelUp()
    {
        level++;
        GameObject betterGunObj = Instantiate(nextLevelPrefab);
        
        GunBase betterGun = betterGunObj.GetComponent<GunBase>();

        IDropArea currentDropArea = dropArea;
        dropArea.Gun = null;
        dropArea = null;

        betterGun.dropArea = currentDropArea;
        currentDropArea.Gun = betterGun;

        if (isAttackModeActive)
            betterGun.SetAttackMode();

        UpgradeParticle.Instance.ShowParticle(betterGun.transform.position);
        
        Destroy(gameObject);
    }

    public virtual void SetAttackMode()
    {
        if (AudioManager.Instance)
            AudioManager.Instance.PlaySetGunActiveSound(transform.position);
        isAttackModeActive = true;
        //transform.localEulerAngles = Vector3.zero;
    }
    
}

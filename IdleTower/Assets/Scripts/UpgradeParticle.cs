using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeParticle : Singleton<UpgradeParticle>
{

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void ShowParticle(Vector3 position)
    {
        transform.position = position;
        AudioManager.Instance.PlayUpgradeSound(position);
        gameObject.SetActive(true);
        Invoke(nameof(Disable), 0.5f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}

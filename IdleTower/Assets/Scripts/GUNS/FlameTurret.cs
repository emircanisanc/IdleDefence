using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTurret : GunBase
{
    [SerializeField] Transform checkAreaTransform;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject fireParticle;
    [SerializeField] Transform turretRotatePoint;
    Collider[] colliders;
    EnemyController target;
    public float radius = 2f;
    bool isFiring;
    float timer;
    public float damage;
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();    
    }

    protected override void HandleGunUpdate()
    {
        base.HandleGunUpdate();

        

        if (target == null)
        {
            if (isFiring)
            {
                isFiring = false;
                audioSource.Stop();
                fireParticle.SetActive(false);
            }
            timer += Time.deltaTime;
            if (timer >= 0.6f)
            {
                timer = 0f;
                CheckArea();
            }
        }
        else
        {
            if (!target.isAlive)
            {
                target = null;
                return;
            }
            if (!isFiring)
            {
                audioSource.Play();
                isFiring = true;
                fireParticle.SetActive(true);
            }
            Vector3 dir = (target.transform.position - turretRotatePoint.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            turretRotatePoint.rotation = Quaternion.Lerp(turretRotatePoint.rotation, targetRot, Time.deltaTime * 10f);
            target.ApplyDamage((int)(damage * Time.deltaTime));
        }
        
    }

    void OnDisable()
    {
        fireParticle.SetActive(false);
        audioSource.Stop();
    }

    protected void CheckArea()
    {
        colliders = Physics.OverlapSphere(checkAreaTransform.position, radius, targetLayer);
        if (colliders.Length > 0)
        {
            foreach (var coll in colliders)
            {
                if (coll.gameObject.activeSelf)
                {
                    target = coll.GetComponent<EnemyController>();
                    return;
                }
            }
        }
        target = null;
    }

    public override void SetAttackMode()
    {
        base.SetAttackMode();
        checkAreaTransform.localPosition = new Vector3(checkAreaTransform.localPosition.x, -transform.position.y, checkAreaTransform.localPosition.z);
    }
}

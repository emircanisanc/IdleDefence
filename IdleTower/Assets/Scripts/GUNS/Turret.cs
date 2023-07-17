using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class Turret : GunBase
{
    [SerializeField] Transform checkAreaTransform;
    [SerializeField] Transform firePoint;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject projectilePF;
    [SerializeField] Transform turretRotatePoint;
    [SerializeField] protected float fireDuration = 2f;

    [SerializeField] AudioClip fireClip;

    List<Projectile> projectiles;

    public float radius = 2f;

    Transform target;

    protected float nextFireTime;

    AudioSource audioSource;
    bool isFiring;
    

    Collider[] colliders;

    void Awake()
    {
        projectiles = new List<Projectile>();
        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < 10; i++)
        {
            var obj = Instantiate(projectilePF);
            obj.SetActive(false);
            projectiles.Add(obj.GetComponent<Projectile>());
        }
    }

    private bool GetFreeProjectile(out Projectile projectile)
    {
        projectile = projectiles.Find(x => !x.gameObject.activeSelf);
        return projectile != null;
    }

    protected override void HandleGunUpdate()
    {
        if (Time.time < nextFireTime)
            return;

        if (isFiring)
            return;

        CheckArea();

        if (target == null)
            return;

        isFiring = true;

        Vector3 dir = (target.position - turretRotatePoint.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        turretRotatePoint.DORotateQuaternion(targetRot, fireDuration / 2).OnComplete(() => Fire());
    }

    protected void Fire()
    {
        if (GetFreeProjectile(out var projectile))
        {
            if (fireClip)
            {
                audioSource.PlayOneShot(fireClip);
            }
            nextFireTime = Time.time + fireDuration / 2;
            Vector3 targetPos = target.position;
            target = null;
            projectile.FireProjectile(firePoint, targetPos);
            isFiring = false;
        }
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
                    target = coll.transform;
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
        //transform.localEulerAngles = Vector3.zero;
    }

}

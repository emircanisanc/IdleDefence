using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDropArea : MonoBehaviour
{
    public GunBase Gun {
        get {
            return gun;
        }
        set {
            gun = value;
            if (value != null) {
                gun.transform.position = transform.position;
                gun.transform.eulerAngles = transform.eulerAngles;
                //gun.transform.SetParent(transform);
            }
        }
    }

    private GunBase gun;
}

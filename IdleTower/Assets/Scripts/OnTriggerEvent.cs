using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public UnityEvent onTrigger;

    void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeSelf)
            return;
            
        if (other.TryGetComponent<GunBase>(out var gunBase))
        {
            onTrigger?.Invoke();
            gameObject.SetActive(false);
        }
    }
}

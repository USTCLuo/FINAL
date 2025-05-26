using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractTriggerListener : MonoBehaviour
{
    public UnityEvent onEnter;
    public UnityEvent onExit;

    public LayerMask layerMask;
    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & 1 << other.gameObject.layer) != 0)
        {
            onEnter?.Invoke();
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if ((layerMask.value & 1 << other.gameObject.layer) != 0)
        {
            onExit?.Invoke();
        }
    }
}

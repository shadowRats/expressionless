using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evnt : MonoBehaviour
{
    protected virtual void Start()
    {

    }

    public virtual void Interact(RaycastHit info)
    {
        print("nuh");
    }
}

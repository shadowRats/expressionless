using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuber : Evnt
{
    Rigidbody rb;

    readonly float force = 1000;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody>();
    }

    public override void Interact(RaycastHit info)
    {
        rb.AddForce(Vector3.up * force);
    }
}

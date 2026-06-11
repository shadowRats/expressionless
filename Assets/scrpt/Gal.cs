using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gal : Evnt
{

    protected override void Start()
    {
        base.Start();

    }

    public override void Interact(RaycastHit info)
    {
        StartCoroutine(Knife());


    }


    IEnumerator Knife()
    {
        yield return null;

    }
}

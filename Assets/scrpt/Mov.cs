using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mov : Lookie
{

    readonly float speed = 5;


    protected override void Start()
    {
        base.Start();
        cam = GetComponent<Camera>();
    }

    protected override void Update2()
    {


        if (Input.GetKey(KeyCode.Space))
        {
            Move(transform.up);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Move(-transform.up);
        }

        transform.localRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * sensitivity);
    }


    protected override void Move(Vector3 dir)
    {
        transform.position += speed * Time.deltaTime * dir;
    }


}

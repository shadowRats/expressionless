using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravMov : Lookie
{
    bool grounded;

    Rigidbody rb;
     
    readonly float force = 5, maxSpeed = 10, jump = 700, camSpeed = 4;

    Vector3 camPivot;
    
    protected override void Start()
    {
        base.Start();
        cam = GetComponentInChildren<Camera>();
        

        rb = GetComponent<Rigidbody>();
        camPivot = cam.transform.localPosition;
        Physics.gravity *= 2;
    }


    protected override void Update2()
    {
        Vector2 vel = new(rb.velocity.x, rb.velocity.z);

        if (vel.magnitude > maxSpeed)
        {
            vel = maxSpeed * vel.normalized;
            rb.velocity = new Vector3(vel.x, rb.velocity.y, vel.y);
        }

        transform.localRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, Input.GetAxis("Mouse X"), 0) * sensitivity);

        cam.transform.localRotation = Quaternion.Euler(cam.transform.localEulerAngles + new Vector3(-Input.GetAxis("Mouse Y"), 0, 0) * sensitivity);

        if (cam.transform.localEulerAngles.x < 180)
        {
            if (cam.transform.localEulerAngles.x > 89)
            {
                cam.transform.localRotation = Quaternion.Euler(89, 0, 0);
            }
        }
        else if (cam.transform.localEulerAngles.x < 271)
        {
            cam.transform.localRotation = Quaternion.Euler(271, 0, 0);
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            camDist += Input.mouseScrollDelta.y * camSpeed;

            if (camDist > camPivot.z)
            {
                camDist = camPivot.z;
            }
        }

        cam.transform.localPosition = camPivot + camDist * new Vector3(0, -Mathf.Sin(Mathf.Deg2Rad * cam.transform.localEulerAngles.x) * cam.transform.localScale.y, Mathf.Cos(Mathf.Deg2Rad * cam.transform.localEulerAngles.x) * cam.transform.localScale.z);

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(Vector3.up * jump);
        }
    }

    protected override void Move(Vector3 dir)
    {
        rb.AddForce(dir * force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }
}

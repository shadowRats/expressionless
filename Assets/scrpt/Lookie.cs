using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lookie : MonoBehaviour
{
    protected readonly float sensitivity = 2, reach = 10;

    Canvas canvas;
    protected Camera cam;

    readonly List<Transform> interactables = new();
    readonly List<Evnt> events = new();

    int hit = 0;

    bool paused = false;

    protected float camDist;

    protected virtual void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;


        interactables.Add(null);
        events.Add(GetComponent<Evnt>());


        canvas = FindObjectOfType<Canvas>();
    }

    void Update()
    {
        if (paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                paused = false;
            }
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                paused = true;
                return;
            }




            Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit info, reach + camDist, LayerMask.GetMask("Interactable"));

            if (info.transform != interactables[hit])
            {
                bool b = true;

                int i = 0;

                foreach (Transform t in interactables)
                {
                    if (t == info.transform)
                    {
                        b = false;
                        break;
                    }

                    i++;
                }

                hit = i;

                if (b)
                {
                    interactables.Add(info.transform);
                    events.Add(info.transform.GetComponent<Evnt>());
                }

                //show something!!!!

            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //events[hit].Interact(info);

                StartCoroutine(Knife(info));
            }



            if (Input.GetKey(KeyCode.W))
            {
                Move(transform.forward);
            }

            if (Input.GetKey(KeyCode.S))
            {
                Move(-transform.forward);
            }

            if (Input.GetKey(KeyCode.D))
            {
                Move(transform.right);
            }

            if (Input.GetKey(KeyCode.A))
            {
                Move(-transform.right);
            }

            Update2();

        }
    }

    protected virtual void Update2()
    {

    }

    protected virtual void Move(Vector3 dir)
    {

    }

    [SerializeField]
    Transform[] points;


    private IEnumerator Knife(RaycastHit hit)
    {
        Mesh mesh = (hit.collider as MeshCollider).sharedMesh;



        yield return null;
    }
}

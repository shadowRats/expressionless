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

    readonly float length = 1, height = 0.06f, width = 0.01f, offset = 0.02f;

    private IEnumerator Knife(RaycastHit hit)
    {
        Mesh mesh = (hit.collider as MeshCollider).sharedMesh;



        RaycastHit[] info;
        info = new RaycastHit[6];



        List<int> tris = new();
        tris.Add(hit.triangleIndex);

        List<Vector3> verts = new();

        for (int i = 0; i < info.Length; i++)
        {
            Vector3 from;

            if (i % 3 > 0)
            {
                float stepper = i - 3, stepper2 = -(i % 2 - 0.5f);
                from = stepper / Mathf.Abs(stepper) * width * cam.transform.right + stepper2 / Mathf.Abs(stepper2) * offset * cam.transform.up;
                
            }
            else
            {
                from = ((i - 1.5f) / 1.5f) * height * cam.transform.up;
            }

            Physics.Raycast(cam.transform.position + from, cam.transform.forward, out info[i], reach + camDist + length, LayerMask.GetMask("Interactable"));

            if (!tris.Contains(info[i].triangleIndex))
            {
                tris.Add(info[i].triangleIndex);
            }

            points[i].position = info[i].point;
            points[i].parent = hit.transform;
            verts.Add(points[i].localPosition);
        }

        if (tris.Count == 1)
        {
            int startPoint = 0;
            float dist = 3;

            for (int i = 1; i < verts.Count; i++)
            {
                float d = (mesh.vertices[tris[0] * 3] - verts[i]).magnitude;

                if (d < dist)
                {
                    startPoint = i;
                    dist = d;
                }

            }

            startPoint = (startPoint - 1 + verts.Count) % verts.Count;
            int point = startPoint;

            for (int i = 0; i < 3; i++)
            {
                Vector3[] norms;
                norms = new Vector3[3];

                for (int i2 = 0; i2 < norms.Length; i2++)
                {
                    norms[i2] = (mesh.vertices[tris[0] * 3 + i] - verts[(point + i2 - 1 + verts.Count) % verts.Count]).normalized;
                }

                float mag = (norms[0] - norms[2]).magnitude, magprev = (norms[1] - norms[0]).magnitude, magNext = (norms[1] - norms[2]).magnitude;
                if (mag < magprev || mag < magNext)
                {
                    startPoint = (startPoint + 1) % verts.Count;
                    point = startPoint;
                    i = -1;

                    yield return null;
                }
                else
                {
                    point = (point + 2) % verts.Count;
                }

            }

            for (int i = 0; i < 3; i++)
            {
                points[(startPoint + 2 * i) % verts.Count].localPosition = mesh.vertices[tris[0] * 3 + i];
            }
        }

        List<int> newTris = new();
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            if (!tris.Contains(mesh.triangles[i] / 3))
            {
                newTris.Add(mesh.triangles[i]);
            }
        }


    }
}

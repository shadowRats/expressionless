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
    MeshRenderer[] points;

    [SerializeField]
    Material[] colors;

    readonly float length = 1, height = 0.06f, width = 0.01f, offset = 0.02f;

    private IEnumerator Knife(RaycastHit hit)
    {
        MeshFilter mF = hit.transform.GetComponent<MeshFilter>();
        Mesh mesh = mF.mesh;

        RaycastHit[] info = new RaycastHit[6];

        List<int> tris = new();
        tris.Add(hit.triangleIndex);

        List<Vector3> verts = new();

        for (int i = 0; i < info.Length; i++)
        {
            points[i].material = colors[^1];

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

            points[i].transform.position = info[i].point;
            points[i].transform.parent = hit.transform;
            verts.Add(points[i].transform.localPosition);
        }

        if (tris.Count == 1 && verts.Count == 6)
        {

            int[] closest = new int[3];

            List<int>[] vertverts = new List<int>[3];

            for (int i = 0; i < 3; i++)
            {
                
                Vector2[] dists = new Vector2[6];

                closest[i] = 0;

                for (int i2 = 0; i2 < dists.Length; i2++)
                {
                    dists[i2] = mesh.vertices[tris[0] * 3 + i] - verts[i2];

                    if (dists[i2].magnitude < dists[closest[i]].magnitude)
                    {
                        closest[i] = i2;
                    }
                }

                vertverts[i] = new();
                vertverts[i].Add(closest[i]);

                bool contNeg = true;
                bool contPos = true;

                for (int i2 = 0; i2 < 3; i2++)
                {
                    if (contNeg)
                    {
                        if (Vector3.SignedAngle(dists[(closest[i] - i2 + 6) % 6], dists[(closest[i] - i2 + 5) % 6], hit.normal) < 0)
                        {
                            vertverts[i].Insert(0, (closest[i] - i2 + 5) % 6);
                        }
                        else
                        {
                            contNeg = false;
                        }
                    }

                    if (contPos)
                    {
                        if (Vector3.SignedAngle(dists[(closest[i] + i2) % 6], dists[(closest[i] + i2 + 1) % 6], hit.normal) > 0)
                        {
                            vertverts[i].Add((closest[i] + i2 + 1) % 6);
                        }
                        else
                        {
                            contPos = false;
                        }
                    }
                    
                }

            }

            for (int i = 0; i < 3; i++)
            {
                for (int i2 = 1; i2 < vertverts[i].Count; i2++)
                {
                    if (vertverts[(i + 2) % 3].Contains(vertverts[i][i2]))
                    {
                        vertverts[(i + 2) % 3].Remove(vertverts[i][i2]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            List<Vector3> newVerts = new(mesh.vertices);
            newVerts.AddRange(verts);

            List<int> newTris = new(mesh.triangles);

            foreach (int tri in tris)
            {
                for (int i = 0; i < 3; i++)
                {
                    newTris.RemoveAt(tri * 3);
                }
            }

            int start = mesh.vertexCount;

            for (int i = 0; i < 3; i++)
            {
                //clockwise
                newTris.Add(tris[0] * 3 + (i + 2) % 3);
                newTris.Add(tris[0] * 3 + i);
                newTris.Add(start + vertverts[i][0]);

                for (int i2 = 1; i2 < vertverts[i].Count; i2++)
                {
                    newTris.Add(start + vertverts[i][i2]);
                    newTris.Add(start + vertverts[i][i2 - 1]);
                    newTris.Add(tris[0] * 3 + i);
                }
            }


            mesh.Clear();

            mesh.vertices = newVerts.ToArray();
            mesh.triangles = newTris.ToArray();

            mesh.Optimize();

            mF.mesh = mesh;





            for (int i = 0; i < 3; i++)
            {
                for (int i2 = 0; i2 < vertverts[i].Count; i2++)
                {
                    int c = i;

                    for (int i3 = 0; i3 < i; i3++)
                    {
                        if (vertverts[i3].Contains(vertverts[i][i2]))
                        {
                            c += i3 + 2;
                        }
                    }

                    points[vertverts[i][i2]].material = colors[c];
                }
            }

        }





        yield return null;
    }
}

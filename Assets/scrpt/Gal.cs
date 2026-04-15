using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gal : Evnt
{

    Mesh mesh;

    readonly float length = 1, height = 0.2f, safeDist = 1;

    [SerializeField]
    Transform[] points;

    protected override void Start()
    {
        base.Start();

        mesh = GetComponent<MeshFilter>().mesh;

    }

    public override void Interact(RaycastHit info)
    {
        StartCoroutine(Knife(info));


    }


    IEnumerator Knife(RaycastHit info)
    {

        points[0].position = info.point;
        print(info.barycentricCoordinate);











        yield return null;
        /*
        List<int> hitTris = new();

        hitTris.Add(info.triangleIndex);

        print(info.normal);

        Vector2 d = new(info.normal.x, info.normal.z);
        Vector3 otherdir = new(d.normalized.x * info.normal.y, d.magnitude, d.normalized.y * info.normal.y);

        Physics.Raycast(info.point + info.normal * safeDist + height * otherdir, info.normal, out RaycastHit hit, length + safeDist, LayerMask.GetMask("Interactable"));
        Physics.Raycast(info.point + info.normal * safeDist - height * otherdir, info.normal, out RaycastHit hit2, length + safeDist, LayerMask.GetMask("Interactable"));

        points[0].position = hit.point;
        points[1].position = info.point;


        points[2].position = info.point + info.normal * safeDist;

        List<int> newTris = new();

        for (int i = 0; i < mesh.triangles.Length; i++)
        {

            if (!hitTris.Contains(i/3))
            {
                newTris.Add(mesh.triangles[i]);
            }
        }
        */

    }
}

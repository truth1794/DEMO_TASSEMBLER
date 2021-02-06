using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetPivotScript : MonoBehaviour {

    Vector3 p; //Pivot value -1..1, calculated from Mesh bounds
    Vector3 last_p; //Last used pivot

    GameObject obj; //Selected object in the Hierarchy
    MeshFilter meshFilter; //Mesh Filter of the selected object
    Mesh mesh; //Mesh of the selected object
    Collider col; //Collider of the selected object


    bool pivotUnchanged; //Flag to decide when to instantiate a copy of the mesh

    private void Start() {
        obj = GameObject.Find("GridGenerator").GetComponent<TileControl>().buffer0;
        foreach(Transform child in obj.transform) {
            mesh = child.GetComponent<Mesh>();
        }
        Init();
    }

    void Init() {
        RecognizeSelectedObject();
        if (obj) {
            if (mesh) {
                p = Vector3.zero;
                UpdatePivot();
            }
        }
    }

    void UpdatePivot() {
        Vector3 diff = Vector3.Scale(mesh.bounds.extents,last_p - p); //Calculate difference in 3d position
        foreach (Transform child in obj.transform) {
            child.transform.position -= Vector3.Scale(diff, child.transform.localScale);
        }
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < verts.Length; i++) {
            verts[i] += diff;
        }
        mesh.vertices = verts; //Assign the vertex array back to the mesh
        mesh.RecalculateBounds(); //Recalculate bounds of the mesh, for the renderer's sake
                                  //The 'center' parameter of certain colliders needs to be adjusted
                                  //when the transform position is modified
    }

    void UpdatePivotVector() {
        Bounds b = mesh.bounds;
        Vector3 offset = -1 * b.center;
        p = last_p = new Vector3(offset.x / b.extents.x, offset.y / b.extents.y, offset.z / b.extents.z);
    }

    //When a selection change notification is received
    //recalculate the variables and references for the new object
    void OnSelectionChange() {
        RecognizeSelectedObject();
    }

    //Gather references for the selected object and its components
    //and update the pivot vector if the object has a Mesh specified
    void RecognizeSelectedObject() {
        Transform t = null;
        foreach (Transform child in obj.GetComponentInChildren<Transform>()) {
            t = child.transform;
        }
        obj = t ? t.gameObject : null;
        if (obj) {
            meshFilter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
            mesh = meshFilter ? meshFilter.sharedMesh : null;
            if (mesh)
                UpdatePivotVector();
            col = obj.GetComponent(typeof(Collider)) as Collider;
            pivotUnchanged = true;
        }
        else {
            mesh = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateMeshHeight : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private int matIndex;
    
    private Mesh mesh;
    private float objectHeight;
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mesh = meshRenderer.sharedMesh;
        mat = meshRenderer.materials[matIndex];
    }

    // Update is called once per frame
    void Update()
    {
        objectHeight = mesh.bounds.size.y;
        mat.SetFloat("_ObjectHeight", objectHeight);
    }
}

using UnityEngine;
using System.Collections;

public enum nemiciTipo
{ 
    globlin1,
    globlin2,
    globlin3
}

public class GeneraPercorso : MonoBehaviour
{
    public Transform[] Percorso;
    public nemiciTipo nemico = nemiciTipo.globlin1;
    public int numPoint = 2;
    public float size = 0.1f;
    // Use this for initialization
    void Start()
    {
        for (int i = 1; i <= numPoint; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = i.ToString();
            cube.transform.localScale = new Vector3(size, size, size);
            Renderer mesh = cube.GetComponent<MeshRenderer>();
            mesh.material.color = Color.red;


        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
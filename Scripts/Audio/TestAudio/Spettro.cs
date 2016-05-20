using UnityEngine;
using System;
using System.Collections.Generic;


public class Spettro : MonoBehaviour {

    public Transform puntoA;
    public Transform puntoB;
    public Camera m_camera;
    public LineRenderer lineR;
    public AudioSource s_audio;
    public GameObject Prefab;
    public int Point;
    public int Objects = 10;
    public float Raggio = 5f;
    public Material mat;
    private float[] SpectromData = new float[1024];
    private Vector3 pos_;
    private int indice = 1;
    private String STR;
    private GameObject tmp;
    private List<GameObject> Objs = new List<GameObject>();
    private float r;
    private float g;
    private float b;

    void Start()
    {
        if (Objs.Count < Objects)
        {
            for (int i = 0; i < Objects; i++)
            {
                float Angolo = i * Mathf.PI * 2 / Objects;
                Vector3 pos = new Vector3(Mathf.Cos(Angolo), 0f, Mathf.Sin(Angolo)) * Raggio;
                tmp = Instantiate(Prefab, pos, Quaternion.identity) as GameObject;
                Objs.Add(tmp);
                Objs[i].SetActive(false);
            }
        }
        r = mat.color.r;
        g = mat.color.g;
        b = mat.color.b;
    }

    void Update () {
        s_audio.GetSpectrumData(SpectromData, 0, FFTWindow.BlackmanHarris);

        switch (indice)
        {
            case 1:
                STR = "Line";
                SpectrumLine();
                break;
            case 2:
                STR = "Cube";

                SpectrumCube();
                break;
        }
    }
    void SpectrumLine()
    {
        if (lineR.enabled == false)
            lineR.enabled = true;

        puntoA.position = m_camera.ViewportToWorldPoint(new Vector3(0f, 0f, m_camera.nearClipPlane));
        puntoB.position = m_camera.ViewportToWorldPoint(new Vector3(1f, 0f, m_camera.nearClipPlane));
        lineR.SetVertexCount(Point);
        lineR.SetPosition(0, puntoA.position);
        for (int i = 1; i < Point - 1; i++)
        {

            float Data = SpectromData[i];
            pos_ = m_camera.ViewportToWorldPoint(new Vector3(1f * i / 100, Data * 20, m_camera.nearClipPlane));
            lineR.SetPosition(i, pos_);
        }
        lineR.SetPosition(Point - 1, puntoB.position);
    }
    void SpectrumCube()
    {
        if(lineR.enabled == true)
        lineR.enabled = false;

        for (int i = 0; i < Objects; i++)
        {
            Objs[i].gameObject.GetComponent<Renderer>().material = mat;
            Objs[i].SetActive(true);
            Vector3 ScalaPrecedente = Objs[i].transform.localScale;
            ScalaPrecedente.y = Mathf.Lerp(ScalaPrecedente.y, SpectromData[i] * 40, Time.deltaTime * 30);
            Objs[i].transform.localScale = ScalaPrecedente;
        }
    }
    void OnGUI()
    {
        GUILayout.BeginHorizontal(GUILayout.Width(200), GUILayout.Height(200));
        GUILayout.Label(STR);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Width(200), GUILayout.Height(200));
        if (GUILayout.Button(new GUIContent("Cambia")))
        {
            indice++;
            indice = indice > 2 ? 1 : 2;
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(200), GUILayout.Height(200));

        GUILayout.Label("Red");
       r=  GUILayout.HorizontalSlider(r, 0f, 1f);
        GUILayout.Label("Green");
       g= GUILayout.HorizontalSlider(g, 0f, 1f);
        GUILayout.Label("Blue");
       b= GUILayout.HorizontalSlider(b, 0f, 1f);
       mat.SetColor ("_EmissionColor",new Color(r, g, b));
   
    GUILayout.EndVertical();
    }

}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;

[RequireComponent(typeof(SphereCollider))]

public class SpawnPoint : MonoBehaviour {

    public enum Sesso
    {
        maschio, femmina
    }

    public classiPersonaggi tipi;
    public Sesso sesso;
    public int quantita;
    public float tempo;
    public float raggio = 1f;
    //public List<Vector3> posizioni = new List<Vector3>();

    private SphereCollider spCollider;
    private GameObject instanza;
    private Vector3 localTransform;
    private RaycastHit hit;
    private Vector3 pos;
    private LayerMask terreno = 0 << 10;
    private float precX;
    private float precZ;
    private float randomX;
    private float randomZ;
    private bool giustaPosizione;
    private caratteristichePersonaggioV2 databaseInizialeProprieta;
    private DatiPersonaggio datip;
    private List<GameObject> listaOggettiAttivi = new List<GameObject>();

    void Awake()
    {
        spCollider = GetComponent<SphereCollider>();
        if (spCollider == null)
        {
            spCollider = gameObject.AddComponent<SphereCollider>();
            spCollider.isTrigger = true;
        }
        spCollider.radius = raggio;
        spCollider.isTrigger = true;
        if (databaseInizialeProprieta == null)
        {
            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig3))
            {
                string percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig3);
                databaseInizialeProprieta = AssetDatabase.LoadAssetAtPath<caratteristichePersonaggioV2>(percorso + Statici.STR_DatabaseDiGioco3);
            }
        }
    }
    //ausdioafpkafrpeqwrt9pfpjoi
    void Start()
    {
        terreno = ~terreno;
        for (int i = 0; i < databaseInizialeProprieta.matriceProprieta.Count; i++)
        {
            if (!databaseInizialeProprieta.matriceProprieta[i].giocabile && databaseInizialeProprieta.matriceProprieta[i].classe == tipi)//
            {
                if (sesso == Sesso.maschio)
                    instanza = Resources.Load(databaseInizialeProprieta.matriceProprieta[i].nomeM) as GameObject;
                else
                    instanza = Resources.Load(databaseInizialeProprieta.matriceProprieta[i].nomeF) as GameObject;
            }
        }
        StartCoroutine(Posizione());
    }
    void Update()
    {
        Debug.DrawLine(new Vector3(spCollider.transform.position.x + pos.x, spCollider.transform.position.y + raggio
, spCollider.transform.position.z + pos.z), new Vector3(spCollider.transform.position.x + pos.x
, spCollider.transform.position.y + -raggio, spCollider.transform.position.z + pos.z), Color.red);
    }

    IEnumerator Posizione()
    {
        while (true)
        {
            Randomizza();
            pos = new Vector3(randomX, 0f, randomZ);
            Ray ray = new Ray((new Vector3(spCollider.transform.position.x + pos.x, spCollider.transform.position.y + raggio
                , spCollider.transform.position.z + pos.z))
                , Vector3.down * (raggio * 2));
            if (Physics.Raycast(ray, out hit, int.MaxValue, terreno, QueryTriggerInteraction.Ignore) && giustaPosizione && listaOggettiAttivi.Count < quantita)// OK Niente 
            {
                if (hit.collider)
                {
                    Debug.Log("Colpito!");
                    localTransform = hit.point;
                    GameObject tmp = Instantiate(instanza, localTransform, Quaternion.identity) as GameObject;
                    listaOggettiAttivi.Add(tmp);
                }
            }

            else
            {
                Debug.Log("Controlla posizione dello Spawpoint");
            }
            yield return new WaitForSeconds(tempo);
        }
    }
    void Randomizza()
    {
        if (precX == randomX)
        {
            randomX = UnityEngine.Random.Range(-raggio, raggio);
        }
        if (precZ == randomZ)
        {
            randomZ = UnityEngine.Random.Range(-raggio, raggio);
        }
        precX = randomX;
        precZ = randomZ;
    }
    void OnTriggerStay(Collider coll)
    {
        if (coll)
            giustaPosizione = true;
        else
            giustaPosizione = false;
    }
}


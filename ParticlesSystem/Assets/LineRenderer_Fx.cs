using UnityEngine;
using System.Collections;

public class LineRenderer_Fx : MonoBehaviour {

    public Transform target;
    public int QuantitaSeg = 1;
    public int zigZag;
    public float variabileVel = 1f;
    public float scale;
    Vector3 Position;
    Vector3 offSet;
    private LineRenderer line;
    private Perlin noise;
    private float onOverZig;
    private const float val_1 = 0.12345f;
    private const float val_2 = 0.23456f;
    private const float val_3 = 0.34567f;
    void Start () {

        line = GetComponent<LineRenderer>();
        if (noise == null)
            noise = new Perlin();

        onOverZig = 1f / (float)zigZag;

        for (int i = 0; i < QuantitaSeg; i++)
        {
            line.SetVertexCount(i);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float timeX = Time.time * variabileVel* val_1;
        float timeY = Time.time * variabileVel * val_2;
        float timeZ= Time.time * variabileVel * val_3;
        for (int i = 0; i < QuantitaSeg;i++)
        {
            Position = Vector3.Lerp(transform.position, target.position, onOverZig * (float)i);
            offSet = new Vector3(noise.Noise(timeX + Position.x, timeY + Position.y, timeZ + Position.z),
                                          noise.Noise(timeX + Position.x, timeY + Position.y, timeZ + Position.z),
                                          noise.Noise(timeX + Position.x, timeY + Position.y, timeZ + Position.z));

            Position += (offSet * scale * ((float)i * onOverZig));
            line.SetPosition(i , Position);
        }

    }
}

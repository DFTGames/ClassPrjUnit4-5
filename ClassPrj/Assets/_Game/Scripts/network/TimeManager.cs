using UnityEngine;
using System;
using System.Collections;

// Synchronizes client time with the server time
public class TimeManager : MonoBehaviour
{
    private readonly float period = 3.0f;

    private static TimeManager instance;
    public static TimeManager Instance
    {
        get
        {
            return instance;
        }
    }

    private float lastRequestTime = float.MaxValue;
    private float timeBeforeSync = 0;
    private bool synchronized = false;

    private double lastServerTime = 0;
    private double lastLocalTime = 0;

    private bool running = false;

    private double averagePing = 0;
    private int pingCount = 0;

    private readonly int averagePingCount = 10;
    private double[] pingValues;
    private int pingValueIndex;

    void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        pingValues = new double[averagePingCount];
        pingCount = 0;
        pingValueIndex = 0;
        running = true;
    }

    public void Synchronize(double timeValue)
    { //TIME VALUE E' IL TEMPO QUADO VIENE INVIATO DAL SERVER (e' il tempo che c'e sul server quando viene fatto la richiesta)
      // Measure the ping in milliseconds
        double ping = (Time.time - timeBeforeSync) * 1000;  //QUA FA LA DIFFERENZA TRA IL TEMPO DI PARTENZA DA CLIENT (TIMEbEFOREsYNC) E IL TEMPO ATTUALE AL MOMENTO DELLA RICEZIONE (dovrebbe essere il Ping pong)
        CalculateAveragePing(ping);   //fa la media aritmetica degli ultimi 10 ping CLIENT-SERVER  e lo memorizza nel AveragePing

        // Take the time passed between server sends response and we get it 
        // as half of the average ping value
        double timePassed = averagePing / 2.0f;  //qua calcola il Ping...(ping Pong /2...quindi da 1 side)
        lastServerTime = timeValue + timePassed; ///e' il tempo del server stimato usando il dato che abbiamo nel ping
		lastLocalTime = Time.time;  //tempo attuale

        synchronized = true;
    }

    void Update()
    {
        if (!running) return;

        if (lastRequestTime > period)
        {
            lastRequestTime = 0;
            timeBeforeSync = Time.time;  //TEMPO DEL CLIENT PRIMA DELLA SINCRONIZZAZIONE SERVER
            ManagerNetwork.TimeSyncRequest(); //VIENE FATTO RICHIESTA AL SERVER ...IL QUALE APPENA LO RICEVE ..INVIA AL CLIENT IL SUO TEMPO...IL QUALE CLIENT RICHIAMA IL SYNCRONIZE QUA SOTTO
        }
        else
        {
            lastRequestTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Network time in msecs
    /// </summary>
    public double NetworkTime
    {    //COME DA DESCRIZIONE SOTTO IN INGLESE
        get
        {
            // Taking server timestamp + time passed locally since the last server time received			
            return (Time.time - lastLocalTime) * 1000 + lastServerTime;
        }
    }

    public double AveragePing
    {
        get
        {
            return averagePing;
        }
    }


    private void CalculateAveragePing(double ping)
    { //fa la media aritmetica degli ultimi 10 ping CLIENT-SERVER e lo memorizza nel AveragePing
        pingValues[pingValueIndex] = ping;
        pingValueIndex++;
        if (pingValueIndex >= averagePingCount) pingValueIndex = 0;
        if (pingCount < averagePingCount) pingCount++;

        double pingSum = 0;
        for (int i = 0; i < pingCount; i++)
        {
            pingSum += pingValues[i];
        }

        averagePing = pingSum / pingCount;
    }


}

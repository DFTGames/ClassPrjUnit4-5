using UnityEngine;
using System.Collections;



    public class Pattugliamento : IStato
    {

        public string Nome { get; set; }

        private int indiceDestinazioni = 0;
        private bool CambiaDirezione = false;
        private FSM MioCervello;
        private NavMeshAgent Agente;
        private Transform ProssimaDirezione;
        private Transform[] Destinazioni;
        private Animator Animatore;

        public void Esecuzione()
        {          
            if (Agente.remainingDistance <= Agente.stoppingDistance)
           
               {
                indiceDestinazioni = indiceDestinazioni < Destinazioni.Length - 1 ?
                    ++indiceDestinazioni : 0;
                ProssimaDirezione = Destinazioni[indiceDestinazioni];
                Agente.SetDestination(ProssimaDirezione.position);
              //  Agente.speed = 0.5f;
                CambiaDirezione = false;

            }
            Animatore.SetFloat("Velocita", 0.5f);
    }

        public void EsecuzioneTerminata()
        {

        }

        public void Inizializza(FSM oggetto)
        {

            MioCervello = oggetto;
            Agente = MioCervello.gameObject.GetComponent<NavMeshAgent>();
            Animatore = MioCervello.gameObject.GetComponent<Animator>();

            if (MioCervello.classeGoblin == 1)
            {
                Destinazioni = new Transform[GameObject.Find("GeneraPercorso").GetComponent
                    <GeneraPercorso>().Itinerario(TipoPercorso.A).Count];
                for (int i = 0; i < Destinazioni.Length; i++)
                {
                    Destinazioni[i] =
               GameObject.Find("GeneraPercorso").GetComponent<GeneraPercorso>().Itinerario(TipoPercorso.A)[i];
               }
            }
            else if (MioCervello.classeGoblin == 2)
            {
                Destinazioni = new Transform[GameObject.Find("GeneraPercoso").GetComponent
                    <GeneraPercorso>().Itinerario(TipoPercorso.B).Count];
                for (int i = 0; i < Destinazioni.Length; i++)
                {
                    Destinazioni[i] = 
                   GameObject.Find("GeneraPercorso").GetComponent<GeneraPercorso>().Itinerario(TipoPercorso.B)[i];
                }
            }
            else if (MioCervello.classeGoblin == 3)
                {
                    Destinazioni = new
                Transform[GameObject.Find("GeneraPercorso").GetComponent<GeneraPercorso>().Itinerario
                (TipoPercorso.C).Count];
                    for (int i = 0; i < Destinazioni.Length; i++)
                    {
                        Destinazioni[i] = GameObject.Find("GeneraPercorso").GetComponent<GeneraPercorso>
                        ().Itinerario(TipoPercorso.C)[i];

                    }
                }
              ProssimaDirezione = Destinazioni[indiceDestinazioni];
              Agente.SetDestination(ProssimaDirezione.position);  
        }

        public void PreparoEsecuzione()
        {

        }
    }


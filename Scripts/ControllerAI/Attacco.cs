public class Attacco : IStato
{
    public string Nome { get; set; }

    private FSM MioCervello;

    public void Inizializza(FSM oggetto)
    {
        MioCervello = oggetto;
    }

    public void PreparoEsecuzione()
    {
        if (MioCervello.TipoArma == TipoArma.Pugno)
            MioCervello.Animatore.SetBool("Pugno", true);
        else
        {
            MioCervello.Animatore.SetBool("PrendiArco", true);
            MioCervello.Animatore.SetBool("TiraFreccie", true);
        }
        MioCervello.Agente.stoppingDistance = MioCervello.DistanzaAttacco;
    }

    public void Esecuzione()
    {
        MioCervello.Agente.SetDestination(MioCervello.ObiettivoNemico.position);
    }

    public void EsecuzioneTerminata()
    {
        MioCervello.Animatore.SetBool("Pugno", false);
        MioCervello.Animatore.SetBool("PrendiArco", false);
        MioCervello.Animatore.SetBool("TiraFreccie", false);
        MioCervello.Animatore.SetBool("MettiVia", true);
    }
}
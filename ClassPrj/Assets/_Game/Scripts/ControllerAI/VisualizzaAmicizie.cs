using UnityEngine;

[ExecuteInEditMode]
public class VisualizzaAmicizie : MonoBehaviour
{
    public GameData item;
    public string[] nemici;
    public string[] amici;
    public string[] indifferenti;

    private int numeroNemici = 0;
    private int numeroAmici = 0;
    private int numeroIndifferenti = 0;

    // Use this for initialization
    private void Start()
    {
       if (item != null)
        {
            Setup(item);
        }
    }

    public void Setup(GameData item)
    {
        this.item = item;

        for (int i = 0; i < item.tipoEssere.Length; i++)
        {
            if (item.tipoEssere[i].Equals(gameObject.tag))
            {
                for(int j = 0; j < item.tipoEssere.Length; j++)
                {
                    if (item.matriceAmicizie[i].elementoAmicizia[j] == GameData.Amicizie.Nemico)
                        numeroNemici++;
                    else if (item.matriceAmicizie[i].elementoAmicizia[j] == GameData.Amicizie.Alleato)
                        numeroAmici++;
                    else if (item.matriceAmicizie[i].elementoAmicizia[j]== GameData.Amicizie.Neutro)
                        numeroIndifferenti++;
                }
              
            }
        }
        nemici = new string[numeroNemici];
        amici = new string[numeroAmici];
        indifferenti = new string[numeroIndifferenti];
     

        int a = 0;
        int b = 0;
        int c = 0;
        int indice = 0;
       
    
            for (int i = 0; i < item.tipoEssere.Length; i++)
            {

                if (item.tipoEssere[i].Equals(gameObject.tag))
                {
                    for (int j = 0; j < item.tipoEssere.Length; j++)
                    {
                        if (item.matriceAmicizie[i].elementoAmicizia[j] == GameData.Amicizie.Nemico)
                        {
                            nemici[a] = item.tipoEssere[j];
                            a++;
                        }
                            
                        else if (item.matriceAmicizie[i].elementoAmicizia[j] == GameData.Amicizie.Alleato)
                        {
                            amici[b] = item.tipoEssere[j];
                            b++;
                        }
                        else if (item.matriceAmicizie[i].elementoAmicizia[j] == GameData.Amicizie.Neutro)
                        {
                            indifferenti[c] = item.tipoEssere[j];
                            c++;
                        }
                    }

                }
            }
 
       
   }
}
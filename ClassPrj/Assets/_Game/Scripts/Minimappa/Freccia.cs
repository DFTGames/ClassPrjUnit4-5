using UnityEngine;
using UnityEngine.UI;

public class Freccia : MonoBehaviour
{
    private Image imageFreccia;
    private Minimappa minimappa;

    // Use this for initialization
    private void Start()
    {
        imageFreccia = GetComponent<Image>();
        minimappa = GetComponentInParent<Minimappa>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (minimappa.PlayerT == null)
            return;
        imageFreccia.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -minimappa.PlayerT.eulerAngles.y));
    }
}
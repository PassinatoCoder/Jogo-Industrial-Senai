using UnityEngine;
using TMPro;

public class ItemChecklistUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI texto;
    [SerializeField] private Color corPendente = Color.white;
    [SerializeField] private Color corConcluida = new Color(0.3f, 1f, 0.4f);

    private string textoBase;

    public void Configurar(string textoTarefa)
    {
        textoBase = textoTarefa;
        MarcarComoPendente();
    }

    public void MarcarComoPendente()
    {
        texto.text = $"[ ] {textoBase}";
        texto.color = corPendente;
    }

    public void MarcarComoConcluida()
    {
        texto.text = $"[X] {textoBase}";
        texto.color = corConcluida;
    }
}
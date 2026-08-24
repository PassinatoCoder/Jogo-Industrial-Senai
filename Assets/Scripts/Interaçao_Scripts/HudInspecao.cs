using UnityEngine;
using TMPro; // Usamos TextMeshPro porque a fonte normal da Unity é embaçada

public class HUDInspecao : MonoBehaviour
{
    // O Padrão Singleton: Permite que qualquer script do jogo ache a UI facilmente
    public static HUDInspecao Instancia { get; private set; }

    [Header("Elementos da Tela")]
    [SerializeField] private GameObject fundoDoPainel; // A caixinha preta translúcida
    [SerializeField] private TextMeshProUGUI textoNome;
    [SerializeField] private TextMeshProUGUI textoDescricao;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        Esconder(); // Começa o jogo desligado
    }

    public void Mostrar(string nome, string descricao)
    {
        textoNome.text = nome;
        textoDescricao.text = descricao;
        fundoDoPainel.SetActive(true);
    }

    public void Esconder()
    {
        fundoDoPainel.SetActive(false);
    }
}

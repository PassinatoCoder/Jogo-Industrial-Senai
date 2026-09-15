using UnityEngine;
using TMPro;

public class HUDInteracao : MonoBehaviour
{
    public static HUDInteracao Instancia { get; private set; }

    [Header("Painel de Inspeção (Aperte Y)")]
    [SerializeField] private GameObject painelInspecao;
    [SerializeField] private TextMeshProUGUI textoNome;
    [SerializeField] private TextMeshProUGUI textoDescricao;

    [Header("Prompt RDR2 (Canto da Tela)")]
    [SerializeField] private GameObject painelBotoes;
    [SerializeField] private TextMeshProUGUI textoBotoes;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        EsconderInspecao();
        EsconderBotoes();
    }

    // --- CONTROLE DA INSPEÇÃO ---
    public void MostrarInspecao(string nome, string descricao)
    {
        textoNome.text = nome;
        textoDescricao.text = descricao;
        painelInspecao.SetActive(true);
    }
    public void EsconderInspecao() => painelInspecao.SetActive(false);
    public bool InspecaoAberta() => painelInspecao.activeSelf;

    // --- CONTROLE DOS BOTÕES (E, F, Y) ---
    public void MostrarBotoes(string texto)
    {
        textoBotoes.text = texto;
        painelBotoes.SetActive(true);
    }
    public void EsconderBotoes() => painelBotoes.SetActive(false);
}
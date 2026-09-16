using System.Collections;
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

    [Header("Aviso de Ação Inválida (Smart Tagging)")]
    [SerializeField] private GameObject painelAvisoInvalido;
    [SerializeField] private TextMeshProUGUI textoAvisoInvalido;
    [SerializeField] private float duracaoAviso = 1.5f;

    private Coroutine rotinaAviso;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        EsconderInspecao();
        EsconderBotoes();
        if (painelAvisoInvalido != null) painelAvisoInvalido.SetActive(false);
    }

    public void MostrarInspecao(string nome, string descricao)
    {
        textoNome.text = nome;
        textoDescricao.text = descricao;
        painelInspecao.SetActive(true);
    }
    public void EsconderInspecao() => painelInspecao.SetActive(false);
    public bool InspecaoAberta() => painelInspecao.activeSelf;

    public void MostrarBotoes(string texto)
    {
        textoBotoes.text = texto;
        painelBotoes.SetActive(true);
    }
    public void EsconderBotoes() => painelBotoes.SetActive(false);

    public void MostrarAvisoInvalido(string mensagem)
    {
        if (rotinaAviso != null) StopCoroutine(rotinaAviso);
        rotinaAviso = StartCoroutine(RotinaAvisoInvalido(mensagem));
    }

    private IEnumerator RotinaAvisoInvalido(string mensagem)
    {
        textoAvisoInvalido.text = mensagem;
        painelAvisoInvalido.SetActive(true);
        yield return new WaitForSeconds(duracaoAviso);
        painelAvisoInvalido.SetActive(false);
        rotinaAviso = null;
    }
}
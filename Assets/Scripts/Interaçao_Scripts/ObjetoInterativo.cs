using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class ObjetoInterativo : MonoBehaviour, IInteragivel
{
    public enum TipoInteracao { ApenasEvento, ItemPegavel }

    [Header("Configurações Base")]
    [SerializeField] private TipoInteracao tipoDeInteracao = TipoInteracao.ApenasEvento;
    [SerializeField] private string nomeAcaoMundo = "Pegar"; // Ex: "Pegar Extintor"

    [Header("Dados do Item")]
    [SerializeField] private string nomeDoItem = "Objeto Desconhecido";
    [SerializeField, TextArea] private string descricaoInspecao = "Descrição...";
    [SerializeField] private bool podeSerUsado = false;

    [Header("Eventos")]
    [SerializeField] private UnityEvent aoInteragirMundo;
    [SerializeField] private UnityEvent aoUsar;

    private bool estaNaMao = false;
    private Vector3 posicaoOriginal;
    private Quaternion rotacaoOriginal;
    private Transform paiOriginal;
    private Collider2D colisor;

    private void Awake()
    {
        colisor = GetComponent<Collider2D>();
        colisor.isTrigger = true;
        posicaoOriginal = transform.position;
        rotacaoOriginal = transform.rotation;
        paiOriginal = transform.parent;
    }

    public void MostrarAviso(bool mostrar)
    {
        if (mostrar && !estaNaMao) HUDInteracao.Instancia.MostrarBotoes($"[E] {nomeAcaoMundo}");
        else if (!mostrar && !estaNaMao) HUDInteracao.Instancia.EsconderBotoes();
    }

    public void Interagir(GameObject instigador) // TECLA E
    {
        if (tipoDeInteracao == TipoInteracao.ApenasEvento)
        {
            aoInteragirMundo.Invoke();
            return;
        }

        if (!estaNaMao) PegarItem(instigador);
        else GuardarItem();
    }

    private void PegarItem(GameObject jogador)
    {
        estaNaMao = true;
        colisor.enabled = false;

        Transform maoDoJogador = jogador.transform.Find("PontoMao");
        if (maoDoJogador != null)
        {
            transform.SetParent(maoDoJogador);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        AtualizarBotoesNaMao();
        aoInteragirMundo.Invoke();
    }

    private void GuardarItem()
    {
        estaNaMao = false;
        colisor.enabled = true;

        transform.SetParent(paiOriginal);
        transform.position = posicaoOriginal;
        transform.rotation = rotacaoOriginal;

        HUDInteracao.Instancia.EsconderInspecao();
        HUDInteracao.Instancia.EsconderBotoes();
    }

    public void Inspecionar() // TECLA Y
    {
        if (!estaNaMao) return;

        if (HUDInteracao.Instancia.InspecaoAberta()) HUDInteracao.Instancia.EsconderInspecao();
        else HUDInteracao.Instancia.MostrarInspecao(nomeDoItem, descricaoInspecao);
    }

    public void Usar() // TECLA F
    {
        if (estaNaMao && podeSerUsado) aoUsar.Invoke();
    }

    private void AtualizarBotoesNaMao()
    {
        string botoes = "[E] Guardar   [Y] Inspecionar";
        if (podeSerUsado) botoes += "   [F] Usar";
        HUDInteracao.Instancia.MostrarBotoes(botoes);
    }
}
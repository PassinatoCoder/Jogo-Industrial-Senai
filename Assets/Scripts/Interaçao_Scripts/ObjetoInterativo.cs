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

    [Header("Zona de Guarda (Smart Tagging)")]
    [SerializeField] private TipoItem tipoItem = TipoItem.Generico;

    [Header("Inventário de Cinto")]
    [SerializeField] private Sprite iconeInventario;

    [Header("Eventos")]
    [SerializeField] private UnityEvent aoAcionar;              // TipoInteracao.ApenasEvento
    [SerializeField] private UnityEvent aoPegar;                 // Item saiu do mundo (primeira vez na mão)
    [SerializeField] private UnityEvent aoGuardarNaZonaCorreta;  // Smart Tagging: guardado no lugar certo
    [SerializeField] private UnityEvent aoUsar;                  // Tecla F

    private bool estaNaMao = false;
    private Vector3 posicaoOriginal;
    private Quaternion rotacaoOriginal;
    private Transform paiOriginal;
    private Collider2D colisor;

    public TipoItem TipoItem => tipoItem;
    public Sprite IconeInventario => iconeInventario;
    public bool EstaNaMao => estaNaMao;
    public string NomeDoItem => nomeDoItem;
    public bool EhItemPegavel => tipoDeInteracao == TipoInteracao.ItemPegavel;

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

    public void Interagir(GameObject instigador, Transform pontoMao) // TECLA E
    {
        if (tipoDeInteracao == TipoInteracao.ApenasEvento)
        {
            aoAcionar.Invoke();
            return;
        }

        if (!estaNaMao) PegarItem(pontoMao);
        else GuardarItem();
    }

    private void PegarItem(Transform maoDoJogador)
    {
        estaNaMao = true;
        colisor.enabled = false;

        if (maoDoJogador != null)
        {
            transform.SetParent(maoDoJogador);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        AtualizarBotoesNaMao();
        aoPegar.Invoke();
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

    // Chamado pelo PlayerMinigame quando o item é guardado numa ZonaDeGuarda compatível.
    public void GuardarNaZona(Transform pontoEncaixe)
    {
        estaNaMao = false;
        colisor.enabled = true;

        transform.SetParent(pontoEncaixe);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // A zona correta vira o novo "lar" do item — uma devolução comum futura já cai aqui.
        paiOriginal = pontoEncaixe;
        posicaoOriginal = transform.position;
        rotacaoOriginal = transform.rotation;

        HUDInteracao.Instancia.EsconderInspecao();
        HUDInteracao.Instancia.EsconderBotoes();
        aoGuardarNaZonaCorreta.Invoke();
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
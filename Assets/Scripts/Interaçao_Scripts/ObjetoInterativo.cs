using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class ObjetoInterativo : MonoBehaviour, IInteragivel
{
    public enum TipoInteracao { ApenasEvento, ItemPegavel }

    [Header("Configurações Base")]
    [SerializeField, Tooltip("ApenasEvento = Botões/Máquinas | ItemPegavel = Ferramentas/EPI")]
    private TipoInteracao tipoDeInteracao = TipoInteracao.ApenasEvento;
    [SerializeField, Tooltip("O balão de 'Aperte E'")]
    private GameObject iconeAviso;

    [Header("Dados do Item (Inspecionar)")]
    [SerializeField] private string nomeDoItem = "Objeto Desconhecido";
    [SerializeField, TextArea] private string descricaoInspecao = "Descrição do objeto ao ser inspecionado.";
    [SerializeField, Tooltip("Pode ser usado enquanto segura? (Ex: Alicate, Chave)")]
    public bool podeSerUsado = false;

    [Header("Eventos de Resposta (A Mágica)")]
    [SerializeField, Tooltip("O que acontece quando aperta E (ou inspeciona)")]
    private UnityEvent aoInteragirOuInspecionar;
    [SerializeField, Tooltip("O que acontece quando o jogador clica para Usar o item na mão")]
    private UnityEvent aoUsar;

    // --- Memória do Objeto ---
    private bool estaNaMao = false;
    private Vector3 posicaoOriginal;
    private Quaternion rotacaoOriginal;
    private Transform paiOriginal; // Caso ele estivesse dentro de uma gaveta ou prateleira
    private Collider2D colisor;

    private void Awake()
    {
        colisor = GetComponent<Collider2D>();
        colisor.isTrigger = true; // Garante que o jogador não tropece

        if (iconeAviso != null) iconeAviso.SetActive(false);

        // O objeto tira uma "foto" de onde ele nasceu no cenário
        posicaoOriginal = transform.position;
        rotacaoOriginal = transform.rotation;
        paiOriginal = transform.parent;
    }

    public void MostrarAviso(bool mostrar)
    {
        // Se o item já está na mão, não faz sentido mostrar balão flutuando
        if (iconeAviso != null && !estaNaMao)
        {
            iconeAviso.SetActive(mostrar);
        }
    }

    public void Interagir(GameObject instigador)
    {
        // CENA 1: É só um botão ou máquina (Interação Simples)
        if (tipoDeInteracao == TipoInteracao.ApenasEvento)
        {
            aoInteragirOuInspecionar.Invoke();
            return;
        }

        // CENA 2: É uma ferramenta, EPI ou Manual (Pega e Guarda)
        if (tipoDeInteracao == TipoInteracao.ItemPegavel)
        {
            if (!estaNaMao)
            {
                PegarEInspecionar(instigador);
            }
            else
            {
                ColocarDeVoltaNoLugar();
            }
        }
    }

    private void PegarEInspecionar(GameObject jogador)
    {
        estaNaMao = true;
        colisor.enabled = false; // Desliga a colisão para não empurrar o jogador
        MostrarAviso(false);

        // Encontra o ponto da mão do jogador (você precisará criar um GameObject vazio chamado 'PontoMao' dentro do Player)
        Transform maoDoJogador = jogador.transform.Find("PontoMao");

        if (maoDoJogador != null)
        {
            transform.SetParent(maoDoJogador);
            transform.localPosition = Vector3.zero; // Gruda perfeitamente na mão
            transform.localRotation = Quaternion.identity;
        }

        // --- SISTEMA DE INSPEÇÃO ---
        Debug.Log($"INSPECIONANDO: [{nomeDoItem}] - {descricaoInspecao}");
        // Aqui o evento pode ligar a sua UI do Canva na tela, mostrar os textos, etc.
        aoInteragirOuInspecionar.Invoke();
    }

    private void ColocarDeVoltaNoLugar()
    {
        estaNaMao = false;
        colisor.enabled = true; // Volta a ser detectável

        // Desgruda da mão e volta EXATAMENTE para o local e prateleira de origem
        transform.SetParent(paiOriginal);
        transform.position = posicaoOriginal;
        transform.rotation = rotacaoOriginal;

        Debug.Log($"{nomeDoItem} foi colocado de volta no devido lugar.");
    }

    // Método que o Player vai chamar quando apertar o botão de Usar (Ex: Botão Esquerdo do Mouse)
    public void UsarAcaoPrimaria()
    {
        if (estaNaMao && podeSerUsado)
        {
            Debug.Log($"Você usou o item: {nomeDoItem}");
            aoUsar.Invoke(); // Pode tocar um som, rodar uma animação de solda, etc.
        }
        else if (estaNaMao && !podeSerUsado)
        {
            Debug.Log($"O item {nomeDoItem} serve apenas para inspeção ou leitura.");
        }
    }
}
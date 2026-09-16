using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMinigame : MonoBehaviour
{
    [Header("Agachamento")]
    [SerializeField] private float multiplicadorVelocidadeAgachado = 0.5f;
    private bool estaAgachado = false;

    [Header("Movimentação Horizontal")]
    [SerializeField] private float velocidadeMaxima = 8f;
    [SerializeField] private float aceleracao = 10f;
    [SerializeField] private float desaceleracao = 10f;

    [Header("Física do Pulo (Game Feel)")]
    [SerializeField] private float forcaDoPulo = 15f;
    [SerializeField] private float multiplicadorQueda = 2.5f;
    [SerializeField] private float multiplicadorPuloCurto = 2f;
    [SerializeField] private float tempoCoyote = 0.15f;
    [SerializeField] private float tempoJumpBuffer = 0.15f;

    [Header("Detecção de Chão")]
    [SerializeField] private Transform pontoPe;
    [SerializeField] private float raioChao = 0.2f;
    [SerializeField] private LayerMask layerChao;

    [Header("Sistema de Interação")]
    [SerializeField] private Transform pontoInteracao;
    [SerializeField] private Transform pontoMao;
    [SerializeField] private float raioInteracao = 0.5f;
    [SerializeField] private LayerMask layerInterativo;

    [Header("Sistema de Zonas de Guarda (Smart Tagging)")]
    [SerializeField] private LayerMask layerZonas;

    [Header("Cinto e Bolsa")]
    [SerializeField] private CintoInventario cinto;
    [SerializeField] private BolsaInventario bolsa;

    private IInteragivel objetoFocadoAtual;
    private ZonaDeGuarda zonaFocada;

    private Rigidbody2D rb;
    private BoxCollider2D colisor;
    private Vector2 inputMovimento;
    private bool estaNoChao;
    private bool estavaNoChaoFrameAnterior;
    private float contadorCoyote;
    private float contadorJumpBuffer;
    private bool segurandoPulo;

    private Vector2 tamanhoOriginal;
    private Vector2 offsetOriginal;
    private Vector2 tamanhoAgachado;
    private Vector2 offsetAgachado;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colisor = GetComponent<BoxCollider2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        tamanhoOriginal = colisor.size;
        offsetOriginal = colisor.offset;
        tamanhoAgachado = new Vector2(tamanhoOriginal.x, tamanhoOriginal.y / 2f);
        offsetAgachado = new Vector2(offsetOriginal.x, offsetOriginal.y - (tamanhoOriginal.y / 4f));
    }

    private void Update()
    {
        VerificarChao();
        GerenciarTimers();
        AplicarGravidadePersonalizada();
        VerificarInteracaoAoRedor();
        VerificarZonaAoRedor();
    }

    private void FixedUpdate()
    {
        MoverPlayer();
    }

    public void AoMover(InputAction.CallbackContext context)
    {
        inputMovimento = context.ReadValue<Vector2>();
    }

    public void AoPular(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            contadorJumpBuffer = tempoJumpBuffer;
            segurandoPulo = true;
            TentarPular();
        }
        else if (context.canceled)
        {
            segurandoPulo = false;
        }
    }

    public void AoAgachar(InputAction.CallbackContext context)
    {
        if (context.started && estaNoChao)
        {
            estaAgachado = true;
            colisor.size = tamanhoAgachado;
            colisor.offset = offsetAgachado;
        }
        else if (context.canceled)
        {
            estaAgachado = false;
            colisor.size = tamanhoOriginal;
            colisor.offset = offsetOriginal;
        }
    }

    private void MoverPlayer()
    {
        float velocidadeAtual = estaAgachado ? (velocidadeMaxima * multiplicadorVelocidadeAgachado) : velocidadeMaxima;
        float velocidadeAlvo = inputMovimento.x * velocidadeAtual;

        float taxaVelocidade = (Mathf.Abs(velocidadeAlvo) > 0.01f) ? aceleracao : desaceleracao;
        float diferencaVelocidade = velocidadeAlvo - rb.linearVelocity.x;
        float forca = diferencaVelocidade * taxaVelocidade;

        rb.AddForce(forca * Vector2.right, ForceMode2D.Force);
    }

    private void TentarPular()
    {
        if (contadorCoyote > 0f && contadorJumpBuffer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * forcaDoPulo, ForceMode2D.Impulse);

            contadorCoyote = 0f;
            contadorJumpBuffer = 0f;
        }
    }

    private void AplicarGravidadePersonalizada()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplicadorQueda - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !segurandoPulo)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (multiplicadorPuloCurto - 1) * Time.deltaTime;
        }
    }

    private void VerificarChao()
    {
        estavaNoChaoFrameAnterior = estaNoChao;
        estaNoChao = Physics2D.OverlapCircle(pontoPe.position, raioChao, layerChao);

        if (estaNoChao && !estavaNoChaoFrameAnterior)
        {
            contadorCoyote = tempoCoyote;
            TentarPular();
        }
    }

    private void GerenciarTimers()
    {
        if (estaNoChao) contadorCoyote = tempoCoyote;
        else contadorCoyote -= Time.deltaTime;

        contadorJumpBuffer -= Time.deltaTime;
    }

    private void VerificarInteracaoAoRedor()
    {
        Collider2D colisorEncontrado = Physics2D.OverlapCircle(pontoInteracao.position, raioInteracao, layerInterativo);

        if (colisorEncontrado != null)
        {
            IInteragivel objetoEncontrado = colisorEncontrado.GetComponent<IInteragivel>();

            if (objetoEncontrado != null && objetoEncontrado != objetoFocadoAtual)
            {
                if (objetoFocadoAtual != null) objetoFocadoAtual.MostrarAviso(false);
                objetoFocadoAtual = objetoEncontrado;
                objetoFocadoAtual.MostrarAviso(true);
            }
        }
        else
        {
            if (objetoFocadoAtual != null)
            {
                objetoFocadoAtual.MostrarAviso(false);
                objetoFocadoAtual = null;
            }
        }
    }

    private void VerificarZonaAoRedor()
    {
        Collider2D colisorZona = Physics2D.OverlapCircle(pontoInteracao.position, raioInteracao, layerZonas);
        ZonaDeGuarda zonaEncontrada = colisorZona != null ? colisorZona.GetComponent<ZonaDeGuarda>() : null;

        if (zonaEncontrada != zonaFocada)
        {
            zonaFocada?.LimparFoco();
            zonaFocada = zonaEncontrada;
        }

        if (zonaFocada != null)
        {
            IInteragivel itemNaMao = pontoMao.GetComponentInChildren<IInteragivel>();
            ObjetoInterativo item = itemNaMao as ObjetoInterativo;
            TipoItem tipoNaMao = item != null ? item.TipoItem : TipoItem.Generico;
            zonaFocada.MostrarFoco(tipoNaMao, item != null);
        }
    }

    // TECLA E — pegar do mundo / devolver ao mundo ou zona. NÃO mexe em cinto/bolsa.
    public void AoInteragir(InputAction.CallbackContext context)
    {
        if (!context.started || (cinto != null && cinto.EmTransicao)) return;

        IInteragivel itemNaMao = pontoMao.GetComponentInChildren<IInteragivel>();

        if (itemNaMao != null)
        {
            ObjetoInterativo item = itemNaMao as ObjetoInterativo;

            if (zonaFocada != null && item != null)
            {
                if (zonaFocada.ItemGuardadoAqui)
                {
                    HUDInteracao.Instancia.MostrarAvisoInvalido("Essa zona já está ocupada!");
                }
                else if (zonaFocada.AceitaTipo(item.TipoItem))
                {
                    item.GuardarNaZona(zonaFocada.PontoDeEncaixe);
                    zonaFocada.MarcarComoOcupada(true);
                    cinto?.LimparSeForEsteItem(item);
                }
                else
                {
                    HUDInteracao.Instancia.MostrarAvisoInvalido($"Não dá pra guardar {item.NomeDoItem} aqui!");
                }
                return;
            }

            itemNaMao.Interagir(this.gameObject, pontoMao);

            if (item != null && !item.EstaNaMao)
            {
                cinto?.LimparSeForEsteItem(item);
            }
            return;
        }

        if (objetoFocadoAtual != null)
        {
            objetoFocadoAtual.Interagir(this.gameObject, pontoMao);
            // Pegar só deixa na mão — guardar no cinto/bolsa é ação separada (teclas G / B).
        }
    }

    public void AoUsar(InputAction.CallbackContext context)
    {
        if (!context.started || (cinto != null && cinto.EmTransicao)) return;

        IInteragivel itemNaMao = pontoMao.GetComponentInChildren<IInteragivel>();
        if (itemNaMao != null) itemNaMao.Usar();
    }

    public void AoInspecionar(InputAction.CallbackContext context)
    {
        if (!context.started || (cinto != null && cinto.EmTransicao)) return;

        IInteragivel itemNaMao = pontoMao.GetComponentInChildren<IInteragivel>();
        if (itemNaMao != null) itemNaMao.Inspecionar();
    }

    // TECLA G — guarda no cinto pela primeira vez, ou alterna sacar/guardar se já registrado.
    public void AoAlternarCinto(InputAction.CallbackContext context)
    {
        if (!context.started || cinto == null || cinto.EmTransicao) return;

        if (!cinto.TemItem)
        {
            IInteragivel itemNaMao = pontoMao.GetComponentInChildren<IInteragivel>();
            ObjetoInterativo item = itemNaMao as ObjetoInterativo;
            if (item != null) cinto.GuardarNoCinto(item, item.IconeInventario);
        }
        else
        {
            cinto.AlternarEquipar();
        }
    }

    // TECLA B — guarda o item da mão num slot livre da bolsa (precisa estar desbloqueada).
    public void AoGuardarNaBolsa(InputAction.CallbackContext context)
    {
        if (!context.started || bolsa == null || !bolsa.EstaDesbloqueada) return;

        IInteragivel itemNaMao = pontoMao.GetComponentInChildren<IInteragivel>();
        ObjetoInterativo item = itemNaMao as ObjetoInterativo;
        if (item == null) return;

        if (bolsa.Guardar(item, item.IconeInventario))
        {
            cinto?.LimparSeForEsteItem(item);
        }
    }

    // TECLA I — abre/fecha a grade da bolsa.
    public void AoAbrirBolsa(InputAction.CallbackContext context)
    {
        if (context.started) bolsa?.AlternarAbertura();
    }

    private void OnDrawGizmosSelected()
    {
        if (pontoPe != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pontoPe.position, raioChao);
        }
        if (pontoInteracao != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pontoInteracao.position, raioInteracao);
        }
    }
}
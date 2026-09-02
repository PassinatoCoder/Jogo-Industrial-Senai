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

    private IInteragivel objetoFocadoAtual;

    // Variáveis Internas
    private Rigidbody2D rb;
    private BoxCollider2D colisor;
    private Vector2 inputMovimento;
    private bool estaNoChao;
    private float contadorCoyote;
    private float contadorJumpBuffer;
    private bool segurandoPulo;

    // Variáveis matemáticas do Colisor
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
    }

    private void FixedUpdate()
    {
        MoverPlayer();
    }

    // =========================
    // INPUT (Novo Input System)
    // =========================
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

    // =========================
    // MOVIMENTO E FÍSICA
    // =========================
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

    // =========================
    // CHÃO E TIMERS
    // =========================
    private void VerificarChao()
    {
        estaNoChao = Physics2D.OverlapCircle(pontoPe.position, raioChao, layerChao);
    }

    private void GerenciarTimers()
    {
        if (estaNoChao) contadorCoyote = tempoCoyote;
        else contadorCoyote -= Time.deltaTime;

        contadorJumpBuffer -= Time.deltaTime;
    }

    // =========================
    // SISTEMA DE INTERAÇÃO RDR2
    // =========================
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

    // =========================
    // SISTEMA DE AÇÕES (E, F, Y)
    // =========================
    public void AoInteragir(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Primeiro checa se tem algo na mão (Se tiver, a prioridade é Guardar)
            IInteragivel itemNaMao = pontoMao.GetComponentInChildren<IInteragivel>();
            if (itemNaMao != null)
            {
                itemNaMao.Interagir(this.gameObject);
                return;
            }

            // Se a mão está vazia, tenta pegar o que está focando no chão
            if (objetoFocadoAtual != null)
            {
                objetoFocadoAtual.Interagir(this.gameObject);
            }
        }
    }

    public void AoUsar(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IInteragivel itemNaMao = pontoMao.GetComponentInChildren<IInteragivel>();
            if (itemNaMao != null) itemNaMao.Usar();
        }
    }

    public void AoInspecionar(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IInteragivel itemNaMao = pontoMao.GetComponentInChildren<IInteragivel>();
            if (itemNaMao != null) itemNaMao.Inspecionar();
        }
    }

    // =========================
    // DEBUG GIZMOS
    // =========================
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
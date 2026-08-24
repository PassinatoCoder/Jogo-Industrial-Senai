using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Para trocar de fase depois

public class MenuManager : MonoBehaviour
{
    [Header("Comportamento do Menu")]
    [SerializeField, Tooltip("Marque TRUE na cena do Menu Inicial. Deixe FALSE nas fases do jogo.")]
    private bool cenaDoMenuPrincipal = true;

    [Header("Estrutura do Menu")]
    [SerializeField] private CanvasGroup painelMestre;
    [SerializeField] private CanvasGroup painelTitulo;  // Jogar, Opções, Sair do Jogo
    [SerializeField] private CanvasGroup painelPause;   // Continuar, Opções, Voltar pro Menu Principal
    [SerializeField] private CanvasGroup painelOpcoes;  // O painel compartilhado

    [Header("Configurações de Transição")]
    [SerializeField, Range(0.1f, 1f)] private float tempoDeFade = 0.3f;

    private CanvasGroup painelAtual;
    private bool emTransicao = false;
    private bool jogoPausado = false;

    private void Start()
    {
        if (cenaDoMenuPrincipal)
        {
            // Se for a tela inicial do jogo
            painelAtual = painelTitulo;
            ConfigurarPainelVisivel(painelMestre);
            ConfigurarPainelVisivel(painelTitulo);
            ConfigurarPainelInvisivel(painelPause);
            ConfigurarPainelInvisivel(painelOpcoes);
        }
        else
        {
            // Se for uma fase da fábrica, o menu começa totalmente invisível e o jogo roda normal
            painelAtual = painelPause;
            ConfigurarPainelInvisivel(painelMestre);
            ConfigurarPainelInvisivel(painelTitulo);
            ConfigurarPainelInvisivel(painelPause);
            ConfigurarPainelInvisivel(painelOpcoes);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (emTransicao) return;

            if (cenaDoMenuPrincipal)
            {
                // No Menu Inicial, o ESC só volta das opções para o título
                if (painelAtual == painelOpcoes) StartCoroutine(TransitarPaineis(painelOpcoes, painelTitulo));
            }
            else
            {
                // Nas fases, o ESC controla o Pause
                if (jogoPausado)
                {
                    if (painelAtual == painelOpcoes) StartCoroutine(TransitarPaineis(painelOpcoes, painelPause));
                    else FecharPause();
                }
                else
                {
                    AbrirPause();
                }
            }
        }
    }

    // --- CONTROLE DO PAUSE NAS FASES ---
    private void AbrirPause()
    {
        jogoPausado = true;
        Time.timeScale = 0f;
        painelAtual = painelPause;
        ConfigurarPainelVisivel(painelPause);
        ConfigurarPainelInvisivel(painelOpcoes);
        StartCoroutine(FadeCanvasMestre(0f, 1f));
    }

    private void FecharPause()
    {
        jogoPausado = false;
        Time.timeScale = 1f;
        StartCoroutine(FadeCanvasMestre(1f, 0f));
    }

    // --- FUNÇÕES DOS BOTÕES ---
    public void Botao_Jogar()
    {
        if (emTransicao) return;
        Debug.Log("Carregando Fase 1...");
        // SceneManager.LoadScene("Fase_Tutorial");
    }

    public void Botao_Continuar()
    {
        if (emTransicao) return;
        FecharPause();
    }

    public void Botao_AbrirOpcoes()
    {
        if (emTransicao || painelAtual == painelOpcoes) return;
        StartCoroutine(TransitarPaineis(painelAtual, painelOpcoes));
    }

    public void Botao_Voltar()
    {
        if (emTransicao) return;
        CanvasGroup destino = cenaDoMenuPrincipal ? painelTitulo : painelPause;
        StartCoroutine(TransitarPaineis(painelAtual, destino));
    }

    public void Botao_VoltarMenuPrincipal()
    {
        if (emTransicao) return;
        Time.timeScale = 1f;
        Debug.Log("Voltando para a tela de título...");
        // SceneManager.LoadScene("Menu_Principal");
    }

    public void Botao_SairDoJogo()
    {
        if (emTransicao) return;
        Application.Quit();
    }

    private IEnumerator FadeCanvasMestre(float inicio, float fim)
    {
        emTransicao = true;
        painelMestre.blocksRaycasts = false;

        float tempo = 0;
        while (tempo < tempoDeFade)
        {
            tempo += Time.unscaledDeltaTime;
            painelMestre.alpha = Mathf.Lerp(inicio, fim, tempo / tempoDeFade);
            yield return null;
        }

        painelMestre.alpha = fim;

        // A MÁGICA ESTÁ AQUI: Liga e desliga a interação do mestre junto com o fade
        if (fim == 1f)
        {
            painelMestre.interactable = true;
            painelMestre.blocksRaycasts = true;
        }
        else
        {
            painelMestre.interactable = false;
        }

        emTransicao = false;
    }

    private IEnumerator TransitarPaineis(CanvasGroup saindo, CanvasGroup entrando)
    {
        emTransicao = true;
        saindo.interactable = false;
        saindo.blocksRaycasts = false;

        float tempo = 0;
        while (tempo < tempoDeFade)
        {
            tempo += Time.unscaledDeltaTime;
            saindo.alpha = Mathf.Lerp(1, 0, tempo / tempoDeFade);
            yield return null;
        }
        saindo.alpha = 0;

        tempo = 0;
        while (tempo < tempoDeFade)
        {
            tempo += Time.unscaledDeltaTime;
            entrando.alpha = Mathf.Lerp(0, 1, tempo / tempoDeFade);
            yield return null;
        }
        entrando.alpha = 1;
        entrando.interactable = true;
        entrando.blocksRaycasts = true;
        painelAtual = entrando;
        emTransicao = false;
    }

    private void ConfigurarPainelVisivel(CanvasGroup p) { p.alpha = 1; p.interactable = true; p.blocksRaycasts = true; }
    private void ConfigurarPainelInvisivel(CanvasGroup p) { p.alpha = 0; p.interactable = false; p.blocksRaycasts = false; }
}
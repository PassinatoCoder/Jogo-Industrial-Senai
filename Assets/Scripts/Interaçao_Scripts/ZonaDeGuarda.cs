using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZonaDeGuarda : MonoBehaviour
{
    [Header("Configuração da Zona")]
    [SerializeField] private TipoItem tipoAceito;
    [SerializeField] private Transform pontoDeEncaixe; // onde o item fica visualmente guardado
    [SerializeField] private string nomeExibicao = "Zona de Guarda";

    [Header("Feedback Visual (opcional — liga quando tiver arte)")]
    [SerializeField] private SpriteRenderer indicadorContorno;
    [SerializeField] private Color corNeutra = Color.white;
    [SerializeField] private Color corValida = Color.green;
    [SerializeField] private Color corInvalida = Color.red;

    public Transform PontoDeEncaixe => pontoDeEncaixe != null ? pontoDeEncaixe : transform;
    public bool ItemGuardadoAqui { get; private set; }
    public string NomeExibicao => nomeExibicao;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public bool AceitaTipo(TipoItem tipo) => tipo == tipoAceito;

    public void MostrarFoco(TipoItem tipoNaMao, bool temItemNaMao)
    {
        if (indicadorContorno == null) return;
        if (!temItemNaMao) { indicadorContorno.color = corNeutra; return; }
        indicadorContorno.color = AceitaTipo(tipoNaMao) ? corValida : corInvalida;
    }

    public void LimparFoco()
    {
        if (indicadorContorno != null) indicadorContorno.color = corNeutra;
    }

    public void MarcarComoOcupada(bool ocupada) => ItemGuardadoAqui = ocupada;
}
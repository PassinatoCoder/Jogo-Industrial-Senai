using UnityEngine;

public class BolsaInventario : MonoBehaviour
{
    public static BolsaInventario Instancia { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject painelBolsa;
    [SerializeField] private Transform containerSlots; // 9 SlotBolsaUI já montados com Grid Layout Group
    [SerializeField] private Transform pontoMao;

    [Header("Visual (ativa quando a mochila é equipada)")]
    [SerializeField] private GameObject visualBolsaNoCorpo; // sprite extra "cinto + bolsa" no player, opcional

    private SlotBolsaUI[] slots;

    public bool EstaDesbloqueada { get; private set; }
    public bool EstaAberta { get; private set; }

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else { Destroy(gameObject); return; }

        slots = containerSlots.GetComponentsInChildren<SlotBolsaUI>(true);
        painelBolsa.SetActive(false);
    }

    public void Desbloquear(int slotsQueLibera)
    {
        EstaDesbloqueada = true;
        if (visualBolsaNoCorpo != null) visualBolsaNoCorpo.SetActive(true);
    }

    public void AlternarAbertura()
    {
        if (!EstaDesbloqueada) return;
        EstaAberta = !EstaAberta;
        painelBolsa.SetActive(EstaAberta);
    }

    public bool Guardar(ObjetoInterativo item, Sprite icone)
    {
        if (!EstaDesbloqueada) return false;

        foreach (SlotBolsaUI slot in slots)
        {
            if (slot.Vazio)
            {
                slot.Ocupar(item, icone);
                item.gameObject.SetActive(false);
                item.transform.SetParent(transform); // guardado, sem posição física relevante
                return true;
            }
        }

        HUDInteracao.Instancia.MostrarAvisoInvalido("Bolsa cheia!");
        return false;
    }

    public void RetirarParaMao(SlotBolsaUI slot)
    {
        if (pontoMao.GetComponentInChildren<IInteragivel>() != null)
        {
            HUDInteracao.Instancia.MostrarAvisoInvalido("Suas mãos já estão ocupadas!");
            return;
        }

        ObjetoInterativo item = slot.Retirar();
        if (item == null) return;

        item.gameObject.SetActive(true);
        item.transform.SetParent(pontoMao);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }
}
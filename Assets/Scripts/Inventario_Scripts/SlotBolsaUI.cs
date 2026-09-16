using UnityEngine;
using UnityEngine.UI;

public class SlotBolsaUI : MonoBehaviour
{
    [SerializeField] private Image icone;
    [SerializeField] private Sprite spriteVazio;
    [SerializeField] private Button botao;

    private ObjetoInterativo itemGuardado;

    public bool Vazio => itemGuardado == null;

    private void Awake()
    {
        if (botao != null) botao.onClick.AddListener(() => BolsaInventario.Instancia.RetirarParaMao(this));
        AtualizarIcone();
    }

    public void Ocupar(ObjetoInterativo item, Sprite iconeItem)
    {
        itemGuardado = item;
        AtualizarIcone(iconeItem);
    }

    public ObjetoInterativo Retirar()
    {
        ObjetoInterativo item = itemGuardado;
        itemGuardado = null;
        AtualizarIcone();
        return item;
    }

    private void AtualizarIcone(Sprite spriteItem = null)
    {
        if (icone == null) return;
        icone.sprite = spriteItem != null ? spriteItem : spriteVazio;
    }
}
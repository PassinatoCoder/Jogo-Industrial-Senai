using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CintoInventario : MonoBehaviour
{
    [Header("UI do Slot")]
    [SerializeField] private Image iconeSlot;
    [SerializeField] private CanvasGroup grupoSlot;
    [SerializeField] private Sprite spriteSlotVazio;

    [Header("Ancoragem no Player")]
    [SerializeField] private Transform pontoMao;
    [SerializeField] private Transform pontoCinto;

    [Header("Timing (evita spam de guardar/sacar)")]
    [SerializeField] private float duracaoTransicao = 0.25f;

    private ObjetoInterativo itemAtual;
    private bool itemEstaNaMao;
    private bool emTransicao;

    public bool TemItem => itemAtual != null;
    public bool EmTransicao => emTransicao;

    // Ação deliberada: joga o item que está na mão pro cinto pela primeira vez.
    public void GuardarNoCinto(ObjetoInterativo item, Sprite icone)
    {
        if (TemItem || emTransicao || item == null) return;
        StartCoroutine(RotinaGuardarPrimeiraVez(item, icone));
    }

    private IEnumerator RotinaGuardarPrimeiraVez(ObjetoInterativo item, Sprite icone)
    {
        emTransicao = true;
        yield return new WaitForSeconds(duracaoTransicao);

        itemAtual = item;
        AtualizarIcone(icone);

        itemAtual.transform.SetParent(pontoCinto);
        itemAtual.transform.localPosition = Vector3.zero;
        itemAtual.transform.localRotation = Quaternion.identity;
        itemAtual.gameObject.SetActive(false);
        HUDInteracao.Instancia.EsconderBotoes();

        itemEstaNaMao = false;
        emTransicao = false;
    }

    // Já registrado: alterna sacar (mão) / guardar (cinto).
    public void AlternarEquipar()
    {
        if (!TemItem || emTransicao) return;
        StartCoroutine(RotinaAlternar());
    }

    private IEnumerator RotinaAlternar()
    {
        emTransicao = true;
        yield return new WaitForSeconds(duracaoTransicao);

        if (itemEstaNaMao)
        {
            itemAtual.transform.SetParent(pontoCinto);
            itemAtual.transform.localPosition = Vector3.zero;
            itemAtual.transform.localRotation = Quaternion.identity;
            itemAtual.gameObject.SetActive(false);
            HUDInteracao.Instancia.EsconderBotoes();
        }
        else
        {
            itemAtual.gameObject.SetActive(true);
            itemAtual.transform.SetParent(pontoMao);
            itemAtual.transform.localPosition = Vector3.zero;
            itemAtual.transform.localRotation = Quaternion.identity;
        }

        itemEstaNaMao = !itemEstaNaMao;
        emTransicao = false;
    }

    public void LimparSlot()
    {
        itemAtual = null;
        itemEstaNaMao = false;
        AtualizarIcone(null);
    }

    // Só limpa se o item que saiu for realmente o que o cinto está rastreando
    // (evita apagar por engano um item diferente do que está guardado).
    public void LimparSeForEsteItem(ObjetoInterativo item)
    {
        if (itemAtual == item) LimparSlot();
    }

    private void AtualizarIcone(Sprite icone)
    {
        if (iconeSlot == null) return;
        bool temItem = icone != null;
        iconeSlot.sprite = temItem ? icone : spriteSlotVazio;
        if (grupoSlot != null) grupoSlot.alpha = temItem ? 1f : 0.35f;
    }
}
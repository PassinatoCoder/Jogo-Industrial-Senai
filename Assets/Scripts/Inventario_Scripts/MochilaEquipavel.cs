using UnityEngine;

[RequireComponent(typeof(ObjetoInterativo))]
public class MochilaEquipavel : MonoBehaviour
{
    [SerializeField] private int slotsQueLibera = 9;
    [SerializeField] private BolsaInventario bolsa;
    [SerializeField] private CintoInventario cinto;

    private ObjetoInterativo objetoInterativo;

    private void Awake()
    {
        objetoInterativo = GetComponent<ObjetoInterativo>();
    }

    // Conectar isso no evento "Ao Usar" da própria mochila, no Inspector.
    public void Equipar()
    {
        if (bolsa != null) bolsa.Desbloquear(slotsQueLibera);
        if (cinto != null) cinto.LimparSeForEsteItem(objetoInterativo);

        Destroy(gameObject); // a mochila em si some — vira parte permanente do personagem
    }
}
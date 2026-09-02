using UnityEngine;

public interface IInteragivel
{
    void MostrarAviso(bool mostrar);
    void Interagir(GameObject instigador); // Tecla E
    void Usar(); // Tecla F
    void Inspecionar(); // Tecla Y
}
using UnityEngine;

public interface IInteragivel
{
    void MostrarAviso(bool mostrar);
    void Interagir(GameObject instigador, Transform pontoMao); // Tecla E
    void Usar(); // Tecla F
    void Inspecionar(); // Tecla Y
}
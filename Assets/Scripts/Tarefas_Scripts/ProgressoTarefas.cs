using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DefinicaoTarefa
{
    public string id;
    [TextArea] public string texto;
}

public class ProgressoTarefas : MonoBehaviour
{
    public static ProgressoTarefas Instancia { get; private set; }

    [Header("Configuração da Checklist da Fase")]
    [SerializeField] private List<DefinicaoTarefa> tarefasDaFase = new List<DefinicaoTarefa>();

    [Header("Referências de UI")]
    [SerializeField] private Transform containerLista; // objeto com Vertical Layout Group
    [SerializeField] private ItemChecklistUI prefabItemChecklist;

    [Header("Eventos")]
    [SerializeField] private UnityEvent aoCompletarTodasTarefas;

    private readonly Dictionary<string, ItemChecklistUI> linhasPorId = new Dictionary<string, ItemChecklistUI>();
    private readonly HashSet<string> tarefasConcluidas = new HashSet<string>();

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else { Destroy(gameObject); return; }

        MontarLista();
    }

    private void MontarLista()
    {
        for (int i = containerLista.childCount - 1; i >= 0; i--)
            Destroy(containerLista.GetChild(i).gameObject);

        linhasPorId.Clear();
        tarefasConcluidas.Clear();

        foreach (DefinicaoTarefa tarefa in tarefasDaFase)
        {
            if (string.IsNullOrEmpty(tarefa.id)) continue;

            ItemChecklistUI linha = Instantiate(prefabItemChecklist, containerLista);
            linha.Configurar(tarefa.texto);
            linhasPorId[tarefa.id] = linha;
        }
    }

    public void MarcarConcluida(string idTarefa)
    {
        if (string.IsNullOrEmpty(idTarefa)) return;
        if (tarefasConcluidas.Contains(idTarefa)) return;
        if (!linhasPorId.TryGetValue(idTarefa, out ItemChecklistUI linha)) return;

        tarefasConcluidas.Add(idTarefa);
        linha.MarcarComoConcluida();

        if (tarefasConcluidas.Count >= linhasPorId.Count)
            aoCompletarTodasTarefas?.Invoke();
    }

    public bool TarefaConcluida(string idTarefa) => tarefasConcluidas.Contains(idTarefa);
}
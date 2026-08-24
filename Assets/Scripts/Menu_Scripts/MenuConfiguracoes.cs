using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuConfiguracoes : MonoBehaviour
{
    [Header("Sistema de Áudio Sensorial")]
    [SerializeField, Tooltip("Arraste o seu Mixer_Industrial aqui.")]
    private AudioMixer mixerIndustrial;

    [SerializeField, Tooltip("Arraste o Slider de volume do Canvas aqui.")]
    private Slider sliderVolumeMaster;

    private void Start()
    {
        bool modoSalvo = PlayerPrefs.GetInt("TelaCheia", 1) == 1; Screen.fullScreen = modoSalvo;
        // 1. Busca no "Cofre" do jogo se o jogador já salvou um volume antes. Se for a primeira vez, o padrão é 1 (100%).
        float volumeSalvo = PlayerPrefs.GetFloat("VolumeMaster", 1f);

        // 2. Atualiza a posição visual da barrinha do Slider
        sliderVolumeMaster.value = volumeSalvo;

        // 3. Aplica o volume real no jogo
        AjustarVolumeMaster(volumeSalvo);

        // 4. Conecta o Slider ao script automaticamente (sem precisar ir no botão + do Inspector)
        sliderVolumeMaster.onValueChanged.AddListener(AjustarVolumeMaster);
    }

    public void AjustarVolumeMaster(float valor)
    {
        // A matemática Triple-A: O ouvido humano não entende volume de forma linear, mas sim logarítmica (Decibéis).
        // Essa fórmula converte a barrinha de 0 a 1 do Slider para -80dB a 0dB no Mixer.
        float volumeEmDecibeis = Mathf.Log10(valor) * 20f;

        // Envia o valor para aquele parâmetro que você "Expôs" no Mixer
        mixerIndustrial.SetFloat("MasterVolume", volumeEmDecibeis);

        // Salva a preferência do jogador para a próxima vez que ele abrir o jogo
        PlayerPrefs.SetFloat("VolumeMaster", valor);
        PlayerPrefs.Save();
    }
    public void AlternarTelaCheia(bool isTelaCheia)
    {
        Screen.fullScreen = isTelaCheia;
        // Salva a configuração!
        PlayerPrefs.SetInt("TelaCheia", isTelaCheia ? 1 : 0);
        PlayerPrefs.Save();
    }
}
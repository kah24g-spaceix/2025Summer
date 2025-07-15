using UnityEngine;
using UnityEngine.UI;

public class SoundUIController : MonoBehaviour
{
    [SerializeField] Button _musicButton, _sfxButton;
    [SerializeField] Slider _musicSlider, _sfxSlider;

    private void Start()
    {
        _musicButton.onClick.AddListener(ToggleMusic);
        _sfxButton.onClick.AddListener(ToggleSFX);

        _musicSlider.onValueChanged.AddListener(MusicVolume);
        _sfxSlider.onValueChanged.AddListener(SFXVolume);
    }
    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }
    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
    }
    public void MusicVolume(float volume)
    {
        AudioManager.Instance.MusicVolume(volume);
    }
    public void SFXVolume(float volume)
    {
        AudioManager.Instance.SFXVolume(volume);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public Slider SliderMusic;
    public Slider SliderVolume;
    public new AudioSource audio;
    public float music;
    void Start()
    {     
        Load();     
        ValueMusic();
        //SliderMusic.value = 0.5f;      
    }
    public void SetSliderMusic()
    {
        music= SliderMusic.value;
        Save();
        ValueMusic();
    }
    private void ValueMusic()
    {
       audio.volume= music;
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.buildIndex == 0)
        {
            SliderMusic.value = music;
        }
        
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("music", music);
    }
    private void Load()
    {
        music = PlayerPrefs.GetFloat("music", music);
    }
}

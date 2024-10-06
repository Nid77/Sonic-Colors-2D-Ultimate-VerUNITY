using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions; 

    public Slider musicSlider;
    public Slider soundSlider;
    public Slider voiceSlider;
    public void Start(){

        audioMixer.GetFloat("Music", out float musicValueForSlider);
        musicSlider.value=musicValueForSlider;
        audioMixer.GetFloat("Sound", out float soundValueForSlider);
        soundSlider.value=soundValueForSlider;
        audioMixer.GetFloat("Voice", out float voiceValueForSlider);
        voiceSlider.value=voiceValueForSlider;
        
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolution=0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width +" X "+ resolutions[i].height);     
            if(resolutions[i].width==Screen.width &&  resolutions[i].height==Screen.height){
                currentResolution=i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value=currentResolution;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetMusic(float volume){
        audioMixer.SetFloat("Music",volume);
    }
    public void SetSoundEffect(float volume){
        audioMixer.SetFloat("Sound",volume);
    }
    public void SetVoice(float volume){
        audioMixer.SetFloat("Voice",volume);
    }

    public void Resolution(){

    }

    public void SetFullScreen(bool isFullScreen){
        Screen.fullScreen= isFullScreen;
    }

    public void SetResolution(int resolutionIndex){
        Resolution resolu = resolutions[resolutionIndex];
        Screen.SetResolution(resolu.width,resolu.height,Screen.fullScreen);
    }
}

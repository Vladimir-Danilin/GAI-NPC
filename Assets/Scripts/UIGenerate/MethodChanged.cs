using System.Collections.Generic;
using UnityEngine;

public class MethodChanged : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Dropdown methodSelectorDropdown;
    [SerializeField]
    TMPro.TMP_Dropdown voiceSelecterDropdown;


    readonly List<string> _voice_for_silero = new List<string>() { "aidar", "baya", "kseniya", "xenia", "eugene", "random" };
    List<string> _voice_for_bark = new List<string>() {
            "ru_speaker_0",
            "ru_speaker_1",
            "ru_speaker_2",
            "ru_speaker_3",
            "ru_speaker_4",
            "ru_speaker_5",
            "ru_speaker_6",
            "ru_speaker_7",
            "ru_speaker_8",
            "ru_speaker_9"};

    void Start()
    {
        foreach (string option in _voice_for_bark)
            voiceSelecterDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(option));
    }


    public void ChangeVoiceList()
    {
        voiceSelecterDropdown.options.Clear();
        if (methodSelectorDropdown.captionText.text == methodSelectorDropdown.options[0].text)
            foreach (string option in _voice_for_bark)
                voiceSelecterDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(option));
        else if (methodSelectorDropdown.captionText.text == methodSelectorDropdown.options[1].text)
            foreach (string option in _voice_for_silero)
                voiceSelecterDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(option));
        voiceSelecterDropdown.captionText.text = voiceSelecterDropdown.options[0].text;
    }
}

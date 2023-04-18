using System;
using TMPro;
using Oculus.Voice;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceController : MonoBehaviour
{
    // voice
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    // UI
    [SerializeField] private TMP_Text fullTranscriptText;
    [SerializeField] private TextMeshProUGUI partialTranscriptText;

    private bool appVoiceActive;

    private void Awake()
    {   
        // when you start, you don't want anything from the text fields
        fullTranscriptText.text = partialTranscriptText.text = string.Empty;

        // bind transcriptions and active state
        appVoiceExperience.VoiceEvents.onFullTranscription.AddListener((transcription) =>
        {
            fullTranscriptText.text = transcription;
        });
        
        appVoiceExperience.VoiceEvents.onPartialTranscription.AddListener((transcription) =>
        {
            partialTranscriptText.text = transcription;
        });

        appVoiceExperience.VoiceEvents.OnRequestCreated.AddListener((request) =>
        {
            appVoiceActive = true;
            Debug.Log("OnRequestCreated Active");
        });

        appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(() =>
        {
            appVoiceActive = false;
            Debug.Log("OnRequestCompleted Active");
        });
    }
    // Update is called once per frame
    void Update()
    {
        //// press specific buttons to make the oculus listen
        //if (Keyboard.current.spaceKey.wasPressedThisFrame && !appVoiceActive)
        //{
        //    appVoiceExperience.Activate();
        //}
    }

    private static void DisplayValues(string prefix, string[] info)
    {
        foreach (var item in info)
        {
            Debug.Log($"{prefix} {item}");
        }
    }
}
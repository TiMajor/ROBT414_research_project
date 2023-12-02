using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using TMPro;

public class SpeechRecognition : MonoBehaviour
{
    // Start is called before the first frame update
    //recognised text by microsoft azure
    private string recognizedString = string.Empty;
    //recogniser object
    private SpeechRecognizer recognizer;
    private bool micPermissionGranted = true;
    //recognition language
    private string fromLanguage = "ru-RU";
    //mutex
    private object threadLocker = new object();

    //objects that are given by user in GUI
    public TextMeshProUGUI output;
    public string SpeechServiceAPIKey= string.Empty;
    public string SpeechServiceRegion = string.Empty;
    void Start()
    {

        BeginRecognizing();
    }
    public async void BeginRecognizing()
    {
        if (micPermissionGranted)
        {
            CreateSpeechRecognizer();

            if (recognizer != null)
            {
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                recognizedString = "Скажите что нибудь...";
            }
        }
        else
        {
            recognizedString = "This app cannot function without access to the microphone.";
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        if (recognizedString != string.Empty)
            {
                output.text=recognizedString;
            }
    }
    void CreateSpeechRecognizer()
    {
        if (recognizer == null)
        {
            SpeechConfig config = SpeechConfig.FromSubscription(SpeechServiceAPIKey, SpeechServiceRegion);
            config.SpeechRecognitionLanguage = fromLanguage;
            recognizer = new SpeechRecognizer(config);
            if (recognizer != null)
            {
                recognizer.Recognizing += RecognizingHandler;
                recognizer.Recognized += RecognizedHandler;
                recognizer.SpeechStartDetected += SpeechStartDetected;
                recognizer.SpeechEndDetected += SpeechEndDetectedHandler;
                recognizer.Canceled += CancelHandler;
                recognizer.SessionStarted += SessionStartedHandler;
                recognizer.SessionStopped += SessionStoppedHandler;
            }
        }
    }
    private void SessionStartedHandler(object sender, SessionEventArgs e)
    {
    }

    private void SessionStoppedHandler(object sender, SessionEventArgs e)
    {
        recognizer = null;
    }
    private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
    {
        if (e.Result.Reason == ResultReason.RecognizingSpeech)
        {
            lock (threadLocker)
            {
                recognizedString = $"{e.Result.Text}";
            }
        }
    }
    private void RecognizedHandler(object sender, SpeechRecognitionEventArgs e)
    {
        if (e.Result.Reason == ResultReason.RecognizedSpeech)
        {
            lock (threadLocker)
            {
                recognizedString = $"{e.Result.Text}";
            }
        }
        else if (e.Result.Reason == ResultReason.NoMatch)
        {
        }
    }
     private void SpeechStartDetected(object sender, RecognitionEventArgs e)
    {
    }

    private void SpeechEndDetectedHandler(object sender, RecognitionEventArgs e)
    {
    }

    private void CancelHandler(object sender, RecognitionEventArgs e)
    {
    }
}

using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class TalkieSpeech : MonoBehaviour
{
    public static TalkieSpeech Instance { get; private set; }

    [Header("Debuggers")]
    [SerializeField] private TextMeshProUGUI _textRecognizedDebugger, _speechRecognizerStatusDebugger;

    [Header("Configurations")]
    [SerializeField] private GameObject _micLogo;

    [SerializeField] private Action<string> _currentOnKeywordRecognized;
    [SerializeField] private string[] _currentKeywords;
    [SerializeField] private bool _autoCloseOnKeywordDetected;

    private DictationRecognizer m_DictationRecognizer;

    private bool countdownActivated;
    private float secondsSinceCountdown;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (m_DictationRecognizer != null)
        {
            _speechRecognizerStatusDebugger.text = m_DictationRecognizer.Status.ToString();

            if (_micLogo.activeInHierarchy && m_DictationRecognizer.Status == SpeechSystemStatus.Stopped)
            {
                Debug.Log("stopped");
                _micLogo.GetComponent<Image>().color = Color.red;
            }
            else
            {
                _micLogo.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void InitializeSpeechSystem()
    {
        if (m_DictationRecognizer != null && m_DictationRecognizer.Status == SpeechSystemStatus.Running)
            return;

        m_DictationRecognizer = new DictationRecognizer(DictationTopicConstraint.Dictation)
        {
            InitialSilenceTimeoutSeconds = 999,
            AutoSilenceTimeoutSeconds = 999
        };

        m_DictationRecognizer.DictationResult += DictationResult;
        m_DictationRecognizer.DictationComplete += DictationComplete;
        m_DictationRecognizer.DictationError += DictationError;

        m_DictationRecognizer.Start();
        secondsSinceCountdown = 0;

        _micLogo.SetActive(true);
        Debug.Log("Speech behavior Initiated");
    }

    public void UpdateSpeechBehavior(string[] keywords, Action<string> onRecognized, bool autoCloseOnKeywordDetected)
    {
        InitializeSpeechSystem();
        _currentOnKeywordRecognized = onRecognized;
        _currentKeywords = keywords;
        _autoCloseOnKeywordDetected = autoCloseOnKeywordDetected;
        Debug.Log("Speech behavior updated");
    }

    public void CloseSpeechSystem()
    {
        _micLogo.SetActive(false);
        _currentOnKeywordRecognized = null;
        _currentKeywords = null;
        m_DictationRecognizer.Stop();
    }

    private void DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.LogFormat("Dictation result: {0}", text);

        if (_textRecognizedDebugger != null)
            _textRecognizedDebugger.text = "Text recognized : " + text;

        // check if detected word is on keywords
        for (int i = 0; i <_currentKeywords.Length; i++)
        {
            if (text.ToUpper() == _currentKeywords[i].ToUpper())
            {
                _currentOnKeywordRecognized.Invoke(text);

                if (_autoCloseOnKeywordDetected)
                    CloseSpeechSystem();

                return;
            }
        }
    }

    private void DictationComplete(DictationCompletionCause completionCause)
    {
        if (completionCause != DictationCompletionCause.Complete)
        {
            Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);

            // restart speech system.
            InitializeSpeechSystem();
        }
    }

    private void DictationError(string error, int hresult)
    {
        Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangerThroughSpeech : MonoBehaviour
{
    [SerializeField] private Image _image;

    private void Start()
    {
        //SetupRecognizer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetupRecognizer();
    }

    private void SetupRecognizer()
    {
        string[] keywords = { "red", "blue" };

        TalkieSpeech.Instance.UpdateSpeechBehavior(keywords, OnKeywordRecognized, true);
        _image.color = Color.white;
    }

    public void OnKeywordRecognized(string keyword)
    {
        _image.color = keyword == "red" ? Color.red : Color.blue;
    }
}

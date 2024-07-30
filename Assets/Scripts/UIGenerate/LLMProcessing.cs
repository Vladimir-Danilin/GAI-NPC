using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LLMProcessing : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button sendButton;
    [SerializeField]
    private GameObject userMessagePrefab;
    [SerializeField]
    private GameObject botMessagePrefab;
    [SerializeField]
    private TMP_InputField systemContent;

    private List<GameObject> messages = new List<GameObject>();
    private string response;

    void Start()
    {
        sendButton.onClick.AddListener(SendMessage);
    }

    private void SendMessage()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            AddMessage(inputField.text);
            AddAnswer(inputField.text);
            inputField.text = "";
        }
    }

    private void AddMessage(string messageText)
    {
        GameObject newMessage = Instantiate(userMessagePrefab, content.transform);
        newMessage.GetComponentInChildren<TMP_Text>().text = messageText;
        messages.Add(newMessage);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }
    public void AddMessage1(string messageText)
    {
        GameObject newMessage = Instantiate(userMessagePrefab, content.transform);
        newMessage.GetComponentInChildren<TMP_Text>().text = messageText;
        messages.Add(newMessage);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    private async void AddAnswer(string messageText)
    {
        GameObject newMessage = Instantiate(botMessagePrefab, content.transform);

        var ts = PythonEngine.BeginAllowThreads();
        Action action = () =>
        {
            using dynamic tg = Py.Import("api_together");
            using dynamic client = tg.call_client();
            response = tg.ask_question(messageText, client, "ќтвечай на русском\nотвечай не больше 2-3 предложений\n" + systemContent.text);
        };
        await RunInThreadWithLock(action);
        PythonEngine.EndAllowThreads(ts);

        newMessage.GetComponentInChildren<TMP_Text>().text = response;
        messages.Add(newMessage);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();

    }

    public void SaveMessagesToCSV()
    {
        string savePath = EditorUtility.SaveFolderPanel("”кажите папку сохранени€ диалога", "", @"D:\code\unity\GAI-NPC\Assets\Dialogues");

        using (StreamWriter writer = new StreamWriter(savePath + @"\dialog_1.csv"))
        {
            for (int i = 0; i < messages.Count; i++)
            {
                GameObject message = messages[i];
                TMP_Text messageTextComponent = i % 2 == 0 ? message.GetComponentInChildren<TMP_Text>() : null;
                if (messageTextComponent != null && messages[i] != messages[messages.Count - 2])
                {
                    writer.WriteLine($"{messageTextComponent.text.Replace('\n', ' ')};");
                }
                else if (messageTextComponent != null && messages[i] == messages[messages.Count - 2])
                {
                    writer.WriteLine($"{messageTextComponent.text.Replace('\n', ' ')}");
                }
            }
        }
        using (StreamWriter writer = new StreamWriter(savePath + @"\dialog_2.csv"))
        {
            for (int i = 0; i < messages.Count; i++)
            {
                GameObject message =  messages[i];
                TMP_Text messageTextComponent = i % 2 != 0 ? message.GetComponentInChildren<TMP_Text>() : null;
                if (messageTextComponent != null && messages[i] != messages.Last())
                {
                    writer.WriteLine($"{messageTextComponent.text.Replace('\n', ' ')};");
                }
                else if (messageTextComponent != null && messages[i] == messages.Last())
                {
                    writer.WriteLine($"{messageTextComponent.text.Replace('\n', ' ')}");
                }
            }
        }
    }

    private async Task RunInThreadWithLock(Action action, TimeSpan? waitTimeout = null, CancellationToken cancelToken = default)
    {
        await Task.Run(() =>
        {
            using (Py.GIL())
            {
                action();
            }
        }, cancelToken);
    }
}

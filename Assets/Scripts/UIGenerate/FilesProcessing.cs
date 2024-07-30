using Python.Runtime;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class FilesProcessing : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text pathText;
    [SerializeField]
    TMPro.TMP_Text processing;
    [SerializeField]
    TMPro.TMP_Dropdown generateMethod;
    [SerializeField]
    TMPro.TMP_Dropdown speaker;

    private string _fields;

    public void LoadFile()
    {
        string file = EditorUtility.OpenFilePanel("Выберите csv файл", "", "csv");
        if (file == null)
        {
            pathText.text = "";
            return;
        }
        pathText.text = file;

        StreamReader streamReader = new StreamReader(file);
        _fields = streamReader.ReadToEnd();
    }

    public void TextProccesing()
    {
        processing.text = "Обработка...";
        string savePath = EditorUtility.SaveFolderPanel("Укажите папку сохранения речи", "", @"D:\code\unity\GAI-NPC\Assets\Speech\");

        if (savePath == null)
        {
            processing.text = "";
            return;
        }
        GenerateSpeech(savePath);
    }

    private async void GenerateSpeech(string savePath)
    {
        while (processing.text != "Завершено")
        {
            string[] files = Directory.GetFiles(savePath, "*.wav");
            int counter = 1;

            if (files.Length > 0)
            {
                counter = files.Length + 1;
            }

            foreach (string field in _fields.Split(';'))
            {
                var ts = PythonEngine.BeginAllowThreads();
                if (generateMethod.captionText.text == generateMethod.options[0].text)
                {
                    Action myAction = () =>
                    {
                        using dynamic bark = Py.Import("api_bark");
                        bark.generate_wav(field.ToPython(), savePath + @"\" + counter + ".wav", "v2/" + speaker.captionText.text);
                    };

                    await RunInThreadWithLock(myAction);
                }
                else
                {
                    Action myAction = () =>
                    {
                        using dynamic silero = Py.Import("api_silero");
                        silero.generate_wav(field.ToPython(), savePath + @"\" + counter + ".wav", speaker.captionText.text);
                    };

                    await RunInThreadWithLock(myAction);
                }
                counter++;
                PythonEngine.EndAllowThreads(ts);
            }
            processing.text = "Завершено";
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
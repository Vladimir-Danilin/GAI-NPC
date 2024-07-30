using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{

    public string[] lines;
    public AudioClip[] speeches;
    public float speedText = 0.05f;
    public Text dialogText;
    public float readSpeed = 1250f;
    public RawImage dialogImage;
    public NavMeshAgent navMeshAgent;

    public int index = 0;

    public void StartDialog()
    {
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        AudioSource replica = navMeshAgent.GetComponent<AudioSource>();
        replica.spatialBlend = 1;
        replica.PlayOneShot(speeches[index]);
        foreach (char c in lines[index].ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        //yield return new WaitForSeconds(lines[index].Length / readSpeed * 60);
        yield return new WaitForSeconds(speeches[index].length);
        if (dialogText.text == lines[index])
        {
            dialogText.text = "";
            NextLines();
        }
        else
        {
            StopCoroutine(TypeLine());
            dialogText.text = lines[index];
        }
    }

    private void NextLines()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartDialog();
        }
        else
        {
            StopCoroutine(TypeLine());
            dialogText.text = "";
            dialogImage.gameObject.SetActive(false);
        }
    }
}
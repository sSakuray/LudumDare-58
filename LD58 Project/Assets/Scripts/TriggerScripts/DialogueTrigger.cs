using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private Collider triggerCollider;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Dialogue Content")]
    [SerializeField] private string speakerName;
    [SerializeField] private string[] dialogueLines;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTarget;

    [Header("Scripts to Disable")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private Camera playerCamera;
    private int currentLine;
    private bool isInDialogue;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        playerCamera = Camera.main;
    }

    private void Update()
    {
        if (isInDialogue && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)))
        {
            ShowNextLine();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        isInDialogue = true;
        currentLine = 0;

        // Отключаем указанные скрипты
        foreach (var script in scriptsToDisable)
        {
            script.enabled = false;
        }

        // Настраиваем и показываем UI
        speakerNameText.text = speakerName;
        dialogueText.text = dialogueLines[currentLine];
        dialoguePanel.SetActive(true);

        // Поворачиваем камеру
        if (cameraTarget != null)
        {
            playerCamera.transform.rotation = cameraTarget.rotation;
        }
    }

    private void ShowNextLine()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isInDialogue = false;
        dialoguePanel.SetActive(false);

        // Включаем скрипты обратно
        foreach (var script in scriptsToDisable)
        {
            script.enabled = true;
        }
    }

    // Визуализация хитбокса в редакторе
    private void OnDrawGizmosSelected()
    {
        if (triggerCollider != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(triggerCollider.bounds.center, triggerCollider.bounds.size);
        }
    }
}
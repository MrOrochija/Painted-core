using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SetCheckpoint : MonoBehaviour
{
    private Canvas UI;

    public GameObject player;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private GameObject checkpoint;
    public Button yesButton;
    public Button noButton;

    private GameObject currentCheckpoint;
    private bool active = true;

    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerHealth = player.GetComponent<PlayerHealth>();
        currentCheckpoint = playerHealth.currentCheckpoint;

        if (playerMovement != null)
        {
            playerMovement.OnDrawAnimationFinished += HandleDrawAnimationFinished;
        }

        Transform checkpointTransform = transform.Find("Checkpoint");
        if (checkpointTransform != null)
        {
            checkpoint = checkpointTransform.gameObject;
        }

        Transform canvasTransform = transform.Find("UI");
        if (canvasTransform != null)
        {
            UI = canvasTransform.GetComponent<Canvas>();
        }

        if (yesButton != null) yesButton.onClick.AddListener(YesButtonClick);
        if (noButton != null) noButton.onClick.AddListener(NoButtonClick);
    }

    void OnDestroy()
    {
        if (playerMovement != null)
        {
            playerMovement.OnDrawAnimationFinished -= HandleDrawAnimationFinished;
        }
    }

    void HandleDrawAnimationFinished()
    {
        if (checkpoint != null && player != null)
        {
            Vector3 spawnPosition = player.transform.position - new Vector3(-0.74f, 0.05f, 0f);
            GameObject spawnedCheckpoint = Instantiate(checkpoint, spawnPosition, checkpoint.transform.rotation);

            Destroy(currentCheckpoint);
            spawnedCheckpoint.SetActive(true);
            currentCheckpoint = spawnedCheckpoint;
            playerHealth.currentCheckpoint = spawnedCheckpoint;
        }
    }

    void YesButtonClick()
    {
        if (playerHealth.UseMana(50))
        {
            UISet(false);
            StartCoroutine(playerMovement.DrawAnimation());
        }
    }

    void NoButtonClick()
    {
        UISet(false);
    }

    void Update()
    {
        if (UI == null || !active) return;

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            UI.enabled = !UI.enabled;
            
            if (UI.enabled)
            {
                playerMovement.currentState = PlayerState.Frozen;
            }
            else
            {
                if (playerMovement.currentState != PlayerState.Combat)
                    playerMovement.currentState = PlayerState.Free;
            }
        }
    }

    private void UISet(bool value)
    {
        UI.enabled = value;
        
        if (value)
        {
            playerMovement.currentState = PlayerState.Frozen;
        }
        else
        {
            if (playerMovement.currentState != PlayerState.Combat)
                playerMovement.currentState = PlayerState.Free;
        }
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
        UI.enabled = false;
        
        if (playerMovement.currentState != PlayerState.Combat)
            playerMovement.currentState = PlayerState.Free;
    }
}
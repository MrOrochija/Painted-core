using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; 

public class DialogueModule : SoundsModule
{
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject otherCanvas;
    [SerializeField] private GameObject backgroundCanvas;
    [SerializeField] private GameObject dialogueCanvas;
    
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private TMP_Text whoComponent;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image otherImage;

    private bool isWaitingForInput = false;

    private Coroutine bgFadeCoroutine;
    private Coroutine playerScaleCoroutine;
    private Coroutine otherScaleCoroutine;

    private readonly Vector3 scaleNormal = new Vector3(1.5f, 1.5f, 1f);
    private readonly Vector3 scaleSpeaking = new Vector3(2f, 2f, 1f);
    
    private readonly Color colorNormal = new Color32(59, 59, 59, 255);
    private readonly Color colorSpeaking = Color.white;

    public event System.Action OnDialogueFinished;

    void Awake()
    {
        InitReferences();
        InitializeVisuals();
    }

    private void InitReferences()
    {
        if (playerCanvas == null) playerCanvas = transform.Find("PlayerCanvas")?.gameObject;
        if (otherCanvas == null) otherCanvas = transform.Find("OtherCanvas")?.gameObject;
        if (backgroundCanvas == null) backgroundCanvas = transform.Find("Background")?.gameObject;
        if (dialogueCanvas == null) dialogueCanvas = transform.Find("DialogueCanvas")?.gameObject;

        if (playerImage == null && playerCanvas != null) 
            playerImage = playerCanvas.GetComponentInChildren<Image>(true);
            
        if (otherImage == null && otherCanvas != null) 
            otherImage = otherCanvas.GetComponentInChildren<Image>(true);
            
        if (backgroundImage == null && backgroundCanvas != null) 
            backgroundImage = backgroundCanvas.GetComponentInChildren<Image>(true);

        if (dialogueCanvas != null)
        {
            if (textComponent == null) textComponent = dialogueCanvas.transform.Find("Text")?.GetComponent<TMP_Text>();
            if (whoComponent == null) whoComponent = dialogueCanvas.transform.Find("Who")?.GetComponent<TMP_Text>();
        }
    }

    private void InitializeVisuals()
    {
        if (playerImage != null) { playerImage.transform.localScale = scaleNormal; playerImage.color = colorNormal; }
        if (otherImage != null) { otherImage.transform.localScale = scaleNormal; otherImage.color = colorNormal; }
        
        if (backgroundImage != null)
        {
            Color c = backgroundImage.color;
            c.a = 0f;
            backgroundImage.color = c;
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        if (playerMovement != null)
        {
            playerMovement.currentState = PlayerState.Frozen;
        }

        SetCanvasState(true);
        InitializeVisuals();

        SetupInitialSprites(dialogue);

        if (bgFadeCoroutine != null) StopCoroutine(bgFadeCoroutine);
        bgFadeCoroutine = StartCoroutine(FadeBackground(200f / 255f));

        StartCoroutine(ShowDialogueRoutine(dialogue));
    }

    private void SetupInitialSprites(DialogueData dialogue)
    {
        bool playerSpriteSet = false;
        bool otherSpriteSet = false;

        foreach (DialogueLine line in dialogue.lines)
        {
            bool isPlayer = line.speakerName == "Player";

            if (isPlayer && !playerSpriteSet && playerImage != null && line.speakerSprite != null)
            {
                playerImage.sprite = line.speakerSprite;
                playerSpriteSet = true;
            }
            else if (!isPlayer && !otherSpriteSet && otherImage != null && line.speakerSprite != null)
            {
                otherImage.sprite = line.speakerSprite;
                otherSpriteSet = true;
            }

            if (playerSpriteSet && otherSpriteSet) break;
        }
    }

    private IEnumerator ShowDialogueRoutine(DialogueData dialogue)
    {
        foreach (DialogueLine line in dialogue.lines)
        {
            if (line.isNarrator)
            {
                if (playerCanvas != null) playerCanvas.SetActive(false);
                if (otherCanvas != null) otherCanvas.SetActive(false);
                
                if (whoComponent != null) whoComponent.text = string.IsNullOrEmpty(line.speakerName) ? "" : line.speakerName;
            }
            else
            {
                if (playerCanvas != null) playerCanvas.SetActive(true);
                if (otherCanvas != null) otherCanvas.SetActive(true);

                if (whoComponent != null) whoComponent.text = line.speakerName;

                bool isPlayer = line.speakerName == "Player"; 
                
                Image activeImage = isPlayer ? playerImage : otherImage;
                Image inactiveImage = isPlayer ? otherImage : playerImage;

                if (line.speakerSprite != null && activeImage != null)
                {
                    activeImage.sprite = line.speakerSprite;
                }

                AnimateCharacter(activeImage, scaleSpeaking, colorSpeaking, ref (isPlayer ? ref playerScaleCoroutine : ref otherScaleCoroutine));
                AnimateCharacter(inactiveImage, scaleNormal, colorNormal, ref (isPlayer ? ref otherScaleCoroutine : ref playerScaleCoroutine));
            }

            yield return StartCoroutine(DisplayLineWithPagination(line.text));
        }

        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);

        if (playerCanvas != null && playerCanvas.activeSelf)
            AnimateCharacter(playerImage, scaleNormal, colorNormal, ref playerScaleCoroutine);
            
        if (otherCanvas != null && otherCanvas.activeSelf)
            AnimateCharacter(otherImage, scaleNormal, colorNormal, ref otherScaleCoroutine);

        if (bgFadeCoroutine != null) StopCoroutine(bgFadeCoroutine);
        yield return StartCoroutine(FadeBackground(0f));

        SetCanvasState(false);

        if (playerMovement != null)
        {
            playerMovement.currentState = PlayerState.Free;
        }

        OnDialogueFinished?.Invoke();
    }

    private void SetCanvasState(bool state)
    {
        if (playerCanvas != null) playerCanvas.SetActive(state);
        if (otherCanvas != null) otherCanvas.SetActive(state);
        if (backgroundCanvas != null) backgroundCanvas.SetActive(state);
        if (dialogueCanvas != null) dialogueCanvas.SetActive(state);
    }

    private void AnimateCharacter(Image img, Vector3 targetScale, Color targetColor, ref Coroutine activeCoroutine)
    {
        if (img == null || !img.gameObject.activeInHierarchy) return;
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(AnimateImageRoutine(img, targetScale, targetColor));
    }

    private IEnumerator AnimateImageRoutine(Image img, Vector3 targetScale, Color targetColor)
    {
        Transform targetTransform = img.transform;
        Vector3 startScale = targetTransform.localScale;
        Color startColor = img.color;
        float timer = 0f;

        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.3f;
            
            targetTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            img.color = Color.Lerp(startColor, targetColor, t);
            
            yield return null;
        }

        targetTransform.localScale = targetScale;
        img.color = targetColor;
    }

    private IEnumerator FadeBackground(float targetAlpha)
    {
        if (backgroundImage == null) yield break;

        Color startColor = backgroundImage.color;
        float startAlpha = startColor.a;
        float timer = 0f;

        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            startColor.a = Mathf.Lerp(startAlpha, targetAlpha, timer / 0.3f);
            backgroundImage.color = startColor;
            yield return null;
        }

        startColor.a = targetAlpha;
        backgroundImage.color = startColor;
    }

    private IEnumerator DisplayLineWithPagination(string lineText)
    {
        string remainingText = lineText;

        while (!string.IsNullOrEmpty(remainingText))
        {
            textComponent.overflowMode = TextOverflowModes.Page;
            textComponent.pageToDisplay = 1;
            textComponent.text = remainingText;
            textComponent.ForceMeshUpdate();

            string chunkToPrint;

            if (textComponent.textInfo.pageCount > 1)
            {
                int lastVisibleCharIndex = textComponent.textInfo.pageInfo[0].lastCharacterIndex;
                int cutIndex = -1;
                
                for (int i = lastVisibleCharIndex; i >= 0; i--)
                {
                    char c = remainingText[i];
                    if (c == '.' || c == ',' || c == '!' || c == '?' || c == ';' || c == '…')
                    {
                        cutIndex = i;
                        break;
                    }
                }

                int splitLength = (cutIndex != -1) ? cutIndex + 1 : lastVisibleCharIndex + 1;

                chunkToPrint = remainingText.Substring(0, splitLength);
                remainingText = remainingText.Substring(splitLength).TrimStart();
            }
            else
            {
                chunkToPrint = remainingText;
                remainingText = string.Empty;
            }

            yield return StartCoroutine(TypeTextRoutine(chunkToPrint));
            yield return null;

            isWaitingForInput = true;
            while (isWaitingForInput)
            {
                if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    isWaitingForInput = false;
                }
                yield return null;
            }
        }
    }

    private IEnumerator TypeTextRoutine(string targetText)
    {
        if (textComponent == null) yield break;

        textComponent.text = targetText;
        textComponent.maxVisibleCharacters = 0;
        textComponent.ForceMeshUpdate();

        int totalVisibleCharacters = textComponent.textInfo.characterCount;

        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            textComponent.maxVisibleCharacters = i;

            if (i > 0 && textComponent.textInfo.characterInfo[i - 1].character != ' ')
            {
                PlaySound(sounds[0]);
            }

            float timer = 0f;
            while (timer < 0.03f)
            {
                if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    textComponent.maxVisibleCharacters = totalVisibleCharacters;
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }

        textComponent.maxVisibleCharacters = totalVisibleCharacters;
    }
}
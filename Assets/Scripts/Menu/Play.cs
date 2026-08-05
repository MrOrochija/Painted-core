using UnityEngine;

public class Play : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject menu;

    void Start()
    {
        playerMovement.currentState = PlayerState.Frozen;
    }

    public void OnButtonClick()
    {
        if (playerMovement != null)
        {
            playerMovement.currentState = PlayerState.Free;
        }

        menu.SetActive(false);
    }
}

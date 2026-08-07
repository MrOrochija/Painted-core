using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class AnyKey : MonoBehaviour
{
    [HideInInspector] public char lastChar;
    public Action<char> OnKeyPressed;

    void Update()
    {
        if (Keyboard.current == null) return;

        for (Key key = Key.A; key <= Key.Z; key++)
        {
            KeyControl keyControl = Keyboard.current[key];

            if (keyControl != null && keyControl.wasPressedThisFrame)
            {
                char c = key.ToString().ToLower()[0];
                bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
                
                lastChar = isShiftPressed ? char.ToUpper(c) : c;

                OnKeyPressed?.Invoke(lastChar);
                
                gameObject.SetActive(false);
                break;
            }
        }
    }
}
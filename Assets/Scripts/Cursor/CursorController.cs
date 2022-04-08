using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private bool cursorState = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            cursorState = !cursorState;
            if (cursorState)
            {
                CursorLocked();
            }
            else
            {
                CursorNone();
            }
        }
    }

    private void CursorLocked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void CursorNone()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

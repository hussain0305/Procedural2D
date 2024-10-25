using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AbyssPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PopupManager.Instance.ShowActionPrompt(transform.position + Global.GetPromptOffset(), "E", "The Abyss");
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        PopupManager.Instance.HideCurrentPopup();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Action"))
        {
            SceneManager.LoadScene(2);
        }
    }
}

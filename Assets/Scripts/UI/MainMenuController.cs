using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    private Animator anim;

    public void Start() {
        anim = GetComponent<Animator>();
    }

    public void PlayGame() {
        SceneManager.LoadScene("MainScene");
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void TriggerSettings() {
        anim.SetTrigger("settings");
    }
}

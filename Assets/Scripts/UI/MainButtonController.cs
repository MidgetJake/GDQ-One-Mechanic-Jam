using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtonController : MonoBehaviour {

	public void Resume() {
        GameObject.FindGameObjectWithTag("UI").active = false;
    }

    public void Restart() {
        Application.LoadLevel("Level1");
    }

    public void MainMenu() {
        Application.LoadLevel("StarterScreen");
    }

    public void Quit() {
        Application.Quit();
    }
}

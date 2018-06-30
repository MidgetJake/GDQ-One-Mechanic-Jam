using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour {
    
    public void ChangeScene(string index){
        //SceneManager.LoadScene(index);
        Application.LoadLevel(index);
    }

    public void ExitGame() {
        Application.Quit();
    }

}

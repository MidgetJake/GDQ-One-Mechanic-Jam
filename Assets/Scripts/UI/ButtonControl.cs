using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour {

    public void ChangeScene(int index){
        
        //SceneManager.LoadScene(index);
        SceneManager.LoadSceneAsync(index);
    }

    public void ExitGame() {
        Application.Quit();
    }

}

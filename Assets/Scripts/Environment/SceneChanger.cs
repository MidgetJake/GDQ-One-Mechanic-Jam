using UnityEngine;
using UnityEngine.SceneManagement;

namespace Environment {
	public class SceneChanger : MonoBehaviour {
		public int nextLevel;
	
		public void NextLevel() {
			SceneManager.LoadSceneAsync(nextLevel);		
		}

		public void ResetLevel() {
			int index = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadSceneAsync(index);
		}
	}
}

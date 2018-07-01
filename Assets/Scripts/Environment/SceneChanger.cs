using UnityEngine;
using UnityEngine.SceneManagement;

namespace Environment {
	public class SceneChanger : MonoBehaviour {
		public int nextLevel;
		[SerializeField] private AudioSource m_portalSound;

		public void NextLevel() {
			SceneManager.LoadSceneAsync(nextLevel);
			m_portalSound.Play();
		}

		public void ResetLevel() {
			int index = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadSceneAsync(index);
		}
	}
}

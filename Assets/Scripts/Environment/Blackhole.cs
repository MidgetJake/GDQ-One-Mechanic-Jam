using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Blackhole : MonoBehaviour {
	public float scale = 1f;
	public bool isWhite;
	private SphereCollider m_Collider;

	private void Start() {
		m_Collider = GetComponent<SphereCollider>();
	}

	public float CalculateStrength(Transform other) {
		float magnitude = Mathf.Clamp(m_Collider.radius - Vector3.Distance(other.position, transform.position), 0f, 10f);
		
		return (1 + magnitude) * scale;
	}

	public Vector3 CalculateGravity(Transform other) {
		Vector3 relativeDirection = other.transform.position - transform.position;
		Vector3 gravityDirection = relativeDirection.normalized;
		
		return !isWhite ? gravityDirection : -gravityDirection;
	}

	private void OnCollisionEnter(Collision other) {
		if (other.transform.CompareTag("Player")) {
			other.transform.GetComponent<FirstPersonController>().Kill();
		}
	}
}

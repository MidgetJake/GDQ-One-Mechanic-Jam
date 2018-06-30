using UnityEngine;

public class Blackhole : MonoBehaviour {
	public bool isWhite;
	private SphereCollider m_Collider;

	private void Start() {
		m_Collider = GetComponent<SphereCollider>();
	}

	public float CalculateStrength(Transform other) {
		float magnitude = Mathf.Clamp(m_Collider.radius - Vector3.Distance(other.position, transform.position), 0f, 10f);
		
		return magnitude;
	}

	public Vector3 CalculateGravity(Transform other) {
		Vector3 relativeDirection = other.transform.position - transform.position;
		Vector3 gravityDirection = relativeDirection.normalized;
		
		return !isWhite ? gravityDirection : -gravityDirection;
	}
}

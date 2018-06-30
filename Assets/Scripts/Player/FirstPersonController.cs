using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson {
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour {
        public float attachRange = 30f;
        public float powerMax = 10f;
        public float currPower = 10f;
        public float chargeTime = 0.75f;
        public float shakeAmount = 0.5f;
        
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private ParticleSystem m_TrailParticleSystem;
        [SerializeField] private bool m_CameraShake = false;
        [SerializeField] private Image m_FadeImage;

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private AudioSource m_AudioSource;
        private bool m_Action = false;
        private float m_CurrCharge = 0f;
        private Rigidbody m_Rigidbody;
        private bool m_OnCooldown = false;
        private float m_CooldownTime = 0f;
        private bool m_HasCast;
        private Vector3 m_HitTransform;
        private bool m_Moving = false;
        private Vector3 m_ColliderNormal;

        [SerializeField] private GameObject UI;
        private bool activeMenu = false;

        // Use this for initialization
        private void Start() {
            m_MouseLook.SetCursorLock(true); // Default lock
            m_Camera = Camera.main;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void ToggleMenu() {
            if (UI.active) {
                // HIDE
                UI.active = false;
            } else {
                // SHOW
                UI.active = true;
            }
        }

        // Update is called once per frame
        private void Update()  {
            if (Input.GetKeyDown("escape")) {
                ToggleMenu();
            }

            if (UI.active) {
                Time.timeScale = 0;
                m_MouseLook.SetCursorLock(false);
            } else {
                Time.timeScale = 1;
                m_MouseLook.SetCursorLock(true);
            }

            RotateView();
            if (m_HasCast && !m_Moving) {
                m_Rigidbody.isKinematic = false;
                m_Rigidbody.AddForce(m_Camera.transform.forward * 1000);
                m_CurrCharge = 0f;
                m_Moving = true;
                m_ColliderNormal = Vector3.zero;
                //m_TrailParticleSystem.Play();
            }
            
            if (Input.GetMouseButton(1)) {
                m_Rigidbody.isKinematic = false;
                m_Rigidbody.AddForce(m_ColliderNormal);
                m_ColliderNormal = Vector3.zero;
            } 
            
            if (!m_OnCooldown) {
                if (Input.GetMouseButton(0)) {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, m_Camera.transform.forward, out hit,
                        attachRange)) {
                        if (hit.transform.CompareTag("Castable")) {
                            Debug.DrawRay(transform.position, m_Camera.transform.forward * hit.distance, Color.green);
                            m_CurrCharge += Time.deltaTime;
                            if (m_CurrCharge >= chargeTime) {
                                m_Rigidbody.isKinematic = true; // Freeze for 1 frame
                                m_HasCast = true;
                                m_HitTransform = hit.collider.transform.position;
                                m_Moving = false;
                            }
                        } else {
                            Debug.DrawRay(transform.position, m_Camera.transform.forward * hit.distance,
                                Color.red);
                            m_CurrCharge = 0f;
                        }

                        ;
                    } else {
                        Debug.DrawRay(transform.position, m_Camera.transform.forward * attachRange, Color.red);
                        m_CurrCharge = 0f;
                    }

                    ;
                } else {
                    m_CurrCharge = 0f;
                }
            } else {
                if (m_CooldownTime >= 1f) {
                    m_OnCooldown = false;
                    m_CooldownTime = 0f;
                    m_HasCast = false;
                } else {
                    m_CooldownTime += Time.deltaTime;
                }
            }
        }

        private void FixedUpdate() {

            if (m_CameraShake) {
                if (shakeAmount >= 1.25f) {
                    print("Game over!");
                }
                m_Camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                m_FadeImage.color = new Color(0, 0, 0, 1f * (shakeAmount - 0.25f));
                shakeAmount += Time.deltaTime / 10;
            } else {
                m_Camera.transform.localPosition = Vector3.zero;
                shakeAmount = 0f;
                m_FadeImage.color = new Color(0, 0, 0, 0);
            }
        }

        private void RotateView() {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }

        private void OnCollisionEnter(Collision other) {
            if (m_HasCast && other.transform.CompareTag("Castable") && other.collider.transform.position == m_HitTransform && !Input.GetMouseButton(1)) {
                m_Rigidbody.isKinematic = true;
                m_OnCooldown = true;
                m_TrailParticleSystem.Stop();
                m_ColliderNormal = other.contacts[0].normal * 50;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.transform.CompareTag("Bounds")) {
                m_CameraShake = true;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.transform.CompareTag("Bounds")) {
                m_CameraShake = false;
                shakeAmount = 0f;
            }
        }

        private void OnTriggerStay(Collider other) {
            if (other.transform.CompareTag("Hole")) {
                Vector3 gravityDir = other.transform.GetComponent<Blackhole>().CalculateGravity(transform);
                float magnitude = other.transform.GetComponent<Blackhole>().CalculateStrength(transform);
                m_Rigidbody.AddForce(-gravityDir * magnitude);
            }
        }
    }
}

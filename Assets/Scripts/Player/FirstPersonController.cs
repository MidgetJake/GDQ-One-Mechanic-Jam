using Environment;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson {
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour {
        public float attachRange = 30f;
        public int currPower = 2;
        public float chargeTime = 0.75f;
        public float shakeAmount = 0.5f;

        [SerializeField] private Material m_Grabable;
        [SerializeField] private Material m_NotGrabable;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private ParticleSystem m_TrailParticleSystem;
        [SerializeField] private bool m_CameraShake = false;
        [SerializeField] private Image m_FadeImage;
        [SerializeField] private Image m_Cell1;
        [SerializeField] private Image m_Cell2;
        [SerializeField] private GameObject m_DeathScreen;

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
        private Transform m_PastObjectCache;
        private bool m_CanPause = true;
        private bool m_Dead = false;

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
            if (Input.GetKeyDown("escape") && m_CanPause) {
                ToggleMenu();
            }

            if (UI.active) {
                Time.timeScale = 0;
                m_MouseLook.SetCursorLock(false);
            } else if(!m_Dead){
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
            
            //if (!m_OnCooldown) {
            if (Input.GetMouseButton(0) && currPower > 0) {
                RaycastHit hit;
                Physics.Raycast(transform.position, m_Camera.transform.forward, out hit, attachRange);
                Debug.DrawRay(transform.position, m_Camera.transform.forward * attachRange, Color.green);
                m_CurrCharge += Time.deltaTime;
                if (m_CurrCharge >= chargeTime) {
                    m_Rigidbody.isKinematic = true; // Freeze for 1 frame
                    m_HasCast = true;
                    m_Moving = false;
                    currPower--;
                    if (currPower == 1) {
                        m_Cell2.enabled = false;
                    } else {
                        m_Cell1.enabled = false;
                    }
                }
            } else {
                m_CurrCharge = 0f;
            }
        }

        private void FixedUpdate() {

            if (m_CameraShake) {
                if (shakeAmount >= 0.75f) {
                    Kill();
                }
                m_Camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                m_FadeImage.color = new Color(0, 0, 0, 1.34f * (shakeAmount));
                shakeAmount += Time.deltaTime / 10;
            } else {
                m_Camera.transform.localPosition = Vector3.zero;
                shakeAmount = 0f;
                m_FadeImage.color = new Color(0, 0, 0, 0);
            }
        }

        public void Kill() {
            m_Dead = true;
            m_CanPause = false;
            m_FadeImage.color = new Color(0, 0, 0, 1f);
            Time.timeScale = 0;
            m_MouseLook.SetCursorLock(false);
            m_DeathScreen.SetActive(true);
        }

        private void RotateView() {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }

        private void OnCollisionEnter(Collision other) {
            if (m_HasCast && other.transform.CompareTag("Castable") && m_HitTransform != other.transform.position && !Input.GetMouseButton(1)) {
                if (m_PastObjectCache) {
                    m_PastObjectCache.GetComponent<Renderer>().material = m_Grabable;
                }
                m_PastObjectCache = other.transform;
                m_PastObjectCache.GetComponent<Renderer>().material = m_NotGrabable;
                m_Rigidbody.isKinematic = true;
                m_OnCooldown = true;
                m_TrailParticleSystem.Stop();
                m_ColliderNormal = other.contacts[0].normal * 50;
                currPower = 2;
                m_Cell1.enabled = true;
                m_Cell2.enabled = true;
                m_HitTransform = other.transform.position;
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
            } else if (other.transform.CompareTag("End")) {
                other.GetComponent<SceneChanger>().NextLevel();
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

using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson {
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour {
        public float attachRange = 30f;
        public float powerMax = 10f;
        public float currPower = 10f;
        public float chargeTime = 2f;
        
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        private bool m_Action = false;
        private float m_CurrCharge = 0f;
        private Rigidbody m_Rigidbody;
        private bool m_OnCooldown = false;
        private float m_CooldownTime = 0f;
        private bool m_HasCast;
        private Vector3 m_HitTransform;

        // Use this for initialization
        private void Start() {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        private void Update()  {
            RotateView();
            if (!m_OnCooldown) {
                m_Action = false;
                if (Input.GetMouseButton(1)) {
                    m_Action = true;
                } else if (Input.GetMouseButton(0)) {
                    m_Action = true;
                }

                if (m_Action) {
                    RaycastHit hit;
                    if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hit,
                        attachRange)) {
                        if (hit.transform.CompareTag("Castable")) {
                            Debug.DrawRay(m_Camera.transform.position, m_Camera.transform.forward * hit.distance,
                                Color.green);
                            m_CurrCharge += Time.deltaTime;
                            if (m_CurrCharge >= chargeTime) {
                                m_Rigidbody.isKinematic = false;
                                m_Rigidbody.AddForce(m_Camera.transform.forward * 1000);
                                m_CurrCharge = 0f;
                                m_HasCast = true;
                                m_HitTransform = hit.collider.transform.position;
                            }
                        } else {
                            Debug.DrawRay(m_Camera.transform.position, m_Camera.transform.forward * hit.distance,
                                Color.red);
                            m_CurrCharge = 0f;
                        }

                        ;
                    } else {
                        Debug.DrawRay(m_Camera.transform.position, m_Camera.transform.forward * attachRange, Color.red);
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
            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            if (m_CharacterController.isGrounded) {
                m_MoveDir.y = -m_StickToGroundForce;
            } else  {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            //m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            //UpdateCameraPosition(speed);
            m_MouseLook.UpdateCursorLock();
        }

        private void RotateView() {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }

        private void OnCollisionEnter(Collision other) {
            if (m_HasCast && other.transform.CompareTag("Castable") && other.collider.transform.position != m_HitTransform) {
                m_Rigidbody.isKinematic = true;
                m_OnCooldown = true;
            }
        }
    }
}

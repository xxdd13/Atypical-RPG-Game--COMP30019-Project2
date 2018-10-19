using UnityEngine;
using Proj2.Message;
using System.Collections;

namespace Proj2
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour, IMessageReceiver
    {
        /// xxxxxxxxxxxxxxx
        public PlayerSkill m_skill;

        private Rigidbody rb;

        public Transform respawnPos;

        /// xxxxxxxxxxxxxx
        protected static PlayerController pc;
        public static PlayerController instance { get { return pc; } }

        public bool respawning { get { return m_Respawning; } }

        public float maxForwardSpeed = 20f;        
        public float gravity = 20f;              
        public float jumpSpeed = 10f;            
        public float minTurnSpeed = 400f;         
        public float maxTurnSpeed = 1200f;       
        public float idleTimeout = 5f;            
        public bool canAttack;                    

        public CameraSettings cameraSettings;
        public Camera cam;
        public CameraController camController;

        public MeleeWeapon meleeWeapon;                        
        public RandomAudioPlayer RandAudioHurtAudioPlayer;
        public RandomAudioPlayer RandAudioLandingPlayer;
        public RandomAudioPlayer RandAudioDeathPlayer;
        public RandomAudioPlayer RandAudioAttackPlayer;
        public RandomAudioPlayer RandAudioJumpPlayer;

        protected AnimatorStateInfo ani_cur_state;   
        protected AnimatorStateInfo m_NextStateInfo;
        protected bool m_IsAnimatorTransitioning;
        protected AnimatorStateInfo lastState;    
        protected AnimatorStateInfo nextState;
        protected bool lastTrans;
        protected bool onGround = true;            
        protected bool m_PreviouslyGrounded = true;   
        protected bool jumpReady;                  
        protected float absForwardSpeedMag;         
        protected float m_ForwardSpeed;               
        protected float vSpeed;              
        protected PlayerInput m_Input;                 
        protected CharacterController m_CharCtrl;    
        public Animator m_Animator;                
        protected Material m_CurrentWalkingSurface;    
        protected Quaternion m_TargetRotation;         
        protected float m_AngleDiff;                   
        //protected Collider[] m_OverlapResult = new Collider[8];    
        protected bool m_InAttack;                    
        protected bool m_InCombo;                      
        protected Damageable m_Damageable;             
        protected bool m_Respawning;                   
        protected float m_IdleTimer;                  


        float k_AirborneTurnSpeedProportion = 5.4f;
        float k_GroundedRayDistance = 1f;
        float k_JumpAbortSpeed = 10f;
        float k_MinEnemyDotCoeff = 0.2f;
        float k_InverseOneEighty = 1f / 180f;
        float k_StickingGravityProportion = 0.3f;
        float k_GroundAcceleration = 20f;
        float k_GroundDeceleration = 25f;


        // animator param 

        int ParamAirVSpeed = Animator.StringToHash("AirborneVerticalSpeed");
        int ParamForwardSpeed = Animator.StringToHash("ForwardSpeed");
        int ParamAngleRand = Animator.StringToHash("AngleDeltaRad");
        int TriggerParamIdle = Animator.StringToHash("TimeoutToIdle");
        int TriggerParamGround = Animator.StringToHash("Grounded");
        int TriggerParamInput = Animator.StringToHash("InputDetected");
        int TriggerParamMelee = Animator.StringToHash("MeleeAttack");
        int TriggerParamRB = Animator.StringToHash("RButtonAttack");
        int TriggerParamNuke = Animator.StringToHash("Nuke");
        int TriggerParamGunAuto = Animator.StringToHash("GunAuto");
        int TriggerParamIceSword = Animator.StringToHash("IceSword");
        int TriggerParamBallista = Animator.StringToHash("Ballista");
        int TriggerParamCross = Animator.StringToHash("cross");
        int ani_ParamHurt = Animator.StringToHash("Hurt");
        int ani_ParamDeath = Animator.StringToHash("Death");
        int ani_ParamRespawn = Animator.StringToHash("Respawn");
        int ani_ParamStateTime = Animator.StringToHash("StateTime");

        // States
        int ani_ParamLocomotion = Animator.StringToHash("Locomotion");
        int ani_ParamAirborne = Animator.StringToHash("Airborne");
        int ani_ParamLanding = Animator.StringToHash("Landing");  
        int ani_ParamMelee1 = Animator.StringToHash("EllenCombo1");
        int ani_ParamMelee2 = Animator.StringToHash("EllenCombo2");
        int ani_ParamMelee3 = Animator.StringToHash("EllenCombo3");
        int ani_ParamMelee4 = Animator.StringToHash("EllenCombo4");
        int ani_ParamPlayerDeath = Animator.StringToHash("Die");

        
        readonly int ani_ParamBlockInput = Animator.StringToHash("BlockInput");


        private PlayerCoolDown cd;

        private bool teleported = false;
        private int tpFrame = 4;

        protected bool IsMoveInput
        {
            get { return !Mathf.Approximately(m_Input.MoveInput.sqrMagnitude, 0f); }
        }

        public void SetCanAttack(bool canAttack)
        {
            this.canAttack = canAttack;
        }

        void Reset()
        {
            meleeWeapon = GetComponentInChildren<MeleeWeapon>();

            Transform hurtSource = transform.Find("HurtSource");
            if (hurtSource != null)
                RandAudioHurtAudioPlayer = hurtSource.GetComponent<RandomAudioPlayer>();

            Transform landingSource = transform.Find("LandingSource");
            if (landingSource != null)
                RandAudioLandingPlayer = landingSource.GetComponent<RandomAudioPlayer>();

            cameraSettings = FindObjectOfType<CameraSettings>();

            if (cameraSettings != null)
            {
                if (cameraSettings.follow == null)
                    cameraSettings.follow = transform;

                if (cameraSettings.lookAt == null)
                    cameraSettings.follow = transform.Find("HeadTarget");
            }
        }

        void Awake()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            camController = cam.GetComponent<CameraController>();
            rb = GetComponent<Rigidbody>();
            m_Input = GetComponent<PlayerInput>();
            m_Animator = GetComponent<Animator>();
            m_CharCtrl = GetComponent<CharacterController>();
            cd = GetComponent<PlayerCoolDown>();
            m_skill = GetComponent<PlayerSkill>();
            meleeWeapon.SetOwner(gameObject);

            pc = this;

            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Locked;

        }

        
        void OnEnable()
        {        
            m_Damageable = GetComponent<Damageable>();
            m_Damageable.isInvulnerable = true;
        }


        void OnDisable(){}


        void FixedUpdate()
        {
            RecordAniState();

            UpdateInputBlocking();

            

            m_Animator.SetFloat(ani_ParamStateTime, Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
            m_Animator.ResetTrigger(TriggerParamMelee);

            m_Animator.ResetTrigger(TriggerParamRB);

            if (m_Input.Attack && canAttack)
                m_Animator.SetTrigger(TriggerParamMelee);

            if (m_Input.RButton && canAttack) {
                if (cd.rb)
                {
                    ChangeOrientation();
                    m_Animator.SetTrigger(TriggerParamRB);         
                }
                    
            }
            

            //nukkkkkkkkkkkke
            if (m_Input.NukeButton && canAttack && cd.nuke)
            {
                ChangeOrientation();
                m_Animator.SetTrigger(TriggerParamNuke);
            }


            //guided spell
            if (m_Input.GuidedSpell && canAttack && cd.guidedSpell)
            {
                ChangeOrientation();
                m_Animator.SetTrigger(TriggerParamGunAuto);
            }


            //iceSword
            if(m_Input.IceSword && canAttack && cd.iceSword)
            {
                ChangeOrientation();
                m_Animator.SetTrigger(TriggerParamIceSword);
            }

            //ballista
            if (m_Input.Ballista && canAttack && cd.ballista)
            {
                ChangeOrientation();
                m_Animator.SetTrigger(TriggerParamBallista);
            }

            //cross
            if (m_Input.Cross && canAttack && cd.cross)
            {
                ChangeOrientation();
                m_Animator.SetTrigger(TriggerParamCross);
            }


            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("RButtonAttack") && cd.rb)
            {

                cd.rbCast();
                m_skill.rb();
                RandAudioAttackPlayer.PlayRandomClip();
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Nuke") && cd.nuke)
            {
                cd.nukeCast();
                m_skill.nuke();
                RandAudioAttackPlayer.PlayRandomClip();
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("EllenGunShoot") && cd.guidedSpell)
            {
                cd.guidedSpellCast();
                m_skill.guidedTrigger = true;
                RandAudioAttackPlayer.PlayRandomClip();
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("IceSword") && cd.iceSword)
            {
                cd.iceSwordCast();
                m_skill.iceSwordTrigger = true;
                RandAudioAttackPlayer.PlayRandomClip();
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Ballista") && cd.ballista)
            {
                cd.ballistaCast();
                m_skill.ballista();
                RandAudioAttackPlayer.PlayRandomClip();

            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("cross-exec") && cd.cross)
            {
                cd.crossCast();
                m_skill.cross();
                RandAudioAttackPlayer.PlayRandomClip();

            }
            


            //blink skills
            /**
            if (Input.GetKeyUp(KeyCode.LeftShift) ) {
                RaycastHit hit;
                Vector3 forwardPos = transform.position + transform.forward;
                m_skill.teleport();
                
                Physics.Raycast(forwardPos, transform.forward, out hit, 12f);
                    if (hit.collider != null) //need to stop right before collision
                    {
                        print(hit.collider.gameObject.name);
                        Vector3 teleportPos = hit.point - (transform.forward); //don't wanna stuck, so step back a few units
                        this.transform.position = teleportPos;
                    }
                    else
                    { // hit nothing, just eleport
                        this.transform.position += this.transform.forward * 12f;
                    }

                
                

            /**
            rb.AddForce(transform.forward * 100f, ForceMode.Impulse);

            //teleport force added
            teleported = true;
            tpFrame = 30;

            




        }

            //teleport to gold dragon
            if (Input.GetKeyDown("t"))
            {
                this.transform.position = new Vector3(-88.27f, 0.5f, 113f);


            }
            //teleport to final boss
            if (Input.GetKeyDown("b"))
            {
                this.transform.position = new Vector3(-71.01f, 116f, -256f);

            }

            **/


            MoveForward();
            Jump();
           
            ChangeOrientation();

            MeleeAdd();
            AudioPLay();

            inputIdle();

            m_PreviouslyGrounded = onGround;

            /*
            if (teleported) {
                if (tpFrame <= 0)
                {
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    teleported = false;
                    print("stop");
                    
                }
                else {
                    
                    tpFrame -= 1 ;
                }
                
                
            }
            rb.isKinematic = false;
            */

        }

        // Called at the start of FixedUpdate to record the current state of the base layer of the animator.
        void RecordAniState()
        {
            lastState = ani_cur_state;
            nextState = m_NextStateInfo;
            lastTrans = m_IsAnimatorTransitioning;

            ani_cur_state = m_Animator.GetCurrentAnimatorStateInfo(0);
            m_NextStateInfo = m_Animator.GetNextAnimatorStateInfo(0);
            m_IsAnimatorTransitioning = m_Animator.IsInTransition(0);
        }

        // Called after the animator state has been cached to determine whether this script should block user input.
        void UpdateInputBlocking()
        {
            bool inputBlocked = ani_cur_state.tagHash == ani_ParamBlockInput && !m_IsAnimatorTransitioning;
            inputBlocked |= m_NextStateInfo.tagHash == ani_ParamBlockInput;
            m_Input.playerControllerInputBlocked = inputBlocked;
        }
        

        void MoveForward()
        {
            Vector2 moveInput = m_Input.MoveInput;
            if (moveInput.sqrMagnitude > 1f)
                moveInput.Normalize();

            absForwardSpeedMag = moveInput.magnitude * maxForwardSpeed;

            float a = IsMoveInput ? k_GroundAcceleration : k_GroundDeceleration*3f;

            m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, absForwardSpeedMag, a * Time.deltaTime);
            m_Animator.SetFloat(ParamForwardSpeed, m_ForwardSpeed);
        }

        void Jump()
        {
            
            if (!m_Input.JumpInput && onGround)
                jumpReady = true;

            if (onGround)
            {

                vSpeed = -gravity * k_StickingGravityProportion;
            }
            else
            {
                
                if (!m_Input.JumpInput && vSpeed > 0.0f)
                {

                    vSpeed -= k_JumpAbortSpeed * Time.deltaTime;
                }

                if (Mathf.Approximately(0f, vSpeed))
                {
                    vSpeed = 0f;
                }

                // downward force
                vSpeed += gravity * Time.deltaTime*-1;
            }

            if (onGround && m_Input.JumpInput && jumpReady && !m_InCombo)
            {

                    vSpeed = jumpSpeed;
                    onGround = false;
                    jumpReady = false;
            }
            
            
        }


        void ChangeOrientation()
        {
            Quaternion targetRotation;
            Vector2 moveInput = m_Input.MoveInput;
            Vector3 forward = Quaternion.Euler(0f, camController.x, 0f) * Vector3.forward;
            Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            
            targetRotation = Quaternion.LookRotation(Quaternion.FromToRotation(Vector3.forward, localMovementDirection) * forward);
            if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
            {
                targetRotation = Quaternion.LookRotation(-forward);
            }
            transform.rotation = targetRotation;
        }

        void MeleeAdd() {
            if (ani_cur_state.shortNameHash == ani_ParamMelee1 && lastState.shortNameHash != ani_ParamMelee1)
            {
                RandAudioAttackPlayer.PlayRandomClip();
                m_skill.rb2();
            }

            if (ani_cur_state.shortNameHash == ani_ParamMelee2 && lastState.shortNameHash != ani_ParamMelee2)
            {
                RandAudioAttackPlayer.PlayRandomClip();
                m_skill.rb2();
            }
            if (ani_cur_state.shortNameHash == ani_ParamMelee3 && lastState.shortNameHash != ani_ParamMelee3)
            {
                RandAudioAttackPlayer.PlayRandomClip();
                m_skill.rb2();
            }
            if (ani_cur_state.shortNameHash == ani_ParamMelee4 && lastState.shortNameHash != ani_ParamMelee4)
            {
                RandAudioAttackPlayer.PlayRandomClip();
                m_skill.rb2();
            }
        }

        void AudioPLay()
        {

            if (!onGround && m_PreviouslyGrounded && vSpeed > 0f)
            {
                RandAudioJumpPlayer.PlayRandomClip();
            }

            if (ani_cur_state.shortNameHash == ani_ParamHurt && lastState.shortNameHash != ani_ParamHurt)
            {
                RandAudioHurtAudioPlayer.PlayRandomClip();
            }

            if (ani_cur_state.shortNameHash == ani_ParamPlayerDeath && lastState.shortNameHash != ani_ParamPlayerDeath)
            {
                RandAudioDeathPlayer.PlayRandomClip();
            }

            
        }

        void inputIdle()
        {
            bool inputDetected = IsMoveInput || m_Input.Attack || m_Input.JumpInput || m_Input.RButton || m_Input.NukeButton || m_Input.GuidedSpell || m_Input.IceSword || m_Input.Ballista || m_Input.Cross;
            if (onGround && !inputDetected)
            {
                m_IdleTimer += Time.deltaTime;

                if (m_IdleTimer >= idleTimeout)
                {
                    m_IdleTimer = 0f;
                    m_Animator.SetTrigger(TriggerParamIdle);
                }
            }
            else
            {
                m_IdleTimer = 0f;
                m_Animator.ResetTrigger(TriggerParamIdle);
            }

            m_Animator.SetBool(TriggerParamInput, inputDetected);
        }

        // Called each physics step (so long as the Animator component is set to Animate Physics) after FixedUpdate to override root motion.
        void OnAnimatorMove()
        {
            Vector3 movement;

            if (onGround)
            {
               
                RaycastHit hit;
                Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
                if (Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    
                    movement = Vector3.ProjectOnPlane(m_Animator.deltaPosition, hit.normal);
                    
                    
                }
                else
                {
                    
                    movement = m_Animator.deltaPosition;
                    m_CurrentWalkingSurface = null;
                }
            }
            else
            {
                movement = m_ForwardSpeed * transform.forward * Time.deltaTime;
            }


            m_CharCtrl.transform.rotation *= m_Animator.deltaRotation;

            movement += vSpeed * Vector3.up * Time.deltaTime;

            m_CharCtrl.Move(movement);

            onGround = m_CharCtrl.isGrounded;


            if (!onGround)
                m_Animator.SetFloat(ParamAirVSpeed, vSpeed);

            m_Animator.SetBool(TriggerParamGround, onGround);
        }
        

        public void MeleeAttackStart(int throwing = 0)
        {
            meleeWeapon.BeginAttack(throwing != 0);
            m_InAttack = true;
        }

        // This is called by an animation event when Ellen finishes swinging her staff.
        public void MeleeAttackEnd()
        {
            meleeWeapon.EndAttack();
            m_InAttack = false;
        }

       
        public void Respawn()
        {
            StartCoroutine(Respawning());
        }
        
        protected IEnumerator Respawning()            
        {
            
            // Wait for the animator to be transitioning from the EllenDeath state.
            while (ani_cur_state.shortNameHash != ani_ParamPlayerDeath)
            {
                yield return null;
            }
            
            

            //teleport to spawn pos
            this.transform.position = respawnPos.position+ respawnPos.up*3f+ respawnPos.forward*3f;

            // Get Ellen's health back.
            m_Damageable.ResetDamage();

            // Set the Respawn parameter of the animator.
            m_Animator.SetTrigger(ani_ParamRespawn);
            
            
            
        }

        public void RespawnFinished()
        {
            m_Respawning = false;
            m_Damageable.isInvulnerable = false;
        }

        // Called by Ellen's Damageable when she is hurt.
        public void OnReceiveMessage(MessageType type, object sender, object data)
        {
            switch (type)
            {
                case MessageType.DAMAGED:
                    {
                        Damageable.DamageMessage damageData = (Damageable.DamageMessage)data;
                        Damaged(damageData);
                    }
                    break;
                case MessageType.DEAD:
                    {
                        Damageable.DamageMessage damageData = (Damageable.DamageMessage)data;
                        Die(damageData);
                    }
                    break;
            }
        }

        // Called by OnReceiveMessage.
        void Damaged(Damageable.DamageMessage damageMessage)
        {
            // Set the Hurt parameter of the animator.
            m_Animator.SetTrigger(ani_ParamHurt);

           

            // Play an audio clip of being hurt.
            if (RandAudioHurtAudioPlayer != null)
            {
                RandAudioHurtAudioPlayer.PlayRandomClip();
            }
        }

        // Called by OnReceiveMessage and by DeathVolumes in the scene.
        public void Die(Damageable.DamageMessage damageMessage)
        {
            m_Animator.SetTrigger(ani_ParamDeath);
            m_ForwardSpeed = 0f;
            vSpeed = 0f;
            m_Respawning = true;
            m_Damageable.isInvulnerable = true;
        }
    }
}
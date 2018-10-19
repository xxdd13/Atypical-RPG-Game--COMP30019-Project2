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
        public MeleeWeapon meleeWeapon;                  
        public RandomAudioPlayer footstepPlayer;         
        public RandomAudioPlayer hurtAudioPlayer;
        public RandomAudioPlayer landingPlayer;
        public RandomAudioPlayer emoteLandingPlayer;
        public RandomAudioPlayer emoteDeathPlayer;
        public RandomAudioPlayer emoteAttackPlayer;
        public RandomAudioPlayer emoteJumpPlayer;

        protected AnimatorStateInfo ani_cur_state;   
        protected AnimatorStateInfo m_NextStateInfo;
        protected bool m_IsAnimatorTransitioning;
        protected AnimatorStateInfo m_PreviousCurrentStateInfo;    
        protected AnimatorStateInfo m_PreviousNextStateInfo;
        protected bool m_PreviousIsAnimatorTransitioning;
        protected bool onGround = true;            
        protected bool m_PreviouslyGrounded = true;   
        protected bool m_ReadyToJump;                  
        protected float m_DesiredForwardSpeed;         
        protected float m_ForwardSpeed;               
        protected float m_VerticalSpeed;              
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


        const float k_AirborneTurnSpeedProportion = 5.4f;
        const float k_GroundedRayDistance = 1f;
        const float k_JumpAbortSpeed = 10f;
        const float k_MinEnemyDotCoeff = 0.2f;
        const float k_InverseOneEighty = 1f / 180f;
        const float k_StickingGravityProportion = 0.3f;
        const float k_GroundAcceleration = 20f;
        const float k_GroundDeceleration = 25f;


        // animator param 

        protected int ParamAirVSpeed = Animator.StringToHash("AirborneVerticalSpeed");
        protected int ParamForwardSpeed = Animator.StringToHash("ForwardSpeed");
        protected int ParamAngleRand = Animator.StringToHash("AngleDeltaRad");
        protected int TriggerParamIdle = Animator.StringToHash("TimeoutToIdle");
        protected int TriggerParamGround = Animator.StringToHash("Grounded");
        protected int TriggerParamInput = Animator.StringToHash("InputDetected");
        protected int TriggerParamMelee = Animator.StringToHash("MeleeAttack");
        protected int TriggerParamRB = Animator.StringToHash("RButtonAttack");
        protected int TriggerParamNuke = Animator.StringToHash("Nuke");
        protected int TriggerParamGunAuto = Animator.StringToHash("GunAuto");
        protected int TriggerParamIceSword = Animator.StringToHash("IceSword");
        protected int TriggerParamBallista = Animator.StringToHash("Ballista");
        protected int TriggerParamCross = Animator.StringToHash("cross");


        readonly int ani_ParamHurt = Animator.StringToHash("Hurt");
        readonly int ani_ParamDeath = Animator.StringToHash("Death");
        readonly int ani_ParamRespawn = Animator.StringToHash("Respawn");
        readonly int ani_ParamHurtFromX = Animator.StringToHash("HurtFromX");
        readonly int ani_ParamHurtFromY = Animator.StringToHash("HurtFromY");
        readonly int ani_ParamStateTime = Animator.StringToHash("StateTime");
        readonly int ani_ParamFootFall = Animator.StringToHash("FootFall");

        // States
        readonly int ani_ParamLocomotion = Animator.StringToHash("Locomotion");
        readonly int ani_ParamAirborne = Animator.StringToHash("Airborne");
        readonly int ani_ParamLanding = Animator.StringToHash("Landing");    // Also a parameter.
        readonly int ani_ParamEllenCombo1 = Animator.StringToHash("EllenCombo1");
        readonly int ani_ParamEllenCombo2 = Animator.StringToHash("EllenCombo2");
        readonly int ani_ParamEllenCombo3 = Animator.StringToHash("EllenCombo3");
        readonly int ani_ParamEllenCombo4 = Animator.StringToHash("EllenCombo4");
        readonly int ani_ParamEllenDeath = Animator.StringToHash("EllenDeath");

        // Tags
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

        // Called automatically by Unity when the script is first added to a gameobject or is reset from the context menu.
        void Reset()
        {
            meleeWeapon = GetComponentInChildren<MeleeWeapon>();

            Transform footStepSource = transform.Find("FootstepSource");
            if (footStepSource != null)
                footstepPlayer = footStepSource.GetComponent<RandomAudioPlayer>();

            Transform hurtSource = transform.Find("HurtSource");
            if (hurtSource != null)
                hurtAudioPlayer = hurtSource.GetComponent<RandomAudioPlayer>();

            Transform landingSource = transform.Find("LandingSource");
            if (landingSource != null)
                landingPlayer = landingSource.GetComponent<RandomAudioPlayer>();

            cameraSettings = FindObjectOfType<CameraSettings>();

            if (cameraSettings != null)
            {
                if (cameraSettings.follow == null)
                    cameraSettings.follow = transform;

                if (cameraSettings.lookAt == null)
                    cameraSettings.follow = transform.Find("HeadTarget");
            }
        }

        // Called automatically by Unity when the script first exists in the scene.
        void Awake()
        {
            m_Input = GetComponent<PlayerInput>();
            m_Animator = GetComponent<Animator>();
            m_CharCtrl = GetComponent<CharacterController>();
            rb = GetComponent<Rigidbody>();
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
            m_Damageable.onDamageMessageReceivers.Add(this);
            m_Damageable.isInvulnerable = true;
        }


        void OnDisable(){}


        void FixedUpdate()
        {
            CacheAnimatorState();

            UpdateInputBlocking();

            EquipMeleeWeapon(IsWeaponEquiped());

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
                emoteAttackPlayer.PlayRandomClip();
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Nuke") && cd.nuke)
            {
                cd.nukeCast();
                m_skill.nuke();
                emoteAttackPlayer.PlayRandomClip();
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("EllenGunShoot") && cd.guidedSpell)
            {
                cd.guidedSpellCast();
                m_skill.guidedTrigger = true;
                emoteAttackPlayer.PlayRandomClip();
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("IceSword") && cd.iceSword)
            {
                cd.iceSwordCast();
                m_skill.iceSwordTrigger = true;
                emoteAttackPlayer.PlayRandomClip();
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Ballista") && cd.ballista)
            {
                cd.ballistaCast();
                m_skill.ballista();
                emoteAttackPlayer.PlayRandomClip();

            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("cross-exec") && cd.cross)
            {
                cd.crossCast();
                m_skill.cross();
                emoteAttackPlayer.PlayRandomClip();

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

            AudioPLay();

            TimeoutToIdle();

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
        void CacheAnimatorState()
        {
            m_PreviousCurrentStateInfo = ani_cur_state;
            m_PreviousNextStateInfo = m_NextStateInfo;
            m_PreviousIsAnimatorTransitioning = m_IsAnimatorTransitioning;

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

        // Called after the animator state has been cached to determine whether or not the staff should be active or not.
        bool IsWeaponEquiped()
        {
            bool equipped = m_NextStateInfo.shortNameHash == ani_ParamEllenCombo1 || ani_cur_state.shortNameHash == ani_ParamEllenCombo1;
            equipped |= m_NextStateInfo.shortNameHash == ani_ParamEllenCombo2 || ani_cur_state.shortNameHash == ani_ParamEllenCombo2;
            equipped |= m_NextStateInfo.shortNameHash == ani_ParamEllenCombo3 || ani_cur_state.shortNameHash == ani_ParamEllenCombo3;
            equipped |= m_NextStateInfo.shortNameHash == ani_ParamEllenCombo4 || ani_cur_state.shortNameHash == ani_ParamEllenCombo4;

            return equipped;
        }

        // Called each physics step with a parameter based on the return value of IsWeaponEquiped.
        void EquipMeleeWeapon(bool equip)
        {
            meleeWeapon.gameObject.SetActive(equip);
            m_InAttack = false;
            m_InCombo = equip;

            if (!equip)
                m_Animator.ResetTrigger(TriggerParamMelee);
        }

        // Called each physics step.
        void MoveForward()
        {
            // Cache the move input and cap it's magnitude at 1.
            Vector2 moveInput = m_Input.MoveInput;
            if (moveInput.sqrMagnitude > 1f)
                moveInput.Normalize();

            // Calculate the speed intended by input.
            m_DesiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;

            // Determine change to speed based on whether there is currently any move input.
            float acceleration = IsMoveInput ? k_GroundAcceleration : k_GroundDeceleration*3f;

            // Adjust the forward speed towards the desired speed.
            m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, m_DesiredForwardSpeed, acceleration * Time.deltaTime);

            // Set the animator parameter to control what animation is being played.
            m_Animator.SetFloat(ParamForwardSpeed, m_ForwardSpeed);
        }

        // Called each physics step.
        void Jump()
        {
            // If jump is not currently held and Ellen is on the ground then she is ready to jump.
            if (!m_Input.JumpInput && onGround)
                m_ReadyToJump = true;

            if (onGround)
            {
                // When grounded we apply a slight negative vertical speed to make Ellen "stick" to the ground.
                m_VerticalSpeed = -gravity * k_StickingGravityProportion;

                // If jump is held, Ellen is ready to jump and not currently in the middle of a melee combo...
                if (m_Input.JumpInput && m_ReadyToJump && !m_InCombo)
                {
                    // ... then override the previously set vertical speed and make sure she cannot jump again.
                    m_VerticalSpeed = jumpSpeed;
                    onGround = false;
                    m_ReadyToJump = false;
                }
            }
            else
            {
                // If Ellen is airborne, the jump button is not held and Ellen is currently moving upwards...
                if (!m_Input.JumpInput && m_VerticalSpeed > 0.0f)
                {
                    // ... decrease Ellen's vertical speed.
                    // This is what causes holding jump to jump higher that tapping jump.
                    m_VerticalSpeed -= k_JumpAbortSpeed * Time.deltaTime;
                }

                // If a jump is approximately peaking, make it absolute.
                if (Mathf.Approximately(m_VerticalSpeed, 0f))
                {
                    m_VerticalSpeed = 0f;
                }
                
                // If Ellen is airborne, apply gravity.
                m_VerticalSpeed -= gravity * Time.deltaTime;
            }
        }


        

        // Called each physics step after SetTargetRotation if there is move input and Ellen is in the correct animator state according to IsOrientationUpdated.
        void ChangeOrientation()
        {
            Quaternion targetRotation;
            Vector2 moveInput = m_Input.MoveInput;
            Vector3 forward = Quaternion.Euler(0f, cameraSettings.Current.m_XAxis.Value, 0f) * Vector3.forward;
            Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            
            targetRotation = Quaternion.LookRotation(Quaternion.FromToRotation(Vector3.forward, localMovementDirection) * forward);
            if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
            {
                targetRotation = Quaternion.LookRotation(-forward);
            }
            transform.rotation = targetRotation;
        }


        // Called each physics step to check if audio should be played and if so instruct the relevant random audio player to do so.
        void AudioPLay()
        {

            if (!onGround && m_PreviouslyGrounded && m_VerticalSpeed > 0f)
            {
                emoteJumpPlayer.PlayRandomClip();
            }

            if (ani_cur_state.shortNameHash == ani_ParamHurt && m_PreviousCurrentStateInfo.shortNameHash != ani_ParamHurt)
            {
                hurtAudioPlayer.PlayRandomClip();
            }

            if (ani_cur_state.shortNameHash == ani_ParamEllenDeath && m_PreviousCurrentStateInfo.shortNameHash != ani_ParamEllenDeath)
            {
                emoteDeathPlayer.PlayRandomClip();
            }

            
        }

        // Called each physics step to count up to the point where Ellen considers a random idle.
        void TimeoutToIdle()
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
                // ... raycast into the ground...
                RaycastHit hit;
                Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
                if (Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    // ... and get the movement of the root motion rotated to lie along the plane of the ground.
                    movement = Vector3.ProjectOnPlane(m_Animator.deltaPosition, hit.normal);
                    
                    // Also store the current walking surface so the correct audio is played.
                    Renderer groundRenderer = hit.collider.GetComponentInChildren<Renderer>();
                    m_CurrentWalkingSurface = groundRenderer ? groundRenderer.sharedMaterial : null;
                }
                else
                {
                    // If no ground is hit just get the movement as the root motion.
                    // Theoretically this should rarely happen as when grounded the ray should always hit.
                    movement = m_Animator.deltaPosition;
                    m_CurrentWalkingSurface = null;
                }
            }
            else
            {
                movement = m_ForwardSpeed * transform.forward * Time.deltaTime;
            }


            m_CharCtrl.transform.rotation *= m_Animator.deltaRotation;

            movement += m_VerticalSpeed * Vector3.up * Time.deltaTime;

            m_CharCtrl.Move(movement);

            onGround = m_CharCtrl.isGrounded;


            if (!onGround)
                m_Animator.SetFloat(ParamAirVSpeed, m_VerticalSpeed);

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

       

        // This is usually called by a state machine behaviour on the animator controller but can be called from anywhere.
        public void Respawn()
        {
            StartCoroutine(Respawning());
        }
        
        protected IEnumerator Respawning()
        {
            // Wait for the animator to be transitioning from the EllenDeath state.
            while (ani_cur_state.shortNameHash != ani_ParamEllenDeath || !m_IsAnimatorTransitioning)
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

        // Called by a state machine behaviour on Ellen's animator controller.
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
            if (hurtAudioPlayer != null)
            {
                hurtAudioPlayer.PlayRandomClip();
            }
        }

        // Called by OnReceiveMessage and by DeathVolumes in the scene.
        public void Die(Damageable.DamageMessage damageMessage)
        {
            m_Animator.SetTrigger(ani_ParamDeath);
            m_ForwardSpeed = 0f;
            m_VerticalSpeed = 0f;
            m_Respawning = true;
            m_Damageable.isInvulnerable = true;
        }
    }
}
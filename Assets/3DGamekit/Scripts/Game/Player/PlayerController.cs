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
        protected static PlayerController s_Instance;
        public static PlayerController instance { get { return s_Instance; } }

        public bool respawning { get { return m_Respawning; } }

        public float maxForwardSpeed = 20f;        // How fast Ellen can run.
        public float gravity = 20f;               // How fast Ellen accelerates downwards when airborne.
        public float jumpSpeed = 10f;             // How fast Ellen takes off when jumping.
        public float minTurnSpeed = 400f;         // How fast Ellen turns when moving at maximum speed.
        public float maxTurnSpeed = 1200f;        // How fast Ellen turns when stationary.
        public float idleTimeout = 5f;            // How long before Ellen starts considering random idles.
        public bool canAttack;                    // Whether or not Ellen can swing her staff.

        public CameraSettings cameraSettings;            // Reference used to determine the camera's direction.
        public MeleeWeapon meleeWeapon;                  // Reference used to (de)activate the staff when attacking. 
        public RandomAudioPlayer footstepPlayer;         // Random Audio Players used for various situations.
        public RandomAudioPlayer hurtAudioPlayer;
        public RandomAudioPlayer landingPlayer;
        public RandomAudioPlayer emoteLandingPlayer;
        public RandomAudioPlayer emoteDeathPlayer;
        public RandomAudioPlayer emoteAttackPlayer;
        public RandomAudioPlayer emoteJumpPlayer;

        protected AnimatorStateInfo m_CurrentStateInfo;    // Information about the base layer of the animator cached.
        protected AnimatorStateInfo m_NextStateInfo;
        protected bool m_IsAnimatorTransitioning;
        protected AnimatorStateInfo m_PreviousCurrentStateInfo;    // Information about the base layer of the animator from last frame.
        protected AnimatorStateInfo m_PreviousNextStateInfo;
        protected bool m_PreviousIsAnimatorTransitioning;
        protected bool m_IsGrounded = true;            // Whether or not Ellen is currently standing on the ground.
        protected bool m_PreviouslyGrounded = true;    // Whether or not Ellen was standing on the ground last frame.
        protected bool m_ReadyToJump;                  // Whether or not the input state and Ellen are correct to allow jumping.
        protected float m_DesiredForwardSpeed;         // How fast Ellen aims be going along the ground based on input.
        protected float m_ForwardSpeed;                // How fast Ellen is currently going along the ground.
        protected float m_VerticalSpeed;               // How fast Ellen is currently moving up or down.
        protected PlayerInput m_Input;                 // Reference used to determine how Ellen should move.
        protected CharacterController m_CharCtrl;      // Reference used to actually move Ellen.
        public Animator m_Animator;                 // Reference used to make decisions based on Ellen's current animation and to set parameters.
        protected Material m_CurrentWalkingSurface;    // Reference used to make decisions about audio.
        protected Quaternion m_TargetRotation;         // What rotation Ellen is aiming to have based on input.
        protected float m_AngleDiff;                   // Angle in degrees between Ellen's current rotation and her target rotation.
        protected Collider[] m_OverlapResult = new Collider[8];    // Used to cache colliders that are near Ellen.
        protected bool m_InAttack;                     // Whether Ellen is currently in the middle of a melee attack.
        protected bool m_InCombo;                      // Whether Ellen is currently in the middle of her melee combo.
        protected Damageable m_Damageable;             // Reference used to set invulnerablity and health based on respawning.
        protected Renderer[] m_Renderers;              // References used to make sure Renderers are reset properly. 
        
        protected bool m_Respawning;                   // Whether Ellen is currently respawning.
        protected float m_IdleTimer;                   // Used to count up to Ellen considering a random idle.

        // These constants are used to ensure Ellen moves and behaves properly.
        // It is advised you don't change them without fully understanding what they do in code.
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


        readonly int m_HashHurt = Animator.StringToHash("Hurt");
        readonly int m_HashDeath = Animator.StringToHash("Death");
        readonly int m_HashRespawn = Animator.StringToHash("Respawn");
        readonly int m_HashHurtFromX = Animator.StringToHash("HurtFromX");
        readonly int m_HashHurtFromY = Animator.StringToHash("HurtFromY");
        readonly int m_HashStateTime = Animator.StringToHash("StateTime");
        readonly int m_HashFootFall = Animator.StringToHash("FootFall");

        // States
        readonly int m_HashLocomotion = Animator.StringToHash("Locomotion");
        readonly int m_HashAirborne = Animator.StringToHash("Airborne");
        readonly int m_HashLanding = Animator.StringToHash("Landing");    // Also a parameter.
        readonly int m_HashEllenCombo1 = Animator.StringToHash("EllenCombo1");
        readonly int m_HashEllenCombo2 = Animator.StringToHash("EllenCombo2");
        readonly int m_HashEllenCombo3 = Animator.StringToHash("EllenCombo3");
        readonly int m_HashEllenCombo4 = Animator.StringToHash("EllenCombo4");
        readonly int m_HashEllenDeath = Animator.StringToHash("EllenDeath");

        // Tags
        readonly int m_HashBlockInput = Animator.StringToHash("BlockInput");


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

            /////xxxxxxxxxxxxxxxxx
            cd = GetComponent<PlayerCoolDown>();
            m_skill = GetComponent<PlayerSkill>();

            meleeWeapon.SetOwner(gameObject);

            s_Instance = this;

            Cursor.lockState = CursorLockMode.None;

            // This locks the cursor
            Cursor.lockState = CursorLockMode.Locked;

        }

        // Called automatically by Unity after Awake whenever the script is enabled. 
        void OnEnable()
        {
            //SceneLinkedSMB<PlayerController>.Initialise(m_Animator, this);

            m_Damageable = GetComponent<Damageable>();
            m_Damageable.onDamageMessageReceivers.Add(this);

            m_Damageable.isInvulnerable = true;

            EquipMeleeWeapon(false);

            m_Renderers = GetComponentsInChildren<Renderer>();
        }

        // Called automatically by Unity whenever the script is disabled.
        void OnDisable()
        {
            m_Damageable.onDamageMessageReceivers.Remove(this);

            for (int i = 0; i < m_Renderers.Length; ++i)
            {
                m_Renderers[i].enabled = true;
            }
        }

        // Called automatically by Unity once every Physics step.
        void FixedUpdate()
        {
            CacheAnimatorState();

            UpdateInputBlocking();

            EquipMeleeWeapon(IsWeaponEquiped());

            m_Animator.SetFloat(m_HashStateTime, Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
            m_Animator.ResetTrigger(TriggerParamMelee);

            m_Animator.ResetTrigger(TriggerParamRB);

            if (m_Input.Attack && canAttack)
                m_Animator.SetTrigger(TriggerParamMelee);

            if (m_Input.RButton && canAttack) {
                if (cd.rb)
                {
                    UpdateOrientation();
                    m_Animator.SetTrigger(TriggerParamRB);         
                }
                    
            }
            

            //nukkkkkkkkkkkke
            if (m_Input.NukeButton && canAttack && cd.nuke)
            {
                UpdateOrientation();
                m_Animator.SetTrigger(TriggerParamNuke);
            }


            //guided spell
            if (m_Input.GuidedSpell && canAttack && cd.guidedSpell)
            {
                UpdateOrientation();
                m_Animator.SetTrigger(TriggerParamGunAuto);
            }


            //iceSword
            if(m_Input.IceSword && canAttack && cd.iceSword)
            {
                UpdateOrientation();
                m_Animator.SetTrigger(TriggerParamIceSword);
            }

            //ballista
            if (m_Input.Ballista && canAttack && cd.ballista)
            {
                UpdateOrientation();
                m_Animator.SetTrigger(TriggerParamBallista);
            }

            //cross
            if (m_Input.Cross && canAttack && cd.cross)
            {
                UpdateOrientation();
                m_Animator.SetTrigger(TriggerParamCross);
            }


            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("RButtonAttack") && cd.rb)
            {
                cd.rbCast();
                m_skill.rb();
            }else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Nuke") && cd.nuke)
            {
                cd.nukeCast();
                m_skill.nuke();
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("EllenGunShoot") && cd.guidedSpell)
            {
                cd.guidedSpellCast();
                m_skill.guidedTrigger = true;
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("IceSword") && cd.iceSword)
            {
                cd.iceSwordCast();
                m_skill.iceSwordTrigger = true;
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Ballista") && cd.ballista)
            {
                cd.ballistaCast();
                m_skill.ballista();

            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("cross-exec") && cd.cross)
            {
                cd.crossCast();
                m_skill.cross();

            }


            //blink skills
            if (Input.GetKeyUp(KeyCode.LeftShift) ) {
                RaycastHit hit;
                Vector3 forwardPos = transform.position + transform.forward;
                m_skill.teleport();
                /**
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

                
                **/

                /**
                rb.AddForce(transform.forward * 100f, ForceMode.Impulse);

                //teleport force added
                teleported = true;
                tpFrame = 30;

                **/




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




            CalculateForwardMovement();
            CalculateVerticalMovement();

            SetTargetRotation();

            
            UpdateOrientation();

            PlayAudio();

            TimeoutToIdle();

            m_PreviouslyGrounded = m_IsGrounded;

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
            m_PreviousCurrentStateInfo = m_CurrentStateInfo;
            m_PreviousNextStateInfo = m_NextStateInfo;
            m_PreviousIsAnimatorTransitioning = m_IsAnimatorTransitioning;

            m_CurrentStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            m_NextStateInfo = m_Animator.GetNextAnimatorStateInfo(0);
            m_IsAnimatorTransitioning = m_Animator.IsInTransition(0);
        }

        // Called after the animator state has been cached to determine whether this script should block user input.
        void UpdateInputBlocking()
        {
            bool inputBlocked = m_CurrentStateInfo.tagHash == m_HashBlockInput && !m_IsAnimatorTransitioning;
            inputBlocked |= m_NextStateInfo.tagHash == m_HashBlockInput;
            m_Input.playerControllerInputBlocked = inputBlocked;
        }

        // Called after the animator state has been cached to determine whether or not the staff should be active or not.
        bool IsWeaponEquiped()
        {
            bool equipped = m_NextStateInfo.shortNameHash == m_HashEllenCombo1 || m_CurrentStateInfo.shortNameHash == m_HashEllenCombo1;
            equipped |= m_NextStateInfo.shortNameHash == m_HashEllenCombo2 || m_CurrentStateInfo.shortNameHash == m_HashEllenCombo2;
            equipped |= m_NextStateInfo.shortNameHash == m_HashEllenCombo3 || m_CurrentStateInfo.shortNameHash == m_HashEllenCombo3;
            equipped |= m_NextStateInfo.shortNameHash == m_HashEllenCombo4 || m_CurrentStateInfo.shortNameHash == m_HashEllenCombo4;

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
        void CalculateForwardMovement()
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
        void CalculateVerticalMovement()
        {
            // If jump is not currently held and Ellen is on the ground then she is ready to jump.
            if (!m_Input.JumpInput && m_IsGrounded)
                m_ReadyToJump = true;

            if (m_IsGrounded)
            {
                // When grounded we apply a slight negative vertical speed to make Ellen "stick" to the ground.
                m_VerticalSpeed = -gravity * k_StickingGravityProportion;

                // If jump is held, Ellen is ready to jump and not currently in the middle of a melee combo...
                if (m_Input.JumpInput && m_ReadyToJump && !m_InCombo)
                {
                    // ... then override the previously set vertical speed and make sure she cannot jump again.
                    m_VerticalSpeed = jumpSpeed;
                    m_IsGrounded = false;
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

        // Called each physics step to set the rotation Ellen is aiming to have.
        void SetTargetRotation()
        {
            
        }

        

        // Called each physics step after SetTargetRotation if there is move input and Ellen is in the correct animator state according to IsOrientationUpdated.
        void UpdateOrientation()
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
        void PlayAudio()
        {
            float footfallCurve = m_Animator.GetFloat(m_HashFootFall);

            if (footfallCurve > 0.01f && !footstepPlayer.playing && footstepPlayer.canPlay)
            {
                footstepPlayer.playing = true;
                footstepPlayer.canPlay = false;
                footstepPlayer.PlayRandomClip(m_CurrentWalkingSurface, m_ForwardSpeed < 4 ? 0 : 1);
            }
            else if (footstepPlayer.playing)
            {
                footstepPlayer.playing = false;
            }
            else if (footfallCurve < 0.01f && !footstepPlayer.canPlay)
            {
                footstepPlayer.canPlay = true;
            }

            if (m_IsGrounded && !m_PreviouslyGrounded)
            {
                landingPlayer.PlayRandomClip(m_CurrentWalkingSurface, bankId: m_ForwardSpeed < 4 ? 0 : 1);
                emoteLandingPlayer.PlayRandomClip();
            }

            if (!m_IsGrounded && m_PreviouslyGrounded && m_VerticalSpeed > 0f)
            {
                emoteJumpPlayer.PlayRandomClip();
            }

            if (m_CurrentStateInfo.shortNameHash == m_HashHurt && m_PreviousCurrentStateInfo.shortNameHash != m_HashHurt)
            {
                hurtAudioPlayer.PlayRandomClip();
            }

            if (m_CurrentStateInfo.shortNameHash == m_HashEllenDeath && m_PreviousCurrentStateInfo.shortNameHash != m_HashEllenDeath)
            {
                emoteDeathPlayer.PlayRandomClip();
            }

            if (m_CurrentStateInfo.shortNameHash == m_HashEllenCombo1 && m_PreviousCurrentStateInfo.shortNameHash != m_HashEllenCombo1 ||
                m_CurrentStateInfo.shortNameHash == m_HashEllenCombo2 && m_PreviousCurrentStateInfo.shortNameHash != m_HashEllenCombo2 ||
                m_CurrentStateInfo.shortNameHash == m_HashEllenCombo3 && m_PreviousCurrentStateInfo.shortNameHash != m_HashEllenCombo3 ||
                m_CurrentStateInfo.shortNameHash == m_HashEllenCombo4 && m_PreviousCurrentStateInfo.shortNameHash != m_HashEllenCombo4)
            {
                emoteAttackPlayer.PlayRandomClip();
            }
        }

        // Called each physics step to count up to the point where Ellen considers a random idle.
        void TimeoutToIdle()
        {
            bool inputDetected = IsMoveInput || m_Input.Attack || m_Input.JumpInput || m_Input.RButton || m_Input.NukeButton || m_Input.GuidedSpell || m_Input.IceSword || m_Input.Ballista || m_Input.Cross;
            if (m_IsGrounded && !inputDetected)
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

            // If Ellen is on the ground...
            if (m_IsGrounded)
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
                // If not grounded the movement is just in the forward direction.
                movement = m_ForwardSpeed * transform.forward * Time.deltaTime;
            }

            // Rotate the transform of the character controller by the animation's root rotation.
            m_CharCtrl.transform.rotation *= m_Animator.deltaRotation;

            // Add to the movement with the calculated vertical speed.
            movement += m_VerticalSpeed * Vector3.up * Time.deltaTime;

            // Move the character controller.
            m_CharCtrl.Move(movement);

            // After the movement store whether or not the character controller is grounded.
            m_IsGrounded = m_CharCtrl.isGrounded;

            // If Ellen is not on the ground then send the vertical speed to the animator.
            // This is so the vertical speed is kept when landing so the correct landing animation is played.
            if (!m_IsGrounded)
                m_Animator.SetFloat(ParamAirVSpeed, m_VerticalSpeed);

            // Send whether or not Ellen is on the ground to the animator.
            m_Animator.SetBool(TriggerParamGround, m_IsGrounded);
        }
        
        // This is called by an animation event when Ellen swings her staff.
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
            StartCoroutine(RespawnRoutine());
        }
        
        protected IEnumerator RespawnRoutine()
        {
            // Wait for the animator to be transitioning from the EllenDeath state.
            while (m_CurrentStateInfo.shortNameHash != m_HashEllenDeath || !m_IsAnimatorTransitioning)
            {
                yield return null;
            }
            
            

            //teleport to spawn pos
            this.transform.position = respawnPos.position+ respawnPos.up*3f+ respawnPos.forward*3f;

            // Get Ellen's health back.
            m_Damageable.ResetDamage();

            // Set the Respawn parameter of the animator.
            m_Animator.SetTrigger(m_HashRespawn);
            
            
            
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
            m_Animator.SetTrigger(m_HashHurt);

            // Find the direction of the damage.
            Vector3 forward = damageMessage.damageSource - transform.position;
            forward.y = 0f;

            Vector3 localHurt = transform.InverseTransformDirection(forward);

            // Set the HurtFromX and HurtFromY parameters of the animator based on the direction of the damage.
            m_Animator.SetFloat(m_HashHurtFromX, localHurt.x);
            m_Animator.SetFloat(m_HashHurtFromY, localHurt.z);

            // Shake the camera.
            CameraShake.Shake(CameraShake.k_PlayerHitShakeAmount, CameraShake.k_PlayerHitShakeTime);

            // Play an audio clip of being hurt.
            if (hurtAudioPlayer != null)
            {
                hurtAudioPlayer.PlayRandomClip();
            }
        }

        // Called by OnReceiveMessage and by DeathVolumes in the scene.
        public void Die(Damageable.DamageMessage damageMessage)
        {
            m_Animator.SetTrigger(m_HashDeath);
            m_ForwardSpeed = 0f;
            m_VerticalSpeed = 0f;
            m_Respawning = true;
            m_Damageable.isInvulnerable = true;
        }
    }
}
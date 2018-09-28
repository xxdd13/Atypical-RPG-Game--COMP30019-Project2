using UnityEngine;
using System;
using System.Collections;
using Gamekit3D;


public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance
    {
        get { return s_Instance; }
    }

    protected static PlayerInput s_Instance;

    [HideInInspector]
    public bool playerControllerInputBlocked;

    protected Vector2 m_Movement;
    protected Vector2 m_Camera;
    protected bool m_Jump;
    protected bool m_Attack;
    protected bool m_Pause;
    protected bool m_ExternalInputBlocked;
    public bool RButton;
    public bool NukeButton;
    public bool GuidedSpell;
    public bool IceSword;
    public bool Ballista;


    public Vector2 MoveInput
    {
        get
        {
            if(playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Movement;
        }
    }

    public Vector2 CameraInput
    {
        get
        {
            if(playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Camera;
        }
    }

    public bool JumpInput
    {
        get { return m_Jump && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Attack
    {
        get { return m_Attack && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Pause
    {
        get { return m_Pause; }
    }

    WaitForSeconds m_AttackInputWait;
    WaitForSeconds m_RBInputWait;
    WaitForSeconds m_NukeInputWait;
    WaitForSeconds m_GuidedSpellInputWait;
    WaitForSeconds m_IceSwordInputWait;
    WaitForSeconds m_BallistaInputWait;

    Coroutine m_AttackWaitCoroutine;
    Coroutine m_RBWaitCoroutine;
    Coroutine m_NukeWaitCoroutine;
    Coroutine m_GuidedSpellWaitCoroutine;
    Coroutine m_IceSwordWaitCoroutine;
    Coroutine m_BallistaWaitCoroutine;

    const float k_AttackInputDuration = 0.03f;
    const float k_RBInputDuration = 0.03f;
    const float k_NukeInputDuration = 0.01f;
    const float k_GuidedSpellInputDuration = 0.01f;
    const float k_IceSwordInputDuration = 0.01f;
    const float k_BallistaInputDuration = 0.01f;

    void Awake()
    {
        m_AttackInputWait = new WaitForSeconds(k_AttackInputDuration);
        m_RBInputWait = new WaitForSeconds(k_RBInputDuration);
        m_NukeInputWait = new WaitForSeconds(k_NukeInputDuration);
        m_GuidedSpellInputWait = new WaitForSeconds(k_GuidedSpellInputDuration);
        m_IceSwordInputWait = new WaitForSeconds(k_IceSwordInputDuration);
        m_BallistaInputWait = new WaitForSeconds(k_BallistaInputDuration);

        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != this)
            throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");
    }


    void Update()
    {
        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_Camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_Jump = Input.GetButton("Jump");

        if (Input.GetButtonDown("Fire1"))
        {
            if (m_AttackWaitCoroutine != null)
                StopCoroutine(m_AttackWaitCoroutine);

            m_AttackWaitCoroutine = StartCoroutine(AttackWait());
        }

        if (Input.GetMouseButtonDown(1)) {
            if (m_RBWaitCoroutine != null)
                StopCoroutine(m_RBWaitCoroutine);

            m_RBWaitCoroutine = StartCoroutine(RBWait());
            Debug.Log("Pressed secondary button.");
        }
        if (Input.GetMouseButtonDown(2))
        {
            if (m_NukeWaitCoroutine != null)
                StopCoroutine(m_NukeWaitCoroutine);

            m_NukeWaitCoroutine = StartCoroutine(NukeWait());
            Debug.Log("Pressed middle button.");
        }

        //guidedSpell button q
        if (Input.GetKeyDown("q"))
        {
            if (m_GuidedSpellWaitCoroutine != null)
                StopCoroutine(m_GuidedSpellWaitCoroutine);

            m_GuidedSpellWaitCoroutine = StartCoroutine(GuidedSpellWait());
            Debug.Log("Pressed Q button.");
        }

        //icesword button e
        if (Input.GetKeyDown("e"))
        {
            if (m_IceSwordWaitCoroutine != null)
                StopCoroutine(m_IceSwordWaitCoroutine);

            m_IceSwordWaitCoroutine = StartCoroutine(IceSwordWait());
            Debug.Log("Pressed Q button.");
        }

        //ballista button r
        if (Input.GetKeyDown("r"))
        {
            if (m_BallistaWaitCoroutine != null)
                StopCoroutine(m_BallistaWaitCoroutine);

            m_BallistaWaitCoroutine = StartCoroutine(BallistaWait());
            Debug.Log("Pressed R button.");
        }

    }

    IEnumerator AttackWait()
    {
        m_Attack = true;

        yield return m_AttackInputWait;

        m_Attack = false;
    }
    IEnumerator RBWait()
    {
        RButton = true;

        yield return m_RBInputWait;

        RButton = false;
    }
    IEnumerator NukeWait()
    {
        NukeButton = true;

        yield return m_NukeInputWait;

        NukeButton = false;
    }
    IEnumerator GuidedSpellWait()
    {
        GuidedSpell = true;

        yield return m_GuidedSpellInputWait;

        GuidedSpell = false;
    }
    IEnumerator IceSwordWait()
    {
        IceSword = true;

        yield return m_IceSwordInputWait;

        IceSword = false;
    }
    IEnumerator BallistaWait()
    {
        Ballista = true;

        yield return m_BallistaInputWait;

        Ballista = false;
    }

    public bool HaveControl()
    {
        return !m_ExternalInputBlocked;
    }

    public void ReleaseControl()
    {
        m_ExternalInputBlocked = true;
    }

    public void GainControl()
    {
        m_ExternalInputBlocked = false;
    }
}

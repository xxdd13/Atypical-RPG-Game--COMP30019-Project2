using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Proj2.Message;
using UnityEngine.UI;


namespace Proj2
{
    public partial class Damageable : MonoBehaviour
    {

        public Slider healthSlider;  

        public int maxHitPoints;
        public float invulnerabiltyTime;




        public float hitForwardRotation = 360.0f;

        public bool isInvulnerable { get; set; }
        public int currentHitPoints;

        public UnityEvent OnDeath, OnHit, OnReset;

        
        public List<MonoBehaviour> onDamageMessageReceivers;

        protected float m_timeSinceLastHit = 0.0f;
        protected Collider m_Collider;

        System.Action myEvent;

        void Start()
        {
            ResetDamage();
            m_Collider = GetComponent<Collider>();
        }

        void OnEnable()
        {
            if (healthSlider != null)
                healthSlider.maxValue = this.maxHitPoints;
        }

        void Update()
        {
            if (isInvulnerable)
            {
                m_timeSinceLastHit += Time.deltaTime;
                if (m_timeSinceLastHit > invulnerabiltyTime)
                {
                    m_timeSinceLastHit = 0.0f;
                    isInvulnerable = false;
                }
            }
        }

        public void ResetDamage()
        {
            currentHitPoints = maxHitPoints;
            isInvulnerable = false;
            m_timeSinceLastHit = 0.0f;
            OnReset.Invoke();
        }

        public void SetColliderState(bool enabled)
        {
            m_Collider.enabled = enabled;
        }

        public void ApplyDamage(DamageMessage data)
        {
            if (currentHitPoints <= 0)
            {
                return;
            }

            if (isInvulnerable)
            {
                return;
            }



            isInvulnerable = true;
            currentHitPoints -= data.amount;

            if (currentHitPoints <= 0)
                myEvent += OnDeath.Invoke; 
            else
                OnHit.Invoke();

            var messageType = currentHitPoints <= 0 ? MessageType.DEAD : MessageType.DAMAGED;

            for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
            {
                var receiver = onDamageMessageReceivers[i] as IMessageReceiver;
                receiver.OnReceiveMessage(messageType, this, data);
            }
        }

        void LateUpdate()
        {
            //print(currentHitPoints);
            if (myEvent != null)
            {
                myEvent();
                myEvent = null;
            }

            //update enemy health ui slider
            if (healthSlider != null)
                healthSlider.value = this.currentHitPoints;
        }


    }

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Effects
{
    public class ThunderTime : MonoBehaviour
    {
        public static ThunderTime time;
        List<Timer> timers;
        List<int> clearOnHold;

        private int idCounter;

        class Timer
        {
            public int id;
            public bool active;

            public float dx, finalOne;
            public int ticks,ticksElapsed;
            public Action callBack;

            public Timer(int id_, float rate_, int ticks_, Action callback_)
            {
                id = id_;
                dx = rate_ < 0 ? 0 : rate_;
                ticks = ticks_ < 0 ? 0 : ticks_;
                callBack = callback_;
                finalOne = 0;
                ticksElapsed = 0;
                active = true;
            }

            public void Tick()
            {
                finalOne += Time.deltaTime;

                if (active && finalOne >= dx)
                {
                    finalOne = 0;
                    ticksElapsed++;
                    callBack.Invoke();

                    if (ticks > 0 && ticks == ticksElapsed)
                    {
                        active = false;
                        ThunderTime.time.RemoveTimer(id);
                    }
                }
            }
        }


        void Awake()
        {
            time = this;
            timers = new List<Timer>();
            clearOnHold = new List<int>();
        }


        public int AddTimer(float rate, Action callBack)
        {
            return AddTimer(rate, 0, callBack);
        }


        public int AddTimer(float rate, int ticks, Action callBack)
        {
            Timer newTimer = new Timer(++idCounter, rate, ticks, callBack);
            timers.Add(newTimer);
            return newTimer.id;
        }

      
        public void RemoveTimer(int timerId)
        {
            clearOnHold.Add(timerId);
        }


        void Remove()
        {
            if (clearOnHold.Count > 0)
            {
                foreach (int id in clearOnHold)
                    for (int i = 0; i < timers.Count; i++)
                        if (timers[i].id == id)
                        {
                            timers.RemoveAt(i);
                            break;
                        }

                clearOnHold.Clear();
            }
        }


        void Tick()
        {
            for (int i = 0; i < timers.Count; i++)
                timers[i].Tick();
        }

        void Update()
        {
            Remove();
            Tick();
        }
    }
}
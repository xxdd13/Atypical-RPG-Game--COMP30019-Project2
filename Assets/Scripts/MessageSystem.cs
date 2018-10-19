using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj2
{
    namespace Message
    {
        public enum MessageType
        {
            RESPAWN,
            DAMAGED,
            DEAD
            
        }

        public interface IMessageReceiver
        {
            void OnReceiveMessage(MessageType type, object sender, object msg);
        }
    } 
}

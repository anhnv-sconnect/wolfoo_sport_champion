using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN
{
    public class EventKey : IEventParams
    {
        public struct JumpPlayer : IEventParams
        {
        }
        public struct KickPlayer : IEventParams
        {
        }
    }
}
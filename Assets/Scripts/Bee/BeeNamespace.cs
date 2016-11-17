using UnityEngine;
using System.Collections;

namespace SteeringBehaviorsEnum
{
    public enum behavior_type
    {
        none               = 0x00000,
        seek               = 0x00002,
        flee               = 0x00004,
        arrive             = 0x00008,
        wander             = 0x00010,
        cohesion           = 0x00020,
        separation         = 0x00040,
        allignment         = 0x00080,
        obstacle_avoidance = 0x00100,
        wall_avoidance     = 0x00200,
        follow_path        = 0x00400,
        pursuit            = 0x00800,
        evade              = 0x01000,
        interpose          = 0x02000,
        hide               = 0x04000,
        flock              = 0x08000,
        offset_pursuit     = 0x10000,
    };

    enum Deceleration 
    { 
        slow = 3, normal = 2, fast = 1 
    };

    public struct Weight
    {
        public float seekWeight;
        public float fleeWeight;
        public float arriveWeight;
        public float pursuitWeight;
        public float evadeWeight;
        public float wanderWeight;  
    };

    public struct WanderParameter
    {
        //wander圈的大小
        public float WanderRadius;
        //wander突出在智能体前面的距离
        public float WanderDistance;
        //每秒加到目标随机位置的最大值
        public float WanderJitter;
    }
}



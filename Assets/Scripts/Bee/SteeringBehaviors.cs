using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SteeringBehaviorsEnum;
using UnityEngine.Assertions;



public class SteeringBehaviors : MonoBehaviour{

	public Vehicle agent { get; set; }
   

    //int flags = Convert.ToInt32("10000", 2);
    public int flags = 0x00800;
    public List<string> behaviorList=new List<string>();

	//权重
	float	seekWeight = 1f;
	float	fleeWeight = 1f;
    float   arriveWeight = 1f;
    float   pursuitWeight = 1f;
    float   evadeWeight = 1f;

    void Start()
    {
        //开关
        if (On(behavior_type.seek))
        {
            behaviorList.Add("seek");
        }
        if (On(behavior_type.flee))
        {
            behaviorList.Add("flee");
        }
        if (On(behavior_type.arrive))
        {
            behaviorList.Add("arrive");
        }
        if (On(behavior_type.pursuit))
        {
            behaviorList.Add("pursuit");
        }
        if (On(behavior_type.evade))
        {
            behaviorList.Add("evade");
        }
    }


	/// <summary>
	/// 计算合力
	/// </summary>
	public Vector2 Calculate()
	{

		Vector2 force = Vector2.zero;
		
		if (On(behavior_type.seek))
		{
			force += Seek( GameWorld.Instance.crossHair) * seekWeight;
		}

		if (On(behavior_type.flee))
		{
			force += Flee( GameWorld.Instance.crossHair ) * arriveWeight;
		}

		if (On(behavior_type.arrive))
		{
			force += Arrive( GameWorld.Instance.crossHair, Deceleration.slow ) * fleeWeight;
		}

        if (On(behavior_type.pursuit))
        {
            force += Pursuit( GameWorld.Instance.evade ) * pursuitWeight;
        }

        if (On(behavior_type.evade))
        {
            force += Evade(GameWorld.Instance.pursuit) * evadeWeight;
        }


		return force;
	}

    /// <summary>
    /// 判断是否在bt状态
    /// </summary>
    /// <param name="bt">状态.</param>
	bool On( behavior_type bt )
	{ 
		return ( flags & ((int)bt)) == (int)bt;
	}

	/// <summary>
	/// Seek 位置.
	/// </summary>
	/// <param name="targetPos">目标位置.</param>
	private Vector2 Seek(Vector2 targetPos)
	{
		Vector2 nowPos = new Vector2(this.transform.position.x,this.transform.position.y );
		Vector2 disiredVelocity = ( targetPos - nowPos ).normalized * agent.MaxSpeed();
		return disiredVelocity - agent.velocity;
	}

    /// <summary>
    /// Flee the specified targetPos.
    /// </summary>
    /// <param name="targetPos">Target position.</param>
	private Vector2 Flee(Vector2 targetPos)
	{
		Vector2 nowPos = new Vector2(this.transform.position.x,this.transform.position.y );
		Vector2 disiredVelocity = ( nowPos - targetPos ).normalized * agent.MaxSpeed();
		return disiredVelocity - agent.velocity;
	}




    /// <summary>
    /// Arrive the specified TargetPos .
    /// </summary>
    /// <param name="TargetPos">Target position.</param>
    /// <param name="deceleration">减速的模式</param>
	private Vector2 Arrive(Vector2 TargetPos,Deceleration deceleration)
	{
        //计算目标位置与当前位置的距离
		Vector2 nowPos = new Vector2(this.transform.position.x,this.transform.position.y );
		Vector2 ToTarget = TargetPos - nowPos;
		float dist = ToTarget.magnitude;


        //如果距离>0
		if (dist > 0)
		{
			//because Deceleration is enumerated as an int, this value is required
			//to provide fine tweaking of the deceleration..
			const float DecelerationTweaker = 0.3f;

			//calculate the speed required to reach the target given the desired
			//deceleration
			float speed =  dist / ((float)deceleration * DecelerationTweaker);     

			//确保不超速
			speed = Mathf.Min( speed, agent.MaxSpeed() );

			//from here proceed just like Seek except we don't need to normalize 
			//the ToTarget vector because we have already gone to the trouble
			//of calculating its length: dist. 
			Vector2 DesiredVelocity =  ToTarget * speed / dist;

			return ( DesiredVelocity - agent.velocity );
		}

		return new Vector2(0,0);
	}




    /// <summary>
    /// Pursuit the specified evader.
    /// </summary>
    /// <param name="evader">逃避者.</param>
    Vector2 Pursuit(GameObject evader)
    {
        VehicleTool agentVehicleToolScript = agent.GetComponent<VehicleTool>();
        VehicleTool evaderVehicleToolScript = evader.GetComponent<VehicleTool>();

        //如果逃避者在前面并且朝向智能体， 智能体科技靠近逃避着的当前状态
        Vector2 ToEvader = evader.transform.position - this.transform.position;
       
        double RelativeHeading = Vector2.Dot( agentVehicleToolScript.getForward() , evaderVehicleToolScript.getForward() );

        if ( Vector2.Dot(ToEvader , agentVehicleToolScript.getForward()) > 0 &&  
            (RelativeHeading < -0.95))  //acos(0.95)=18 degs
        {
            return Seek(evader.transform.position);
        }

        //不在前面额时候，我们预测逃避者的位置

        //预测的时间与 逃避者和追逐者的距离成正比
        //与智能体的速度和成反比

        float LookAheadTime = ToEvader.magnitude / 
            (agent.MaxSpeed() + evaderVehicleToolScript.getSpeed());

        //靠近预测的逃避者的位置
        return Seek(new Vector2(evader.transform.position.x,evader.transform.position.y) + agent.velocity * LookAheadTime);
       
    }


    Vector2 Evade( GameObject pursuer)
    {
        /* Not necessary to include the check for facing direction this time */

        //计算出来相差向量
        Vector2 ToPursuer = pursuer.transform.position - this.transform.position;

        //uncomment the following two lines to have Evade only consider pursuers 
        //within a 'threat range'

        //如果大于威胁距离，不逃跑
        const float ThreatRange = 100.0f;
        if (ToPursuer.sqrMagnitude > ThreatRange * ThreatRange) return new Vector2(0,0);

        //the lookahead time is propotional to the distance between the pursuer
        //and the pursuer; and is inversely proportional to the sum of the
        //agents' velocities
        Vehicle agentVehicleScript =agent.GetComponent<Vehicle>();
        VehicleTool pursuerVehicleToolScript = pursuer.GetComponent<VehicleTool>();
        float LookAheadTime = ToPursuer.magnitude / 
            (agentVehicleScript.MaxSpeed() +  pursuerVehicleToolScript.getSpeed());

        //now flee away from predicted future position of the pursuer
        Vehicle pursuerVehicleScript = pursuer.GetComponent<Vehicle>();
        Vector2 pursuerPos = new Vector2(pursuer.transform.position.x, pursuer.transform.position.y);
        return Flee(pursuerPos + pursuerVehicleScript.velocity * LookAheadTime);
    }

  
}

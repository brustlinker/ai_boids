using UnityEngine;
using System;
using System.Collections;
using SteeringBehaviorsEnum;



public class SteeringBehaviors : MonoBehaviour{

	public Vehicle agent { get; set; }
   

    int flags = Convert.ToInt32("10000", 2);


	//权重
	float	seekWeight = 1f;
	float	fleeWeight = 1f;
    float   arriveWeight = 1f;
    float   pursuitWeight = 1f;

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
            force += Pursuit( GameWorld.Instance.evader.transform ) * pursuitWeight;
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
    Vector2 Pursuit(Transform evader)
    {
        /*
        //如果逃避者在前面并且朝向智能体， 智能体科技靠近逃避着的当前状态
        Vector2 ToEvader = evader.position - this.transform.position;
       
        double RelativeHeading = this.transform.forward.Dot(evader.forward);

        if ( (ToEvader.Dot(this.transform.forward) > 0) &&  
            (RelativeHeading < -0.95))  //acos(0.95)=18 degs
        {
            return Seek(evader.position);
        }

        //不在前面额时候，我们预测逃避者的位置

        //预测的时间与 逃避者和追逐者的距离成正比
        //与智能体的速度和成反比
        Target targetScript = evader.GetComponent<Target>();
        double LookAheadTime = ToEvader.magnitude / 
            (agent.MaxSpeed + targetScript.Speed());

        //靠近预测的逃避者的位置
        return Seek(evader.position + targetScript.velocity * LookAheadTime);
        */
        return new Vector2(0, 0);
    }

  
}

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
    Weight weight;

    //wander参数
    private Vector2 wanderTarget;
    WanderParameter wanderParameter;
	float wanderTheta;

    void Start()
    {
        InitBehaviorList();
        InitWeight();
        InitWanderParameter();
    }

    void InitBehaviorList()
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
        if (On(behavior_type.wander))
        {
            behaviorList.Add("wander");
        }
    }

    void InitWeight()
    {
        weight.seekWeight = 1f;
        weight.fleeWeight = 1f;
        weight.arriveWeight = 1f;
        weight.pursuitWeight = 1f;
        weight.evadeWeight = 1f;
        weight.wanderWeight = 1f;
    }

    void InitWanderParameter()
    {
        wanderParameter.Distance = 2f;
        wanderParameter.Radius = 1.5f;
        wanderParameter.WanderJitter = 100f;
		wanderParameter.AngleChange = Mathf.PI;


        //stuff for the wander behavior

        wanderTheta = UnityEngine.Random.Range(0f,1f) * 2 * Mathf.PI;


            
    }



	/// <summary>
	/// 计算合力
	/// </summary>
	public Vector2 Calculate()
	{

		Vector2 force = Vector2.zero;
		
		if (On(behavior_type.seek))
		{
            force += Seek( GameWorld.Instance.crossHair) * weight.seekWeight;
		}

		if (On(behavior_type.flee))
		{
            force += Flee( GameWorld.Instance.crossHair ) * weight.fleeWeight;
		}

		if (On(behavior_type.arrive))
		{
            force += Arrive( GameWorld.Instance.crossHair, Deceleration.slow ) * weight.arriveWeight;
		}

        if (On(behavior_type.pursuit))
        {
            force += Pursuit( GameWorld.Instance.evade ) * weight.pursuitWeight;
        }

        if (On(behavior_type.evade))
        {
            force += Evade(GameWorld.Instance.pursuit) * weight.evadeWeight;
        }
        if (On(behavior_type.wander))
        {
			force += Wander() * weight.wanderWeight*20;
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


    /// <summary>
    /// Evade the specified pursuer.
    /// </summary>
    /// <param name="pursuer">Pursuer.</param>
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

        //预判时间与逃脱这与追击者的移动速度成反比
        //与两者的距离成正比
        Vehicle agentVehicleScript =agent.GetComponent<Vehicle>();
        VehicleTool pursuerVehicleToolScript = pursuer.GetComponent<VehicleTool>();
        float LookAheadTime = ToPursuer.magnitude / 
            (agentVehicleScript.MaxSpeed() +  pursuerVehicleToolScript.getSpeed());

        //从追击者的预判位置为目标进行flee（逃脱）
        Vehicle pursuerVehicleScript = pursuer.GetComponent<Vehicle>();
        Vector2 pursuerPos = new Vector2(pursuer.transform.position.x, pursuer.transform.position.y);
        return Flee(pursuerPos + pursuerVehicleScript.velocity * LookAheadTime);
    }

    /// <summary>
    /// Wander 游荡
    /// 但是目前产生的操控值有点问题，不在圆圈上.
    /// </summary>
    /// 
    
    Vector2 Wander()
    { 
		// Calculate the circle center
		Vector2 circleCenter = new Vector2(agent.velocity.x,agent.velocity.y);
		circleCenter.Normalize();
		circleCenter*=wanderParameter.Distance;

		// Calculate the displacement force
		Vector2 displacement = new Vector2(0, -1);
		displacement*=wanderParameter.Radius;

		//
		// Randomly change the vector direction
		// by making it change its current angle
		displacement=setAngle(displacement, wanderTheta);


		//
		// Change wanderAngle just a bit, so it
		// won't have the same value in the
		// next game frame.
		wanderTheta += UnityEngine.Random.Range(-1f,1f) * wanderParameter.AngleChange;


		// Finally calculate and return the wander force
		Vector2 wanderForce = circleCenter+displacement;

		return  wanderForce;

    }
    
	Vector2 setAngle( Vector2 vector,float angle) {
		var length= vector.magnitude;
		vector.x = Mathf.Cos(angle) * length;
		vector.y = Mathf.Sin(angle) * length;
		return vector;
	}



    Vector2 getWanderForce()
    {
        //这个东西与更新频率有关, so this line must
        //be included when using time independent framerate.
        float jitterThisTimeSlice = wanderParameter.WanderJitter * Time.deltaTime;

        //首先添加一个小随机
        Vector2 randomVector2 = new Vector2(UnityEngine.Random.Range(-1f,1f) * jitterThisTimeSlice,
            UnityEngine.Random.Range(-1f,1f) * jitterThisTimeSlice);

        //计算新的朝向点
        Vector2 newHeadingPoint=getHeadingCirclePoint()+randomVector2;

        //计算出投影到圆上的点
        Vector2 nowPos = new Vector2(transform.position.x,transform.position.y);
        Vector2 newHeadingPointFromCenter = newHeadingPoint - nowPos;
        Vector2 newHeadingCirclePoint = newHeadingPointFromCenter.normalized * wanderParameter.Radius;


		//计算绘制圆圈的中心偏移量
		//移动距离
		VehicleTool vehicleToolScript = this.GetComponent<VehicleTool>();
		Vector2 forward=vehicleToolScript.getForward();
		Vector2 offset = new Vector3(wanderParameter.Distance * forward.y,
			wanderParameter.Distance * forward.x);
		
		return newHeadingCirclePoint + offset;
    }


    /// <summary>
    /// 计算出朝向与操控圆圈的交点
    /// </summary>
    /// <returns>The heading circle point.</returns>
    Vector2 getHeadingCirclePoint()
    {
        //计算交点
        Vector2 nowPos = new Vector2(transform.position.x,transform.position.y);

        VehicleTool vehicleToolScript = this.GetComponent<VehicleTool>();
        Vector2 forward=vehicleToolScript.getForward();

        return new Vector2(nowPos.x + wanderParameter.Radius * forward.y, 
            nowPos.y + wanderParameter.Radius * forward.x);

    }

    void OnDrawGizmos()
    {

        // 设置颜色
        Gizmos.color = Color.green;

        //计算绘制圆圈的中心偏移量
        //移动距离
        VehicleTool vehicleToolScript = this.GetComponent<VehicleTool>();
        Vector2 forward=vehicleToolScript.getForward();
        Vector3 offset = new Vector3(wanderParameter.Distance * forward.y,
            wanderParameter.Distance * forward.x,0);
        
        // 绘制圆环
        Vector3 beginPoint =   transform.position;
        Vector3 firstPoint =   transform.position;

        //转一圈
        float m_Theta = 0.1f;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
        {
            //计算
            float x = wanderParameter.Radius * Mathf.Cos(theta);
            float y = wanderParameter.Radius * Mathf.Sin(theta);

            Vector3 endPoint = transform.position +offset + new Vector3(x , y, 0);
            if (theta == 0)
            {
                firstPoint = endPoint;
            }
            else
            {
                Gizmos.DrawLine(beginPoint, endPoint);
            }
            beginPoint = endPoint;
        }

        // 绘制最后一条线段
        Gizmos.DrawLine(firstPoint, beginPoint);

        //再话一条直线
		Vector2 wanderForce=Wander();
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(wanderForce.x,wanderForce.y,0));

    }

  

    Vector2 Side()
    {
        VehicleTool vehicleToolScript=this.GetComponent<VehicleTool>();
        Vector2 heading=vehicleToolScript.getForward();

        // ax+by=0;取a=1，b=-ax/y,就是b=-x/y

        //防止除0错误
        if (heading.y == 0)
        {
            return new Vector2(0, 1);
        }
        else
        {
            return new Vector2( 1, -heading.x / heading.y);
        }


    }

    Vector2 PointToWorldSpace(Vector2 targetLocal)
    {
        Vector3 ParentPos = this.transform.position;
        return new Vector2(targetLocal.x + ParentPos.x, targetLocal.y + ParentPos.y);
    }
  
}

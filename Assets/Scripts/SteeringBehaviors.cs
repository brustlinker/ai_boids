using UnityEngine;
using System.Collections;



public class SteeringBehaviors : MonoBehaviour{

	public Vehicle agent { get; set; }


	private  enum behavior_type
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

	int flags = (int) 5;


	//权重
	float	seekWeight = 1f;
	float	fleeWeight = 1f;

	/// <summary>
	/// 计算合力
	/// </summary>
	public Vector2 Calculate()
	{
		Vector2 force = Vector2.zero;

		/*
		if (On(behavior_type.seek))
		{
			force += Seek( GameWorld.Instance.crossHair) * seekWeight;
		}

		if (On(behavior_type.flee))
		{
			force += Flee( GameWorld.Instance.crossHair ) * fleeWeight;
		}

		if (On(behavior_type.arrive))
		{
			Debug.Log("dongshen");
			force += Arrive( GameWorld.Instance.crossHair, Deceleration.slow ) * fleeWeight;
		}
		*/
		force += Arrive( GameWorld.Instance.crossHair, Deceleration.slow ) * fleeWeight;


		return force;
	}

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

	private Vector2 Flee(Vector2 targetPos)
	{
		Vector2 nowPos = new Vector2(this.transform.position.x,this.transform.position.y );
		Vector2 disiredVelocity = ( nowPos - targetPos ).normalized * agent.MaxSpeed();
		return disiredVelocity - agent.velocity;
	}


	enum Deceleration 
	{ 
		slow = 3, normal = 2, fast = 1 
	};

	private Vector2 Arrive(Vector2     TargetPos,
		Deceleration deceleration)
	{
		Vector2 nowPos = new Vector2(this.transform.position.x,this.transform.position.y );

		Vector2 ToTarget = TargetPos - nowPos;

		//calculate the distance to the target
		float dist = ToTarget.magnitude;

		if (dist > 0)
		{
			//because Deceleration is enumerated as an int, this value is required
			//to provide fine tweaking of the deceleration..
			const float DecelerationTweaker = 0.3f;

			//calculate the speed required to reach the target given the desired
			//deceleration
			float speed =  dist / ((float)deceleration * DecelerationTweaker);     

			//make sure the velocity does not exceed the max
			speed = Mathf.Min( speed, agent.MaxSpeed() );

			//from here proceed just like Seek except we don't need to normalize 
			//the ToTarget vector because we have already gone to the trouble
			//of calculating its length: dist. 
			Vector2 DesiredVelocity =  ToTarget * speed / dist;

			return ( DesiredVelocity - agent.velocity );
		}

		return new Vector2(0,0);
	}


}

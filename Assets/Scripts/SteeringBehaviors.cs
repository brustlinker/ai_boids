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

	int flags = (int) behavior_type.flee;


	//权重
	float	seekWeight = 1f;
	float	fleeWeight = 1f;

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
			force += Flee( GameWorld.Instance.crossHair ) * fleeWeight;
		}
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



}

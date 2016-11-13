using UnityEngine;
using System.Collections;



public class SteeringBehaviors : MonoBehaviour{

	public Vehicle agent { get; set; }


	/// <summary>
	/// 计算合力
	/// </summary>
	public Vector2 Calculate()
	{
		Vector2 force = Vector2.zero;
		force+= Seek(new Vector2(5,2));
		return force;
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



}

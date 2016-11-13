using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour,MovingEntity {

	//描述自身的物理特性
	public Vector2 velocity { get; set; }
	private const float velocityMax=3;
	private const float mass=1;

	//操控
	private SteeringBehaviors steer;

	//世界


	// Use this for initialization
	void Start () {
		steer=this.GetComponent<SteeringBehaviors>();
		steer.agent=this;
	}
	
	// Update is called once per frame
	void Update () {
		//计算合力
		Vector2 steeringForce = steer.Calculate();

		//计算加速度
		Vector2 acceleration = steeringForce / mass;

		//计算速度
		velocity += acceleration*Time.deltaTime;

		//截断
		velocity=Truncate();

		//移动
		Vector3 velocity3 = new Vector3(velocity.x,velocity.y,0 );
		this.transform.position += velocity3*Time.deltaTime;

	}




	/// <summary>
	/// 如果速度大于最大速度，那么截断
	/// </summary>
	private Vector2 Truncate()
	{
		//利用速度的大小的平方来比较，省去了开平方根的消耗时间
		if(velocity.sqrMagnitude > 2 * velocityMax*velocityMax)
		{
			return velocity.normalized * velocityMax;
		}
		else
		{
			return velocity;
		}
	}


	//继承的几个方法
	public float MaxSpeed ()
	{
		return 10;
	}

	public void MaxForce ()
	{
		
	}

	public void MaxTurnRate()
	{
		
	}
}

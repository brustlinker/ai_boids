using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Vehicle : MonoBehaviour,MovingEntity {

	//描述自身的物理特性
	public Vector2 velocity { get; set; }
	private const float velocityMax=6f;
	private const float forceMax =24f;
	private const float mass=1;

	//操控
	private SteeringBehaviors steer;

	//测试用 
    public Text text;



	// Use this for initialization
	void Start () {
		steer=this.GetComponent<SteeringBehaviors>();
		steer.agent=this;

		//随机一个速度
		velocity = new Vector2(velocityMax,0f);

	}


	
	// Update is called once per frame
	void FixedUpdate () {

		//计算合力
		Vector2 steeringForce = steer.Calculate();

        //截断合力
		steeringForce=TruncateForce(steeringForce);

		//显示合力
		DisplayForce(steeringForce);

		Debug.Log(steeringForce.x+"  "+steeringForce.y);

		//计算加速度
		Vector2 acceleration = steeringForce / mass;

		//计算速度
        velocity += acceleration*Time.deltaTime;



		//截断
		velocity=TruncateSpeed();

		//移动
		Vector3 velocity3 = new Vector3(velocity.x,velocity.y,0 );
		this.transform.position += velocity3*Time.deltaTime;

        //把屏幕循环循环一下
        WrapAround();

        //更新朝向
        UpdateForward();
	}
        

	private Vector2 TruncateForce(Vector2 force)
	{
		//利用速度的大小的平方来比较，省去了开平方根的消耗时间
		if(force.sqrMagnitude > 2 * forceMax*forceMax)
		{
			return force.normalized * forceMax;
		}
		else
		{
			return force;
		}
	}

    /// <summary>
    /// 利用text显示合力的大小
    /// </summary>
    /// <param name="force">Force.</param>
    private void DisplayForce(Vector2 force)
    {
        text.text = force.x.ToString("F2") +":"+ force.y.ToString("F2");
    }


    /// <summary>
    /// 如果速度大于最大速度，那么截断
    /// </summary>
    private Vector2 TruncateSpeed()
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

    private void UpdateForward()
    {
        //计算出夹角(如果利用Atan只能处理1、4象限,而且不用处理除0错误)
        float radians = Mathf.Atan2( this.velocity.y , this.velocity.x );
        //转化为角度
        float degrees = radians * Mathf.Rad2Deg;
      
        //旋转
        this.transform.rotation=Quaternion.Euler(0,0,degrees);
    }


    //treat the screen as a toroid
    void WrapAround( )
    {
        Vector2 cameraSize = CameraTool.Instance.GetCameraSizeInUnit();
        //Rect screen=new Rect(Screen.width);
        Vector2 nowPos = new Vector2(transform.position.x,transform.position.y);
        if (nowPos.y > cameraSize.y / 2)
        {
            transform.position = new Vector3(nowPos.x, nowPos.y - cameraSize.y, 0);
        }
        else if(nowPos.y < - cameraSize.y / 2)
        {
            transform.position = new Vector3(nowPos.x, nowPos.y + cameraSize.y, 0);
        }

        if (nowPos.x > cameraSize.x / 2)
        {
            transform.position = new Vector3(nowPos.x-cameraSize.x, nowPos.y, 0);

        }
        else if(nowPos.x < -cameraSize.x / 2)
        {
            transform.position = new Vector3(nowPos.x+cameraSize.x, nowPos.y, 0);
      
        };

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

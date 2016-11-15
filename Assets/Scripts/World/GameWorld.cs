using UnityEngine;
using System.Collections;

public class GameWorld : MonoBehaviour {

	//单例模式
	private static 	GameWorld 	_instance;
	public 	static 	GameWorld 	Instance
	{
		get{return _instance;}
	}
	void Awake()
	{
		_instance = this;
	}



    //arrived 的目标
	public Vector2 crossHair { get; set; }
    public GameObject evade;
    public GameObject pursuit;

	// Use this for initialization
	void Start () {
		crossHair = new Vector2(5,2);
	}




   
}

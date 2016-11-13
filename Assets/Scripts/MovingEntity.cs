using UnityEngine;
using System.Collections;

public interface MovingEntity  {

	float MaxSpeed(); 
	
	void MaxForce ();

	void MaxTurnRate();

}

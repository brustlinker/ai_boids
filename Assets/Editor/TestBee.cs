﻿using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class TestBee {

    private float EPSINON = 0.001f;

	[Test]
	public void TestGetForward()
	{
		//Arrange
		var gameObject = new GameObject();
        gameObject.AddComponent<Vehicle>();

        Vehicle vehicle =gameObject.GetComponent<Vehicle>();
 
        TestGetForwardOn45(vehicle);

        TestGetForwardOn0(vehicle);

        TestGetForwardOnNegative45(vehicle);
  

  	}


    /// <summary>
    /// Tests the getforward  45度角.
    /// </summary>
    /// <param name="vehicle">Vehicle.</param>
    void TestGetForwardOn45(Vehicle vehicle)
    {
        vehicle.velocity = new Vector2(1,1);
        Assert.IsTrue(Mathf.Abs( vehicle.getForward().x-0.7071f) < EPSINON);
        Assert.IsTrue(Mathf.Abs( vehicle.getForward().y-0.7071f) < EPSINON);
    }

    /// <summary>
    /// Tests the get forward 0度.
    /// </summary>
    /// <param name="vehicle">Vehicle.</param>
    void TestGetForwardOn0(Vehicle vehicle)
    {
        vehicle.velocity = new Vector2(1,0);
        Assert.IsTrue(Mathf.Abs( vehicle.getForward().x-0f) < EPSINON);
        Assert.IsTrue(Mathf.Abs( vehicle.getForward().y-1f) < EPSINON);
    }

    /// <summary>
    /// Tests the get forward -45度.
    /// </summary>
    /// <param name="vehicle">Vehicle.</param>
    void TestGetForwardOnNegative45(Vehicle vehicle)
    {
        vehicle.velocity = new Vector2(-1,-1);
        Assert.IsTrue(Mathf.Abs( vehicle.getForward().x+0.7071f) < EPSINON);
        Assert.IsTrue(Mathf.Abs( vehicle.getForward().y+0.7071f) < EPSINON);
    }
}
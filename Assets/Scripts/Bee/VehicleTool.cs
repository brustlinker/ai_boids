using UnityEngine;
using System.Collections;

public class VehicleTool : MonoBehaviour {

    private Vehicle vehicleScript;

    private void Start()
    {
        vehicleScript = this.GetComponent<Vehicle>();
    }

    public float getSpeed()
    {
        return vehicleScript.velocity.magnitude;
    }

    /// <summary>
    /// 获取朝向
    /// </summary>
    /// <returns>朝向.</returns>
    public Vector2  getForward()
    {
        Vector2 velocity = vehicleScript.velocity;
        float radians = Mathf.Atan2( velocity.y , velocity.x );
        return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
    }

}

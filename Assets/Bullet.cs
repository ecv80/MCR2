using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public Vector3 dir=Vector3.zero;

    float speed=20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dT=Time.deltaTime;
        transform.position+=dir*dT*speed;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag!="OffLimits")
            return;
            
        // Destroy(gameObject);
        GameManager.getInstance().ParkBullet(gameObject);
    }
}

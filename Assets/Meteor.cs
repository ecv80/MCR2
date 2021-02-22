using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {
    //Initialization

    [SerializeField]
    public int hitsToBreak=0;
    [SerializeField]
    public int damage=0;

    float maxSpeed=5f;
    float minSpeed=1f;

    //State
    Vector3 dir=Vector2.zero;

    float spin=0f;

    int hits=0;

     // Start is called before the first frame update
    void Start()
    {
        //Determine direction and speed
        Vector3 target=new Vector2(Random.Range(-9.5f, 9.5f), -5f);

        dir=(target-transform.position).normalized*Random.Range(minSpeed, maxSpeed);

        spin=Random.Range(100f, 500f);
        spin=spin*(Random.value<.5f?1f:-1f);
    }

    // Update is called once per frame
    void Update()
    {
        float dT=Time.deltaTime;

        //Move!
        transform.position+=dir*dT;

        //Rotate
        transform.Rotate(Vector3.forward, spin*dT);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch(col.gameObject.tag) {
            case "OffLimits":
                 Destroy(gameObject);
                 break;
            case "Bullet":
                hits++;

                if (hits>=hitsToBreak)
                    Destroy(gameObject);
                break;
            default:
            break;
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    GameManager GM=null;

    // Start is called before the first frame update
    void Start()
    {
        GM=GameManager.getInstance();
        GM.IncreaseBuildingCount();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag!="Meteor")
            return;

        GM.RemoveTower(gameObject);
        
        GM.DecreaseBuildingCount();
            
        Destroy(gameObject);
    }
}

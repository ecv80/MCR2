using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour, IPointerDownHandler
{
    //Initialization

    [SerializeField]
    List<GameObject> meteors=null;
    [SerializeField]
    List<GameObject> towers=null;
    [SerializeField]
    GameObject bullet=null;
    [SerializeField]
    GameObject gameOverText=null;

    int maxMeteors=1; //How many meteors at the same time? Should increase over time for higher difficulty.

    float meteorInterval=1f; //Meteors will be fired every so many seconds if current meteors < maxMeteors

    Vector3 offScreenPos=new Vector3(1000f, 0f, 0f);

    //State
    static GameManager instance=null;

    int currentMeteors=0;

    int buildings=0;

    List<GameObject> BulletPool=new List<GameObject>();
    int availableBullets=10; //This counter is so that we won't have to waste time counting how many 
                             //bullets from the bullet pool are available each time we want to use one

    public int CurrentMeteors { get => currentMeteors; 
        set {
            currentMeteors=value>=0?value:0;
        }
    }
    
    public static GameManager getInstance() {
        if (instance==null)
            throw new System.Exception("Someone tried to access the Game Manager before it was initialized.");
        return instance;
    }

    void Awake() {
        instance=this;

        Physics.queriesHitTriggers=true; //Detect clicks on triggers too!

        adjustScreen();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<availableBullets; i++) { //We won't have more than 10 bullets on screen at a time
            GameObject go=Instantiate(bullet, offScreenPos, Quaternion.identity);
            go.SetActive(false);
            BulletPool.Add(go);
        }

        StartCoroutine(ThrowMeteor());
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    IEnumerator ThrowMeteor() {

        while (true) {

            if (meteors!=null && meteors.Count>0) {
                if (CurrentMeteors<maxMeteors) {
                    //What meteor?
                    int mIndex=Random.Range(0, meteors.Count);

                    GameObject m=Instantiate(meteors[mIndex]);
                    Meteor mScr=m.GetComponent<Meteor>();

                    m.transform.position=new Vector2(Random.Range(-9.5f, 9.5f), 6f);

                }
            }
            else
                Debug.Log("Meteors not initialized!");

            yield return new WaitForSeconds(meteorInterval);

        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (availableBullets<=0 || towers==null || towers.Count<=0)
            return;
        
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z=0f;

        Vector3 towerPos=towers[Random.Range(0, towers.Count)].transform.position;
        towerPos.x+=0.031f;
        towerPos.y+=0.782f;

        Vector3 dir=(worldPosition-towerPos).normalized;

        // GameObject b=Instantiate(bullet, towerPos, Quaternion.LookRotation(Vector3.forward, dir));
        foreach (GameObject go in BulletPool)
            if (go.activeSelf==false) { //This bullet is available for use
                availableBullets--;

                go.transform.position=towerPos;
                go.transform.rotation=Quaternion.LookRotation(Vector3.forward, dir);
                go.GetComponent<Bullet>().dir=dir;

                go.SetActive(true);

                break;
            }
        
    }

    public void ParkBullet (GameObject go) { //Parks a bullet away and makes it available for future use
        foreach (GameObject b in BulletPool)
            if (b==go) {
                b.SetActive(false);
                b.transform.position=offScreenPos;
                availableBullets++;

                break;
            }
    }

    public void RemoveTower(GameObject go) { //If the Game Object is really a tower, remove it from the list
        // for (int i=towers.Count-1; i>=0; i--)
        //     if (towers[i]==go)
        //         towers.RemoveAt(i);
        towers.Remove(go);
    }

    public void IncreaseBuildingCount() {
        buildings++;
    }

    public void DecreaseBuildingCount() {
        buildings--;

        //Did we lose yet?
        if (buildings<=0) { //We lost
            gameOverText.SetActive(true);
        }
    }

    void adjustScreen () {
        //In this game we're using a fixed aspect ratio of 16:9 (landscape) and we don't want
        //to stretch the view to fit each different device screen, because we want to preserve
        //the aspect ratio
        //If the device screen is wider (17:9, etc.), we scale the camera to fit the height
        //We don't care if there's excess width showing
        //If the device screen is shorter (16:10, etc.), we scale the camera to fit the width
        //Again we don't care if there's excess height showing

        Screen.orientation = ScreenOrientation.Landscape;

        float gameHeight=10f;
        float gameWidth=gameHeight*16f/9f;

        float unitsPerPixel;
        if (Screen.width/Screen.height>=gameWidth/gameHeight) { //Wider screen
            unitsPerPixel = gameHeight / Screen.height;
        }
        else { //narrower screen
            unitsPerPixel = gameWidth / Screen.width;
        }

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        Camera.main.orthographicSize = desiredHalfHeight;
    }
}

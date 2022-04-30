using UnityEngine;

public class MissileMechanics : MonoBehaviour {

    private float endY;
    private float RightendZ;
    private float LeftendZ;

    [SerializeField] private float time;

    //Target
    public GameObject RocketTargetPosition;
    public GameObject RightDoorTargetPosition;
    public GameObject LeftDoorTargetPosition;

    //Rocket
    public GameObject Rocket;
    private Vector3 Rocketpos;
    private float RocketstartY;

    //Stolbe 
    public GameObject Stolb;
    private Vector3 Stolbpos;
    private float StolbstartY;


    // Door
    public GameObject RightDoor;
    public GameObject LeftDoor;
    private Vector3 RightDoorPos;
    private Vector3 LeftDoorPos;
    private float RightDoorStartZ;
    private float LeftDoorStartZ;




    private float currentTime;

    void Start() {
        Setup();
        currentTime = 0f;
    }

    void Update() {


        currentTime += Time.deltaTime;
        if (currentTime < time) {
            OpenMissile();
        }
    }

    private void OpenMissile(){

        //Rocket
        Rocketpos.y = Mathf.Lerp(RocketstartY, endY, currentTime / time);
        Rocket.transform.position = Rocketpos;

        //St�lbe
        Stolbpos.y = Mathf.Lerp(StolbstartY, endY, currentTime / time);
        Stolb.transform.position = Stolbpos;

        //RightDoor
        RightDoorPos.z = Mathf.Lerp(RightDoorStartZ, RightendZ, currentTime / time);
        RightDoor.transform.position = RightDoorPos;

        //LeftDoor
        LeftDoorPos.z = Mathf.Lerp(LeftDoorStartZ, LeftendZ, currentTime / time);
        LeftDoor.transform.position = LeftDoorPos;

    }




    private void Setup() {
        // Stolbe Position
        Stolbpos = Stolb.transform.position;
        StolbstartY = Stolb.transform.position.y;

        //Rocket Position
        Rocketpos = Rocket.transform.position;
        RocketstartY = Rocket.transform.position.y;

        // RightDoor Position 
        RightDoorPos = RightDoor.transform.position;
        RightDoorStartZ = RightDoor.transform.position.z;

        // LeftDoor Position 
        LeftDoorPos = LeftDoor.transform.position;
        LeftDoorStartZ = LeftDoor.transform.position.z;

        // Rocket  target position
        endY = RocketTargetPosition.transform.position.y;

        // RightDoor Target Position 
        RightendZ = RightDoorTargetPosition.transform.position.z;

        //LeftDoor Target Position 
        LeftendZ = LeftDoorTargetPosition.transform.position.z;
    }
}
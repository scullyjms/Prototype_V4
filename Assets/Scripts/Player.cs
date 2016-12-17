using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public float moveSpeed = 0.046875f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallStickTime = 0.25f;


    public float timeToJumpApex = .5f;
    public float minJumpHeight = 5;
    public float maxJumpHeight = 10;
    public float wallSlideSpeedMax = 3;
    

    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float gameOverY = -6;

    float gravity;
    float velocityXSmoothing;
    float timeToWallUnstick;
    float maxJumpVelocity;
    float minJumpVelocity;
    float timeInAir;
    float treeWidth;

    float traverseStart;
    float traverseEnd;

    string verticalHit;
    string horizontalHit;

    bool grounded;
    bool grinding;
    bool traversing;
    bool wallSlideing;

    Vector3 velocity;
    Controller2D controller;
    BoxCollider2D collider;
    GameObject mainTree;
    BoxCollider2D trunkCollider;

    void Start() {
        controller = GetComponent<Controller2D>();
        collider = GetComponent<BoxCollider2D>();
        mainTree = GameObject.Find("Tree");
        trunkCollider = mainTree.GetComponent<BoxCollider2D>();

        wallSlideing = false;
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2); //enable if you want to control jump height
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;   //enable if you want to control jump height
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight); // enable if you want a min jump height
        print("Gravity: " + gravity + " Min Jump Velocity: " + minJumpVelocity + "  Jump Velocity: " + maxJumpVelocity);
    }

    void Update() {
        //GET INPUT AND FIND DIRECTION      
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));      //Get player input
        //int wallDirX = (controller.collisions.left) ? -1 : 1;                                            // Store the input direction
        int wallDirX = 1;
        input.x = 1;

        //WHERE WE GOING? BETTER SMOOTH IT OUT
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

//-------------------- STATES --------------------//
        //IF TOUCHING A WALL
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
            wallSlideing = true;
            velocity.y = 0;
            horizontalHit = controller.horizontalHitString;
            /*if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }*/

            if (timeToWallUnstick > 0) {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0) {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else {
                timeToWallUnstick = wallStickTime;
            }
        }

        //IF TRAVERSEING
        if (traversing) {
            trunkCollider.enabled = false;
            velocity.y = 0;
            if(collider.transform.position.x >= traverseEnd) {

                trunkCollider.enabled = true;
                Debug.Log("TRAVERSE ENDED");
                wallDirX = -1;
                input.x = -1;
                traversing = false;
            }
        }
        //DISABLE THE TREE COLLIDER
        //THEY SHOULD MOVE ON THE X QUICKLY AND STOP WHEN THEY REACH THE APROPE DISTANCE
        //GRAVITY SHOULDN'T HAVE AN AFFECT ON THE y

        //IF PLAYERS ON THE GROUND
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
            timeInAir = 0;
        }

//-------------------- KEY PRESSES --------------------//
        //IF THEY PRESS THE MOUSE BUTTON DOWN
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            //IF THEY ARE WALL SLIDEING
            if (wallSlideing) {
                if (wallDirX == input.x) {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }      
                else if (input.x == 0) {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            //IF THEY ARE ON THE GROUND 
            if (controller.collisions.below) {
                velocity.y = maxJumpVelocity;
            }
        }

        //IF THEY RELEASE THE MOUSE BUTTON
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            //AND THEY HAVEN'T JUMPED ENOUGH      
            if (velocity.y > minJumpVelocity) {
                velocity.y = minJumpVelocity;
            }
            //AND THEY ARE FALLING        
            if (Mathf.Sign(velocity.y) > 0) {
                velocity.y = 0;
            }
        }

        //IF THEY PRESS SPACE
        if (Input.GetKeyDown(KeyCode.Space) && wallSlideing){
            Debug.Log("SPACE");    
            //Debug.Log(controller.treeWidth + "test");
            traversing = true;
            wallSlideing = false;
            velocity.y = 0;
            traverseStart = collider.transform.position.x;
            traverseEnd = traverseStart + controller.treeWidth;
            //velocity.x = traverseEnd - traverseStart;
            Debug.Log("traverseStart: " + traverseStart);
            Debug.Log("traverseEnd: " + traverseEnd);
        }

//-------------------- UPDATE MOVMENTS --------------------//
        //APPLY GRAVITY
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        gameOver();
    }


//-------------------- METHODS --------------------//
    void gameOver() {
        // game over code
        if (transform.localPosition.y < gameOverY) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}


using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class debugText : MonoBehaviour {
 
    public Text distanceText;
    public Text FPSDisplay;
    public Text displayTime;

    float FPSCount;
 
    float frameCount = 0f;
    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.
    float timeCount = 0f;

    float distanceTravelled;
    int distance;
    float avgFrameRate;
    float deltaTime = 0f;

    void Update() {

        //how far has the player traveled?
        distanceTravelled = transform.localPosition.x;
        distanceText.text = "Distance: " + distanceTravelled.ToString("f0");

        //FPS Display
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / updateRate) {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }

        FPSDisplay.text = "FPS: " + fps.ToString("f0");

        //Display Time
        timeCount += Time.deltaTime;
        displayTime.text = "Time: " + timeCount.ToString("f0");

    }
}

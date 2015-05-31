using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

    public int pressingLeftIndex;
    public int pressingRightIndex;

    // touch screen bounds
    private float midX;
    private float maxX;
    private float maxY;

    private static InputController _instance;
    public static InputController Instance() {
        return _instance;
    }
	// Use this for initialization
	void Start () {
	    _instance = this;

        pressingLeftIndex = -1;
        pressingRightIndex = -1;

        midX = Screen.width / 2f;
        maxX = Screen.width;
        maxY = Screen.height;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnFingerDown(FingerDownEvent e) {
        if (e.Position.x < midX) // Left
        {
            pressingLeftIndex = e.Finger.Index;
        } 
        else if (e.Position.x < maxX) // Right
        {
            pressingRightIndex = e.Finger.Index;
        }
    }

    void OnFingerUp(FingerUpEvent e) {
        if (e.Finger.Index == pressingLeftIndex) // Left Finger is released
        {
            pressingLeftIndex = -1;
        } else if (e.Finger.Index == pressingRightIndex) // Right Finger
        {
            pressingRightIndex = -1;
        }
    }

    public bool GetBothPressed()
    {
        return ((pressingLeftIndex > -1) && (pressingRightIndex > -1)) 
            || 
            (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow));
    }

    public float GetHorizontalAxis()
    {
        if (pressingLeftIndex > -1 || Input.GetKey(KeyCode.LeftArrow))
        {
            return -1f;
        }
        else if (pressingRightIndex > -1 || Input.GetKey(KeyCode.RightArrow))
        {
            return 1f;
        }
        else
        {
            return 0.0f;
        }
    }
}

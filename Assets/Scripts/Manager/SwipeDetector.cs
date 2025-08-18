using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SwipeDirection
{
    Up,
    Down,
    Right,
    Left
}
public class SwipeDetector : MonoBehaviour
{
    
   // public static event System.Action<SwipeDirection> onSwipe;

    public static event System.Action<SwipeDirection, int> onSwipeMagnitud;
    // private bool swiping = false;
    // private bool eventSent = false;
    private Vector2 lastPosition;

    public float minSwipeDistY;

    public float minSwipeDistX;


    public float minXSwipPerc;

    public float sizeStep = 0;

    private Vector2 startPos;


    public bool isRunning = true;

    private void Start()
    {
        minSwipeDistX = Screen.width * minXSwipPerc;
        sizeStep = Screen.width / 2;
    }



    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Mouse0))
            startPos = Input.mousePosition;
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            float swipeDistHorizontal = (new Vector3(Input.mousePosition.x, 0, 0) - new Vector3(startPos.x, 0, 0)).magnitude;

          //  Debug.Log("swipe Dist Horizontal " + swipeDistHorizontal);
            if (swipeDistHorizontal > minSwipeDistX)

            {

                float swipeValue = Mathf.Sign(Input.mousePosition.x - startPos.x);

                if (swipeValue > 0)
                {                
                    int val = Mathf.CeilToInt( swipeDistHorizontal / sizeStep);
                    onSwipeMagnitud(SwipeDirection.Left, val);
                }
                else if (swipeValue < 0)
                {                  
                    int val = Mathf.CeilToInt(swipeDistHorizontal / sizeStep) ;
                    onSwipeMagnitud(SwipeDirection.Right, val);
                }


            }
        }
#endif     
        if (!isRunning) return;
        //#if UNITY_ANDROID
        if (Input.touchCount > 0)

        {
            Debug.Log("Hola");
            Touch touch = Input.touches[0];

            switch (touch.phase)

            {

                case TouchPhase.Began:

                    startPos = touch.position;

                    break;



                case TouchPhase.Ended:


                    float swipeDistHorizontal = (new Vector3(touch.position.x, 0, 0) - new Vector3(startPos.x, 0, 0)).magnitude;

                    Debug.Log("swipe Dist Horizontal " + swipeDistHorizontal);
                    if (swipeDistHorizontal > minSwipeDistX)

                    {

                        float swipeValue = Mathf.Sign(touch.position.x - startPos.x);

                        if (swipeValue > 0)
                        {
                            int val = Mathf.CeilToInt(swipeDistHorizontal / sizeStep);
                            onSwipeMagnitud(SwipeDirection.Left, val);
                        }
                        else if (swipeValue < 0)
                        {
                         
                            int val = Mathf.CeilToInt(swipeDistHorizontal / sizeStep);
                            onSwipeMagnitud(SwipeDirection.Right, val);
                        }


                    }
                    break;
            }
        }
    }
}


using UnityEngine;
using System.Collections;

public class SwipeController : MonoBehaviour {
	public float maxSwipeTime = 0.3f;
	public float minSwipeDist = 0.5f;
	public float Delay = 0.01f;

	private float TimeDelayed = 0;
	private bool isSwipe;
	private float fingerStartTime;
	private Vector2 fingerStartPos;
	//public PlayerController _playerControl;

	/// <summary>
	/// Occurs when on left swipe.
	/// </summary>
	public delegate void LeftSwipeAction();
	public static event LeftSwipeAction OnLeftSwipe;

	/// <summary>
	/// Occurs when on right swipe.
	/// </summary>
	public delegate void RightSwipeAction();
	public static event RightSwipeAction OnRightSwipe;

	/// <summary>
	/// Occurs when on up swipe.
	/// </summary>
	public delegate void UpSwipeAction();
	public static event UpSwipeAction OnUpSwipe;

	/// <summary>
	/// Occurs when on down swipe.
	/// </summary>
	public delegate void DownSwipeAction();
	public static event DownSwipeAction OnDownSwipe;


	public delegate void TapAction();
	public static event TapAction OnLeftTap;
	public static event TapAction OnRightTap;

	void Update(){
		if (Input.touchCount > 0){ //Detect touch when not in aiming mode
			
			foreach (Touch touch in Input.touches)
			{
				switch (touch.phase)
				{
					case TouchPhase.Began:
					/* this is a new touch */
						isSwipe = true;
						fingerStartTime = Time.time;
						fingerStartPos = touch.position;
						//ResetDirectionCount();
						break;	

				case TouchPhase.Ended:
					if (isSwipe) {
						if (touch.position.x <= 0) {
							OnLeftTap ();
						} else {
							OnRightTap ();
						}
					}
					break;

					case TouchPhase.Canceled:
						/* The touch is being canceled */
						isSwipe = false;
						break;
						
					case TouchPhase.Moved:
						
						float gestureTime = Time.time - fingerStartTime;
						float gestureDist = (touch.position - fingerStartPos).magnitude;
						
						if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist){
							
							Vector2 direction = touch.position - fingerStartPos;
							Vector2 swipeType = Vector2.zero;
							
							if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
								// the swipe is horizontal:
								swipeType = Vector2.right * Mathf.Sign(direction.x);
							}else{
								// the swipe is vertical:
								swipeType = Vector2.up * Mathf.Sign(direction.y);
							}
							if(swipeType.x != 0.0f){
								if(swipeType.x > 0.0f){
									// MOVE RIGHT
									OnRightSwipe ();
									isSwipe = false;
								}else{
								// MOVE LEFT
									OnLeftSwipe ();
									isSwipe = false;
								}
							}
							if(swipeType.y != 0.0f ){
								if(swipeType.y > 0.0f){
									// MOVE UP
									//OnUpSwipe ();
									isSwipe = false;
									//}
								}else{
									//OnDownSwipe ();
									isSwipe = false;
								}
							}
						}
						break;
				}
			}
		}
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		if(h > 0 && Time.time > TimeDelayed){
			OnRightTap ();
			TimeDelayed = Time.time + Delay;
		} else if (h<0 && Time.time > TimeDelayed){
			OnLeftTap ();
			TimeDelayed = Time.time + Delay;
		} else if (v >0 && Time.time > TimeDelayed){
			OnRightSwipe ();
			TimeDelayed = Time.time + Delay;
		}else if (v <0 && Time.time > TimeDelayed){
			OnRightSwipe ();
			TimeDelayed = Time.time + Delay;
		}

		if (Input.GetMouseButtonDown (0)) {
			float _x = Input.mousePosition.x;
			if (_x <= Camera.main.pixelWidth / 2) {
				OnLeftTap ();
			} else {
				OnRightTap ();
			}
		}


	}
}
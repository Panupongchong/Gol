using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour {

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

	private bool isDraging = false;
	private Vector2 startTouch, swipeDelta;
	private float half;

	void Start(){
		half = Screen.width / 2f;
	}

	private void Update()
	{

		#if UNITY_EDITOR
		#region Standalone Inputs
		if(Input.GetMouseButtonDown(0))
		{
			isDraging = true;
			startTouch = Input.mousePosition;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			swipeDelta = (Vector2) Input.mousePosition - startTouch;
			if(swipeDelta.magnitude < 100f){
				if(startTouch.x < half){
					OnLeftTap();
				} else {
					OnRightTap();
				}
			}
			isDraging = false;
			Reset();
		}

		#endregion

		#else

		#region Mobile Input

		if(Input.touches.Length > 0)
		{
			if(Input.touches[0].phase == TouchPhase.Began)
			{
				isDraging = true;
				startTouch = Input.touches[0].position;
			}
			else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
			{
				swipeDelta = Input.touches[0].position - startTouch;
				if(swipeDelta.magnitude < 20f){
					if(startTouch.x < half){
						OnLeftTap();
					} else {
						OnRightTap();
					}
				}
				isDraging = false;
				Reset();
			}
		}

		#endregion
		#endif
		// Calculate the distance

		swipeDelta = Vector2.zero;

		if(isDraging)
		{
			if(Input.touches.Length > 0)
			{
				swipeDelta = Input.touches[0].position - startTouch;
			}
			else if(Input.GetMouseButton(0))
			{
				swipeDelta = (Vector2) Input.mousePosition - startTouch;
			}
		}

		// Did we cross the deadzone ?
		if(swipeDelta.magnitude > 100f)
		{
			// Which direction ?
			float x = swipeDelta.x;
			float y = swipeDelta.y;

			if(Mathf.Abs(x) > Mathf.Abs(y))
			{
				// Left or right
				if(x < 0)
				{
					OnLeftSwipe ();
				}
				else
				{
					OnRightSwipe ();
				}
			}
			else
			{
				/*// Up or down
				if(y < 0)
				{
					//OnDownSwipe ();
				}
				else
				{
					//OnUpSwipe ();
				}*/
			}

			Reset();

		}

	}

	private void Reset()
	{
		startTouch = swipeDelta = Vector2.zero;
		isDraging = false;
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuizPanelController : MonoBehaviour {

	//Parameter
	private RectTransform background;
	private Image backgroundImage;
	private float blockSize =300;
	private float extraSize = 20;
	public float speed = 100f;
	private int blockCount = 0;
	private  List<BlockObjectController> m_leftBlock = new List<BlockObjectController> ();
	private  List<BlockObjectController> m_rightBlock = new List<BlockObjectController> ();
	private Coroutine moveDownTween = null;

	private bool last = false;

	void Awake(){
		background = GetComponent<RectTransform> ();
		backgroundImage = GetComponent<Image> ();
	}

	public void reset (){
		foreach (BlockObjectController left in m_leftBlock) {
			BlockObjectPoolController.Instance.returnBlock (left);
		}
		foreach (BlockObjectController right in m_rightBlock) {
			BlockObjectPoolController.Instance.returnBlock (right);
		}

		m_leftBlock.Clear ();
		m_rightBlock.Clear ();
		blockCount = 0;
	}

	void OnEnable(){
		last = false;
		backgroundImage.color = Color.gray;
	}

	public void addLine(BlockObjectController _left, BlockObjectController _right){
		m_leftBlock.Add (_left);
		m_rightBlock.Add (_right);
		blockCount++;
		updateSize ();
	}

	private void updateSize(){
		float bgHeight = m_leftBlock.Count * blockSize + (m_leftBlock.Count - 1) * extraSize;
		if (gameObject.activeSelf && background.sizeDelta.y != bgHeight) {
			StartCoroutine (scaleHeightTo (bgHeight));
		} else {
			Vector2 size = new Vector2(background.sizeDelta.x, bgHeight);
			background.sizeDelta = size;
		}
	}

	public bool playCorrect (int _side){
		switch (_side) {
		case 0:
			BlockObjectPoolController.Instance.returnBlock (m_rightBlock [0]);
			m_leftBlock [0].gameObject.SetActive (false);
			m_leftBlock [0].gameObject.transform.SetParent (BlockObjectPoolController.Instance.transform.parent);
			StartCoroutine (AdjustTransInTheEndOfFrame (m_leftBlock [0], m_leftBlock [0].transform.position));
			break;
		case 1:
			BlockObjectPoolController.Instance.returnBlock (m_leftBlock [0]);
			m_rightBlock[0].gameObject.SetActive(false);
			m_rightBlock [0].gameObject.transform.SetParent (BlockObjectPoolController.Instance.transform.parent);
			StartCoroutine(AdjustTransInTheEndOfFrame(m_rightBlock[0], m_rightBlock[0].transform.position));
			break;
		case 2:
			m_leftBlock[0].gameObject.SetActive(false);
			m_leftBlock[0].gameObject.transform.SetParent(BlockObjectPoolController.Instance.transform.parent);
			StartCoroutine(AdjustTransInTheEndOfFrame(m_leftBlock[0], m_leftBlock[0].transform.position));

			m_rightBlock[0].gameObject.SetActive(false);
			m_rightBlock[0].gameObject.transform.SetParent(BlockObjectPoolController.Instance.transform.parent);
			StartCoroutine(AdjustTransInTheEndOfFrame(m_rightBlock[0], m_rightBlock[0].transform.position));

			break;
		}
		blockCount--;
		m_leftBlock.RemoveAt (0);
		m_rightBlock.RemoveAt (0);
		updateSize ();
		if (blockCount == 0) {
			StartCoroutine (disableSelf(0.1f));	
			return true;
		} else {
			return false;
		}
	}

	private IEnumerator AdjustTransInTheEndOfFrame(BlockObjectController obj, Vector3 position) 
	{
		yield return new WaitForEndOfFrame();
		obj.transform.position = position;
		obj.gameObject.SetActive(true);
		obj.animateCorrect ();
	}

	private IEnumerator disableSelf(float _time){
		yield return new WaitForSeconds (_time);
		gameObject.SetActive (false);
	}

	public void playIncorrect (int _side){
		switch (_side) {
		case 0:
			m_leftBlock [0].animateIncorrect ();
			break;
		case 1:
			m_rightBlock [0].animateIncorrect ();
			break;
		case 2:
			m_leftBlock [0].animateIncorrect ();
			m_rightBlock [0].animateIncorrect ();
			break;
		}
	}

	public void animateActive(){
		m_leftBlock[0].animateActive ();
		m_rightBlock[0].animateActive ();

		if (gameObject.activeSelf) {
			StartCoroutine (changeColorTo (Color.red));
		} else {
			backgroundImage.color = Color.red;
		}
		last = true;
	}

	Vector3 _to;
	public void moveDown(float _y){
		if (moveDownTween != null) {
			StopCoroutine (moveDownTween);
		}
		moveDownTween = StartCoroutine (movePlayLineTo (_to.y - _y / (last ? 2f : 1f)));
	}

	public void moveTo(float _y){
		StartCoroutine (movePlayLineTo (_y));
	}

	private IEnumerator movePlayLineTo(float _toY){
		Vector3 _from = transform.localPosition;
		_to = _from;
		_to.y = _toY;
		//Debug.Log ("Moving from " + _from + " to " + _to + " [" + _toY + "]");
		float _time = Mathf.Abs (_toY - _from.y) / speed;
		float _t = 0;
		while (_t < _time){
			float ratio = _t/_time;
			transform.localPosition = Vector3.Lerp (_from, _to, ratio);
			_t += Time.deltaTime;
			yield return null;
		}
		transform.localPosition = _to;
		moveDownTween = null;
	}

	private IEnumerator scaleHeightTo(float _to){
		float _from = background.rect.height;
		float _time = 0.1f;
		float _t = 0;
		while (_t < _time){
			background.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, Mathf.Lerp (_from, _to, _t / _time));
			_t += Time.deltaTime;
			yield return null;
		}
		background.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, _to);
	}

	private IEnumerator changeColorTo(Color _to){
		Color _from = backgroundImage.color;
		float _time = 0.3f;
		float _t = 0;
		while (_t < _time){
			backgroundImage.color = Color.Lerp (_from, _to, _t / _time);
			_t += Time.deltaTime;
			yield return null;
		}
		backgroundImage.color = _to;
	}
}
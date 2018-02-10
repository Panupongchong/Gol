using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameplayViewController : BaseViewController {

    [SerializeField]
	List<Sprite> m_operatorSpriteList;

    [SerializeField]
    Image m_operatorRenderer;
	//Object list
	private List<QuizPanelController> m_panelList = new List<QuizPanelController> ();

	//Not used yet
	private List<Block> m_blockObjectModeTwoList;
	public int m_currentMode; // Current game mode

	private float m_blockPadding; //x
	private float m_halfScreenHeight;
	private float m_quizPadding = 300; //y
	private float m_quizSize = 350;
	private int _lineCount = 0;

	// Use this for initialization

	void Awake () {
		/*UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
		float ratio = (float)mRoot.activeHeight / Screen.height;
		m_blockPadding = Mathf.Ceil(Screen.width * ratio) / 4f;
		m_halfScreenHeight = Mathf.Ceil(Screen.height * ratio) / 2f;*/
	}

	public void reset(){
		foreach (QuizPanelController _panel in m_panelList) {
			_panel.reset ();
			_panel.gameObject.SetActive (false);
		}
		m_panelList.Clear ();
	}

	public void addQuiz (Quiz _quiz){
		List<Line> _lines = ((Mini1Quiz)_quiz).getLines ();
		Debug.Log ("Adding quiz " + _lines.Count + " lines");
		QuizPanelController _panel = QuizPanelPoolController.Instance.getQuizPanel ();
		int _count = 0;
		foreach (Line _line in _lines) {
			_count++;
			List<BlockObjectController> _leftList = new List<BlockObjectController> ();
			foreach (Block _left in _line.m_leftBlock) {
				//Debug.Log ("Left " + _left.getNumber () + " type " + _left.getType ().ToString ());
				BlockObjectController _newLeft = BlockObjectPoolController.Instance.getBlockObject ();
				_newLeft.transform.parent = _panel.transform;
				Vector3 _leftPos = Vector3.left * m_blockPadding;
				_leftPos.y = m_quizSize * (_count - (float)(_lines.Count + 1) / 2f);
				_newLeft.transform.localPosition = _leftPos;
				_newLeft.initialise (_left.getNumber (), _left.getType (), _left.getInverse ());
				_newLeft.gameObject.SetActive (true);
				_leftList.Add (_newLeft);
			}
			List<BlockObjectController> _rightList = new List<BlockObjectController> ();
			foreach (Block _right in _line.m_rightBlock) {
				//Debug.Log ("Right " + _right.getNumber());
				BlockObjectController _newRight = BlockObjectPoolController.Instance.getBlockObject ();
				Vector3 _rightPos = Vector3.right * m_blockPadding;
				_rightPos.y = m_quizSize * (_count - (float)(_lines.Count + 1) / 2f);
				_newRight.transform.parent = _panel.transform;
				_newRight.transform.localPosition = _rightPos;
				_newRight.initialise (_right.getNumber (), _right.getType (), _right.getInverse ());
				_newRight.gameObject.SetActive (true);
				_rightList.Add (_newRight);
			}
			_panel.addLine (_leftList, _rightList);
		}
		_lineCount += _count;
		if (_lineCount == 1) {
			_panel.animateActive ();
		}
		m_panelList.Add (_panel);
		float _location = m_quizPadding + m_quizSize * (_lineCount - _lines.Count + 1) + (m_quizSize / 2 * (_lines.Count - 1));
		_panel.transform.localPosition = Vector3.up * (_location + m_halfScreenHeight);
		_panel.gameObject.SetActive (true);
		_panel.moveTo (_location - m_halfScreenHeight);
	}

	public void playCorrect(int _side){
		bool _done = m_panelList [0].playCorrect (_side);
		if (_done) {
			m_panelList.RemoveAt (0);
		}
		if (m_panelList.Count > 0) {
			m_panelList [0].animateActive ();
			stepDown ();
		}
		_lineCount--;
	}

	public void playWrong(int _side){
		m_panelList [0].playIncorrect (_side);
	}

	public void showOperator(int _operator){
		m_operatorRenderer.sprite = m_operatorSpriteList[_operator];
	}

	private void stepDown(){
		foreach(QuizPanelController _panel in m_panelList){
			_panel.moveDown (m_quizSize);
		}
	}

	protected override void OnBackButton(){
		//show pause
		//unshow pause
	}
}
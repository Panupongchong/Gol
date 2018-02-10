using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameplayViewController : BaseViewController {

    [SerializeField]
	List<Sprite> m_operatorSpriteList;

    [SerializeField]
    Image m_operatorRenderer, timeFill;

	[SerializeField]
	TextMeshProUGUI Score, Combo;
	//Object list
	private List<QuizPanelController> m_panelList = new List<QuizPanelController> ();

	//Not used yet
	private List<Block> m_blockObjectModeTwoList;
	public int m_currentMode; // Current game mode

	private float m_blockPadding; //x
	private float m_halfScreenHeight;
	private float m_quizPadding = 100; //y
	private float m_quizSize = 350;
	private int _lineCount = 0;

	// Use this for initialization

	public void SetScore(int score){
		Score.text = score.ToString ();
	}

	public void SetCombo(int combo){
		Combo.text = combo.ToString ();
	}

	public void SetTimeFill(float fill){
		timeFill.fillAmount = fill;
	}

	void Awake () {
		/*UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
		float ratio = (float)mRoot.activeHeight / Screen.height;
		m_blockPadding = Mathf.Ceil(Screen.width * ratio) / 4f;
		m_halfScreenHeight = Mathf.Ceil(Screen.height * ratio) / 2f;*/

		//m_blockPadding = Screen.width / 4f;
		m_halfScreenHeight = Screen.currentResolution.height /2f;

	}

	public void reset(){
		foreach (QuizPanelController _panel in m_panelList) {
			_panel.reset ();
			_panel.gameObject.SetActive (false);
		}
		_lineCount = 0;
		m_panelList.Clear ();
	}

	public void addQuiz (Quiz _quiz){
		List<Line> _lines = ((Mini1Quiz)_quiz).getLines ();
		QuizPanelController _panel = QuizPanelPoolController.Instance.getQuizPanel ();
		int _count = 0;
		foreach (Line _line in _lines) {
			_count++;

			BlockObjectController _newLeft = null;
			BlockObjectController _newRight = null;

			_newLeft = _line.m_leftBlock.Count == 1 ? BlockObjectPoolController.Instance.getBlockObject () 
				: BlockObjectPoolController.Instance.getDuoBlockObject ();
			_newLeft.transform.SetParent (_panel.transform);
			int _blockCount = 0;
			foreach (Block _left in _line.m_leftBlock) {
				_newLeft.initialise (_left.getNumber (), _left.getType (), _left.getInverse (), _blockCount);
				_blockCount++;
			}
			_newLeft.gameObject.SetActive (true);

			_newRight = _line.m_rightBlock.Count == 1 ? BlockObjectPoolController.Instance.getBlockObject () 
				: BlockObjectPoolController.Instance.getDuoBlockObject ();
			_newRight.transform.SetParent (_panel.transform);
			_blockCount = 0;
			foreach (Block _right in _line.m_rightBlock) {
				_newRight.initialise (_right.getNumber (), _right.getType (), _right.getInverse (), _blockCount);
				_blockCount++;
			}
			_newRight.gameObject.SetActive (true);

			_panel.addLine (_newLeft, _newRight);
		}

		_lineCount += _count;
		m_panelList.Add (_panel);
		float _location = m_quizPadding + m_quizSize * (_lineCount - _lines.Count + 1) + (m_quizSize / 2 * (_lines.Count - 1));
		_panel.transform.localPosition = Vector3.up * (_location + m_halfScreenHeight);
		_panel.gameObject.SetActive (true);
		if (_lineCount - _count == 0) {
			_panel.animateActive ();
		}
		_panel.moveTo (_location - m_halfScreenHeight);
	}

	public void playCorrect(int _side){
		bool _done = m_panelList [0].playCorrect (_side);
		if (_done) {
			m_panelList.RemoveAt (0);
		}
		if (m_panelList.Count > 0) {
			stepDown ();
			m_panelList [0].animateActive ();
		}
		_lineCount--;
		SoundController.Instance.PlaySound ("Correct");
	}

	public void playWrong(int _side){
		m_panelList [0].playIncorrect (_side);
		SoundController.Instance.PlaySound ("Incorrect");
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
		SoundController.Instance.PlaySound ("Back");
		//show pause
		//unshow pause
	}
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Pool;

public class GameplayViewController : BaseViewController
{

	[SerializeField]
	List<Sprite> m_operatorSpriteList;

	[SerializeField]
	Image m_operatorRenderer, timeFill;

	[SerializeField]
	TextMeshProUGUI Score, Combo;
	//Object list
	private List<QuizPanelController> m_panelList = new List<QuizPanelController>();

	//Not used yet
	private List<Block> m_blockObjectModeTwoList;
	public int m_currentMode; // Current game mode

	private float m_blockPadding; //x
	private float m_halfScreenHeight;
	private float m_quizPadding = 100; //y
	private float m_quizSize = 350;
	private int _lineCount = 0;

	public QuizPanelController QuizPrefab;
	private ObjectPool<QuizPanelController> _quizObjectPool;

	public void SetScore(int score)
	{
		Score.text = score.ToString();
	}

	public void SetCombo(int combo)
	{
		Combo.text = combo.ToString();
	}

	public void SetTimeFill(float fill)
	{
		timeFill.fillAmount = fill;
	}

	void Awake()
	{
		/*UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
		float ratio = (float)mRoot.activeHeight / Screen.height;
		m_blockPadding = Mathf.Ceil(Screen.width * ratio) / 4f;
		m_halfScreenHeight = Mathf.Ceil(Screen.height * ratio) / 2f;*/

		//m_blockPadding = Screen.width / 4f;
		m_halfScreenHeight = Screen.currentResolution.height / 2f;
		_quizObjectPool = new(() => Instantiate(QuizPrefab, transform));
	}

	public void Reset()
	{
		foreach (QuizPanelController _panel in m_panelList)
		{
			_panel.Reset();
			_panel.gameObject.SetActive(false);
		}
		_lineCount = 0;
		m_panelList.Clear();
	}

	public void AddQuiz(Quiz _quiz)
	{
		List<Line> linesData = ((Mini1Quiz)_quiz).getLines();
		QuizPanelController _panel = _quizObjectPool.Get();
		int _count = 0;
		foreach (Line lineData in linesData)
		{
			_count++;
			_panel.AddLine(lineData);
		}

		_lineCount += _count;
		m_panelList.Add(_panel);
		float _location = m_quizPadding + m_quizSize * (_lineCount - linesData.Count + 1) + (m_quizSize / 2 * (linesData.Count - 1));
		_panel.transform.localPosition = Vector3.up * (_location + m_halfScreenHeight);
		_panel.gameObject.SetActive(true);
		if (_lineCount - _count == 0)
		{
			_panel.AnimateActive();
		}
		_panel.MoveTo(_location - m_halfScreenHeight);
	}

	public void PlayCorrect(int _side)
	{
		bool _done = m_panelList[0].PlayCorrect(_side);
		if (_done)
		{
			m_panelList.RemoveAt(0);
		}
		if (m_panelList.Count > 0)
		{
			StepDown();
			m_panelList[0].AnimateActive();
		}
		_lineCount--;
		SoundController.Instance.PlaySound("Correct");
	}

	public void PlayWrong(int _side)
	{
		m_panelList[0].PlayIncorrect(_side);
		SoundController.Instance.PlaySound("Incorrect");
	}

	public void ShowOperator(int _operator)
	{
		m_operatorRenderer.sprite = m_operatorSpriteList[_operator];
	}

	private void StepDown()
	{
		foreach (QuizPanelController _panel in m_panelList)
		{
			_panel.MoveDown(m_quizSize);
		}
	}

	protected override void OnBackButton()
	{
		SoundController.Instance.PlaySound("Back");
		//show pause
		//unshow pause
	}
}
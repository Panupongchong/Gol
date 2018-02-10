using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizPanelPoolController : MonoBehaviour {

	public static QuizPanelPoolController _instance;
	public static QuizPanelPoolController Instance
	{
		get {
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<QuizPanelPoolController>();
			}
			return _instance;
		}
	}

	public GameObject m_quizPrefab;
	private List<QuizPanelController> m_quizList = new List<QuizPanelController> ();
	private int m_index = 0;

	public QuizPanelController getQuizPanel(){
		if (m_quizList.Count == 0 || m_quizList [m_index].gameObject.activeSelf) { //Instantiate new object if none above is available
			QuizPanelController _obj = Instantiate(m_quizPrefab, transform).GetComponent<QuizPanelController> ();
			m_quizList.Insert (m_index, _obj);
		}
		int _index = m_index;
		nextIndex ();
		return m_quizList [_index];
	}

	private void nextIndex(){
		m_index++;
		if (m_index >= m_quizList.Count) {
			m_index = 0;
		}
	}
}

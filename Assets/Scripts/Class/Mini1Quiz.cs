using System;
using System.Collections.Generic;

public class Mini1Quiz : Quiz
{
	private List<Line> m_lineList;
	private bool m_more = true;

	public Mini1Quiz ()
	{
		m_lineList = new List<Line> ();
	}

	public void addLine(Line _line){
		m_lineList.Add (_line);
	}

	public override bool checkAnswer(int _answer){
		if (_answer == m_lineList [0].m_answer) {
			return true;
		} else
			return false;
	}

	public override bool next(){
		m_lineList.RemoveAt (0);
		if(m_lineList.Count <= 0){
			return true;
		} else {
			return false;
		}
	}

	public List<Line> getLines(){
		return m_lineList;
	}
}


using System.Collections;
using System.Collections.Generic;

public abstract class QuizFactory {
	protected int m_seed = 0;

	public void setSeed (int _seed){
		m_seed = _seed;
	}

	public abstract Quiz generateQuiz(int lv);
}

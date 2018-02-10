using System;
using System.Collections.Generic;

public abstract class Quiz
{
	public abstract bool checkAnswer (int _answer);
	public abstract bool next ();
}
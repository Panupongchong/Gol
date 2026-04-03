using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseGameplayController : MonoBehaviour
{
	protected int _score = 0;
	protected int _combo = 0;
	protected int _bonus = 0;

	protected int _hit = 0;
	protected int _miss = 0;
	protected float _time = 0;

	protected int _life = 0;
	public int _lv = 1;
	protected float _timeLeft = 0;
	protected int _mode = 1; //1-minigame1, 2-minigame2
	protected int _maxQuiz = 4;
	protected List<Quiz> _quizList;
	protected QuizFactory _factory;
	public GameplayViewController _view;

	protected void OnEnable()
	{
		SwipeController.OnLeftSwipe += OnSwipe;
		SwipeController.OnRightSwipe += OnSwipe;
		SwipeController.OnLeftTap += OnLeftTap;
		SwipeController.OnRightTap += OnRightTap;
		SwipeController.OnMidTap += OnMidTap;
	}

	protected void OnDisable()
	{
		SwipeController.OnLeftSwipe -= OnSwipe;
		SwipeController.OnRightSwipe -= OnSwipe;
		SwipeController.OnLeftTap -= OnLeftTap;
		SwipeController.OnRightTap -= OnRightTap;
		SwipeController.OnMidTap -= OnMidTap;
	}

	protected abstract void OnSwipe();
	protected abstract void OnRightTap();
	protected abstract void OnLeftTap();
	protected abstract void OnMidTap();
}
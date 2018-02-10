using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseGameplayController : MonoBehaviour {
	protected int score = 0;
	protected int life = 0;
	protected int lv = 4;
	protected float timeLeft = 0;
	protected int mode = 1; //1-minigame1, 2-minigame2
	protected int maxQuiz = 4;
	protected List<Quiz> quizList;
	protected QuizFactory m_factory;
	public GameplayViewController m_view;

	protected void OnEnable(){
		SwipeController.OnLeftSwipe += OnSwipe;
		SwipeController.OnRightSwipe += OnSwipe;
		SwipeController.OnLeftTap += OnLeftTap;
		SwipeController.OnRightTap += OnRightTap;
	}

	protected void OnDisable(){
		SwipeController.OnLeftSwipe -= OnSwipe;
		SwipeController.OnRightSwipe -= OnSwipe;
		SwipeController.OnLeftTap -= OnLeftTap;
		SwipeController.OnRightTap -= OnRightTap;
	}

	protected abstract void OnSwipe ();
	protected abstract void OnRightTap ();
	protected abstract void OnLeftTap ();
}
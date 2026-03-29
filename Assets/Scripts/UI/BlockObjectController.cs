using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class BaseBlockObject : MonoBehaviour
{
	public abstract void initialise(int number, Utility.PrimaryType primaryType, bool isInverse, int index = 0);
	public abstract void animateIdle();
	public abstract void animateActive();
	public abstract void animateCorrect();
	public abstract void animateIncorrect();
	public abstract bool IsDuo { get; }
}

public class BlockObjectController : BaseBlockObject
{
	private bool _isPlaying = false;
	private bool _isDisposing = false;
	// public List<GAF.Core.GAFMovieClip> _gaf = new List<GAF.Core.GAFMovieClip> (); //TODO: reintroduce
	private Dictionary<int, string> _label = new Dictionary<int, string>();
	private string _mode = "1";

	public override bool IsDuo => false; //_gaf.Count > 1;

	public override void initialise(int number, Utility.PrimaryType primaryType, bool isInverse, int index = 0)
{
	_label[index] = "MG" + _mode + "-" + primaryType + "-" + number.ToString("D2");

	_isDisposing = false;
	_isPlaying = false;
	//Debug.Log ("label is " + m_label);
	// if (_label.Count == _gaf.Count) { //TODO: reintroduce
	animateIdle();
	// }
}

public override void animateIdle()
{
	// int _count = 0;//TODO: reintroduce
	// foreach (GAF.Core.GAFMovieClip _gaf in _gaf) {
	// 	_gaf.setSequence (_label [_count] + "_0", true);
	// 	_count++;
	// }
}

public override void animateActive()
{
	// int _count = 0; //TODO: reintroduce
	// foreach (GAF.Core.GAFMovieClip _gaf in _gaf) {
	// 	_gaf.setSequence (_label [_count] + "_1", true);
	// 	_count++;
	// }
}

public override void animateCorrect()
{
	StartCoroutine(playCorrectAnimationGAF());
}

public override void animateIncorrect()
{
	StartCoroutine(playIncorrectAnimationGAF());
}

private IEnumerator playIncorrectAnimationGAF()
{
	if (!_isPlaying)
	{
		_isPlaying = true;
		int _count = 0;
		float _animationDuration = 0f;
		// foreach (GAF.Core.GAFMovieClip _gaf in _gaf) {//TODO: reintroduce
		// 	_gaf.setSequence (_label [_count] + "_3", true);
		// 	_animationDuration = Mathf.Max (_animationDuration, _gaf.duration ());
		// 	_count++;
		// }
		yield return new WaitForSeconds(_animationDuration);

		if (_isPlaying)
			animateActive();
		_isPlaying = false;
	}
}

private IEnumerator playCorrectAnimationGAF()
{
	if (!_isDisposing)
	{
		_isDisposing = true;
		int _count = 0;
		float _animationDuration = 0f;
		// foreach (GAF.Core.GAFMovieClip _gaf in _gaf) {//TODO: reintroduce
		// 	_gaf.setSequence (_label [_count] + "_2", true);
		// 	_animationDuration = Mathf.Max (_animationDuration, _gaf.duration ());
		// 	_count++;
		// }
		yield return new WaitForSeconds(_animationDuration);
		BlockObjectPoolController.Instance.returnBlock(this);
	}
}
}

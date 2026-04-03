using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;
using System;
using UnityEngine.AI;


public abstract class BaseBlockObject : MonoBehaviour
{
	public abstract void Initialise(int number, Utility.PrimaryType primaryType, bool isInverse);
	public abstract void AnimateIdle();
	public abstract void AnimateActive();
	public abstract void AnimateCorrect();
	public abstract void AnimateIncorrect();
	public abstract bool IsDuo { get; }
}

public class BlockObjectController : BaseBlockObject
{
	[SerializeField] private SkeletonGraphic _graphic;
	[SerializeField] private List<SkeletonDataAsset> _numbersAsset;
	private bool _isPlaying = false;
	private bool _isDisposing = false;
	// public List<GAF.Core.GAFMovieClip> _gaf = new List<GAF.Core.GAFMovieClip> (); //TODO: reintroduce
	private string _label;
	private string _mode = "1";

	public override bool IsDuo => false; //_gaf.Count > 1;

	public override void Initialise(int number, Utility.PrimaryType primaryType, bool isInverse)
	{
		_label = "MG" + _mode + "-" + primaryType + "-" + number.ToString("D2");

		_isDisposing = false;
		_isPlaying = false;

		_graphic.skeletonDataAsset = _numbersAsset[number];
		string skinKey = primaryType switch
		{
			Utility.PrimaryType.Num => "Number", //"NumberTH"
			Utility.PrimaryType.Inv => "Negative",
			Utility.PrimaryType.Wrd => "Word", //"WordTH"
			Utility.PrimaryType.Dic => "Dice",
			Utility.PrimaryType.Fiv => "FiveBar",
			Utility.PrimaryType.Rom => "Roman",
			Utility.PrimaryType.Clk => "Clock",
			_ => throw new NotImplementedException()
		};
		_graphic.Skeleton.SetSkin(skinKey);
		_graphic.Initialize(true);
		// _graphic.SetMaterialDirty();
		AnimateIdle();
		// }
	}

	public override void AnimateIdle()
	{
		// int _count = 0;//TODO: reintroduce
		// foreach (GAF.Core.GAFMovieClip _gaf in _gaf) {
		// 	_gaf.setSequence (_label [_count] + "_0", true);
		// 	_count++;
		// }
		_graphic.AnimationState.SetAnimation(0, "idle", false);
	}

	public override void AnimateActive()
	{
		// int _count = 0; //TODO: reintroduce
		// foreach (GAF.Core.GAFMovieClip _gaf in _gaf) {
		// 	_gaf.setSequence (_label [_count] + "_1", true);
		// 	_count++;
		// }
		_graphic.AnimationState.SetAnimation(0, "active", false);
	}

	public override void AnimateCorrect()
	{
		StartCoroutine(playCorrectAnimationGAF());
	}

	public override void AnimateIncorrect()
	{
		StartCoroutine(playIncorrectAnimationGAF());
	}

	private IEnumerator playIncorrectAnimationGAF()
	{
		if (!_isPlaying)
		{
			_isPlaying = true;
			_graphic.AnimationState.SetAnimation(0, "incorrect", false);
			float _animationDuration = _graphic.AnimationState.GetCurrent(0).Animation.Duration;
			yield return new WaitForSeconds(_animationDuration);

			if (_isPlaying)
				AnimateActive();
			_isPlaying = false;
		}
	}

	private IEnumerator playCorrectAnimationGAF()
	{
		if (!_isDisposing)
		{
			_isDisposing = true;
			_graphic.AnimationState.SetAnimation(0, "correct", false);
			float _animationDuration = _graphic.AnimationState.GetCurrent(0).Animation.Duration;
			yield return new WaitForSeconds(_animationDuration);
			BlockObjectPoolController.Instance.returnBlock(this);
		}
	}
}

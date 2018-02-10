using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockObjectController : MonoBehaviour {
	private bool isPlaying = false;
	private bool isDisposing = false;
	public List<GAF.Core.GAFMovieClip> m_gaf = new List<GAF.Core.GAFMovieClip> ();
	private Dictionary<int, string> m_label = new Dictionary<int, string> ();
	private string m_mode = "1";

	public bool isDuo {
		get {
			return m_gaf.Count > 1;
		}
	}

	public void initialise(int _number, Utility.PrimaryType _type, bool _inverse, int _index = 0){
		m_label [_index] = "MG" + m_mode + "-" + _type + "-" + _number.ToString ("D2");

		isDisposing = false;
		isPlaying = false;
		//Debug.Log ("label is " + m_label);
		if (m_label.Count == m_gaf.Count) {
			animateIdle ();
		}
	}

	public void animateIdle(){
		int _count = 0;
		foreach (GAF.Core.GAFMovieClip _gaf in m_gaf) {
			_gaf.setSequence (m_label [_count] + "_0", true);
			_count++;
		}
	}

	public void animateActive(){
		int _count = 0;
		foreach (GAF.Core.GAFMovieClip _gaf in m_gaf) {
			_gaf.setSequence (m_label [_count] + "_1", true);
			_count++;
		}
	}

	public void animateCorrect(){
		StartCoroutine (playCorrectAnimationGAF ());
	}

	public void animateIncorrect(){
		StartCoroutine (playIncorrectAnimationGAF ());
	}

	private IEnumerator playIncorrectAnimationGAF(){
		if (!isPlaying) {
			isPlaying = true;
			int _count = 0;
			float _animationDuration = 0f;
			foreach (GAF.Core.GAFMovieClip _gaf in m_gaf) {
				_gaf.setSequence (m_label [_count] + "_3", true);
				_animationDuration = Mathf.Max (_animationDuration, _gaf.duration ());
				_count++;
			}
			yield return new WaitForSeconds (_animationDuration);

			if (isPlaying)
				animateActive ();
			isPlaying = false;
		}
	}

	private IEnumerator playCorrectAnimationGAF(){
		if (!isDisposing) {
			isDisposing = true;
			int _count = 0;
			float _animationDuration = 0f;
			foreach (GAF.Core.GAFMovieClip _gaf in m_gaf) {
				_gaf.setSequence (m_label [_count] + "_2", true);
				_animationDuration = Mathf.Max (_animationDuration, _gaf.duration ());
				_count++;
			}
			yield return new WaitForSeconds (_animationDuration);
			BlockObjectPoolController.Instance.returnBlock (this);
		}
	}
}

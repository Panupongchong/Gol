using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Pool;
using DG.Tweening;

public struct SubQuiz
{
	public readonly List<BaseBlockObject> LeftBlock;
	public readonly List<BaseBlockObject> RightBlock;

	public SubQuiz(List<BaseBlockObject> leftBlocks, List<BaseBlockObject> rightBlocks)
	{
		LeftBlock = leftBlocks;
		RightBlock = rightBlocks;
	}

	public readonly void Reset()
	{
		LeftBlock.ForEach(BlockObjectPoolController.Instance.returnBlock);
		RightBlock.ForEach(BlockObjectPoolController.Instance.returnBlock);
		LeftBlock.Clear();
		RightBlock.Clear();
	}
}

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class QuizPanelController : MonoBehaviour
{
	public BaseBlockObject BlockPrefab;

	public Transform LeftContainer;
	public Transform RightContainer;

	//Parameter
	public float speed = 100f;

	private RectTransform _background;
	private Image _backgroundImage;
	private float _blockSize = 300;
	private float _extraSize = 20;
	private List<SubQuiz> _subQuizzes = new();
	private Coroutine _moveDownTween = null;

	private ObjectPool<BaseBlockObject> _blockObjectPool;
	private bool _last = false;
	private Vector3 _to;

	void Awake()
	{
		_background = GetComponent<RectTransform>();
		_backgroundImage = GetComponent<Image>();

		_blockObjectPool = new(() => Instantiate(BlockPrefab, transform));
	}

	public void Reset()
	{
		_subQuizzes.ForEach(x => x.Reset());
		_subQuizzes.Clear();
	}

	void OnEnable()
	{
		_last = false;
		_backgroundImage.color = Color.gray;
	}

	public void AddLine(Line lineData)
	{
		List<BaseBlockObject> leftBlock = new();
		List<BaseBlockObject> rightBlock = new();
		AddBlock(lineData.LeftBlock, LeftContainer, ref leftBlock);
		AddBlock(lineData.RightBlock, RightContainer, ref rightBlock);
		_subQuizzes.Add(new(leftBlock, rightBlock));
		UpdateSize();
	}

	private void AddBlock(List<Block> blocks, Transform parent, ref List<BaseBlockObject> trackingList)
	{
		foreach (Block block in blocks)
		{
			BaseBlockObject newBlock = _blockObjectPool.Get(); //TODO: check if duo, get 2 instead
			newBlock.transform.SetParent(parent);
			newBlock.Initialise(block.GetNumber(), block.GetBlockType(), block.GetInverse());
			newBlock.gameObject.SetActive(true);
			trackingList.Add(newBlock);
		}
	}

	private void UpdateSize()
	{
		float bgHeight = _subQuizzes.Count * _blockSize + (_subQuizzes.Count - 1) * _extraSize;
		if (gameObject.activeSelf && _background.sizeDelta.y != bgHeight)
		{
			StartCoroutine(ScaleHeightTo(bgHeight));
		}
		else
		{
			Vector2 size = new(_background.sizeDelta.x, bgHeight);
			_background.sizeDelta = size;
		}
	}

	public bool PlayCorrect(int side)
	{
		var left = _subQuizzes[0].LeftBlock;
		var right = _subQuizzes[0].RightBlock;
		switch (side)
		{
			case 0:
				StartCoroutine(AdjustTransInTheEndOfFrame(left));
				right.ForEach(BlockObjectPoolController.Instance.returnBlock);
				break;
			case 1:
				left.ForEach(BlockObjectPoolController.Instance.returnBlock);
				StartCoroutine(AdjustTransInTheEndOfFrame(right));
				break;
			case 2:
				StartCoroutine(AdjustTransInTheEndOfFrame(left));
				StartCoroutine(AdjustTransInTheEndOfFrame(right));
				break;
		}
		_subQuizzes.RemoveAt(0);
		UpdateSize();
		if (_subQuizzes.Count == 0)
		{
			StartCoroutine(DisableSelf(0.1f));
			return true;
		}
		else
		{
			return false;
		}
	}

	private IEnumerator AdjustTransInTheEndOfFrame(List<BaseBlockObject> blocks)
	{
		Vector3[] positions = new Vector3[blocks.Count];
		for (int i = 0; i < blocks.Count; i++)
			positions[i] = blocks[i].transform.position;
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < blocks.Count; i++)
		{
			blocks[i].transform.position = positions[i];
			blocks[i].gameObject.SetActive(true);
			blocks[i].AnimateCorrect();
		}
	}

	private IEnumerator DisableSelf(float _time)
	{
		yield return new WaitForSeconds(_time);
		gameObject.SetActive(false);
	}

	public void PlayIncorrect(int _side)
	{
		switch (_side)
		{
			case 0:
				_subQuizzes[0].LeftBlock.ForEach(x => x.AnimateIncorrect());
				break;
			case 1:
				_subQuizzes[0].RightBlock.ForEach(x => x.AnimateIncorrect());
				break;
			case 2:
				_subQuizzes[0].LeftBlock.ForEach(x => x.AnimateIncorrect());
				_subQuizzes[0].RightBlock.ForEach(x => x.AnimateIncorrect());
				break;
		}
	}

	public void AnimateActive()
	{
		_subQuizzes[0].LeftBlock.ForEach(x => x.AnimateActive());
		_subQuizzes[0].RightBlock.ForEach(x => x.AnimateActive());

		if (gameObject.activeSelf)
		{
			StartCoroutine(ChangeColorTo(Color.red));
		}
		else
		{
			_backgroundImage.color = Color.red;
		}
		_last = true;
	}

	public void MoveDown(float y)
	{
		transform.DOKill();
		transform.DOLocalMoveY(transform.localPosition.y - y / (_last ? 2f : 1f), Mathf.Abs(y - transform.localPosition.y) / speed);
	}

	public void MoveTo(float y)
	{
		transform.DOKill();
		transform.DOLocalMoveY(y, Mathf.Abs(y - transform.localPosition.y) / speed);
	}

	private IEnumerator ScaleHeightTo(float _to)
	{
		float _from = _background.rect.height;
		float _time = 0.1f;
		float _t = 0;
		while (_t < _time)
		{
			_background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(_from, _to, _t / _time));
			_t += Time.deltaTime;
			yield return null;
		}
		_background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _to);
	}

	private IEnumerator ChangeColorTo(Color _to)
	{
		Color _from = _backgroundImage.color;
		float _time = 0.3f;
		float _t = 0;
		while (_t < _time)
		{
			_backgroundImage.color = Color.Lerp(_from, _to, _t / _time);
			_t += Time.deltaTime;
			yield return null;
		}
		_backgroundImage.color = _to;
	}
}
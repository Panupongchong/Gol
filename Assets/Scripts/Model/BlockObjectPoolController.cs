using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObjectPoolController : MonoBehaviour
{

	public static BlockObjectPoolController _instance;
	public static BlockObjectPoolController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindAnyObjectByType<BlockObjectPoolController>();
			}
			return _instance;
		}
	}

	public BlockObjectController m_blockPrefab;
	public BlockObjectController m_blockDuoPrefab;
	private List<BaseBlockObject> m_blockList = new List<BaseBlockObject>();
	private List<BaseBlockObject> m_usedBlockList = new List<BaseBlockObject>();
	private List<BaseBlockObject> m_blockDuoList = new List<BaseBlockObject>();
	private List<BaseBlockObject> m_usedBlockDuoList = new List<BaseBlockObject>();

	public BaseBlockObject getBlockObject()
	{

		if (m_blockList.Count == 0)
		{ //Instantiate new object if none above is available
			BlockObjectController _obj = Instantiate(m_blockPrefab, transform);
			m_usedBlockList.Add(_obj);
			return _obj;
		}
		else
		{
			BaseBlockObject block = m_blockList[0];
			m_blockList.RemoveAt(0);
			m_usedBlockList.Add(block);
			return block;
		}
	}

	public BaseBlockObject getDuoBlockObject()
	{

		if (m_blockDuoList.Count == 0)
		{ //Instantiate new object if none above is available
			BlockObjectController _obj = Instantiate(m_blockDuoPrefab, transform);
			m_usedBlockDuoList.Add(_obj);
			return _obj;
		}
		else
		{
			BaseBlockObject block = m_blockDuoList[0];
			m_blockDuoList.RemoveAt(0);
			m_usedBlockDuoList.Add(block);
			return block;
		}
	}

	public void returnBlock(BaseBlockObject _block)
	{
		_block.gameObject.SetActive(false);
		_block.transform.SetParent(transform);
		if (!_block.IsDuo)
		{
			m_usedBlockList.Remove(_block);
			m_blockList.Add(_block);
		}
		else
		{
			m_usedBlockDuoList.Remove(_block);
			m_blockDuoList.Add(_block);
		}

	}
}

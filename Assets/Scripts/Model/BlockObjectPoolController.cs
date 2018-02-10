using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObjectPoolController : MonoBehaviour {

	public static BlockObjectPoolController _instance;
	public static BlockObjectPoolController Instance
	{
		get {
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<BlockObjectPoolController>();
			}
			return _instance;
		}
	}

	public GameObject m_blockPrefab;
	public GameObject m_blockDuoPrefab;
	private List<BlockObjectController> m_blockList = new List<BlockObjectController> ();
	private List<BlockObjectController> m_usedBlockList = new List<BlockObjectController> ();
	private List<BlockObjectController> m_blockDuoList = new List<BlockObjectController> ();
	private List<BlockObjectController> m_usedBlockDuoList = new List<BlockObjectController> ();

	public BlockObjectController getBlockObject(){

		if (m_blockList.Count == 0) { //Instantiate new object if none above is available
			BlockObjectController _obj = Instantiate (m_blockPrefab, transform).GetComponent<BlockObjectController> ();
			m_usedBlockList.Add (_obj);
			return _obj;
		} else {
			BlockObjectController block = m_blockList [0];
			m_blockList.RemoveAt (0);
			m_usedBlockList.Add (block);
			return block;
		}
	}

	public BlockObjectController getDuoBlockObject(){

		if (m_blockDuoList.Count == 0) { //Instantiate new object if none above is available
			BlockObjectController _obj = Instantiate (m_blockDuoPrefab, transform).GetComponent<BlockObjectController> ();
			m_usedBlockDuoList.Add (_obj);
			return _obj;
		} else {
			BlockObjectController block = m_blockDuoList [0];
			m_blockDuoList.RemoveAt (0);
			m_usedBlockDuoList.Add (block);
			return block;
		}
	}

	public void returnBlock(BlockObjectController _block){

		_block.gameObject.SetActive (false);
		_block.transform.SetParent (transform);
		if (!_block.isDuo) {
			m_usedBlockList.Remove (_block);
			m_blockList.Add (_block);
		} else {
			m_usedBlockDuoList.Remove (_block);
			m_blockDuoList.Add (_block);
		}
	
	}
}

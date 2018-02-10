using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Line {

	public List<Block> m_leftBlock;
	public List<Block> m_rightBlock;
	public int m_answer;

	int left = 0;
	int right = 0;

	public Line(){
		m_leftBlock = new List<Block> ();
		m_rightBlock = new List<Block> ();
	}

	public void addBlock(Block _block, int _side) //0 = left, 1 = right
	{
		if(_side == 0){
			m_leftBlock.Add(_block);
			left += _block.getInverse () ? -_block.getNumber () : _block.getNumber ();
		} else {
			m_rightBlock.Add(_block);
			right += _block.getInverse () ? -_block.getNumber () : _block.getNumber ();
		}
		m_answer = left > right ? 0 : (left < right ? 1 : 2);
	}
}

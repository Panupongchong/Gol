using UnityEngine;
using System.Collections;
using System;

public class Block {
	
	private int m_number;
	private Utility.PrimaryType m_type;
	private bool m_inverse;
	private bool m_mirror;

	public Block (int _number, Utility.PrimaryType _type, bool _mir){
		m_number = _number;
		m_type = _type;
		m_inverse = _type == Utility.PrimaryType.Inv;
		m_mirror = _mir;
	}

	public int getNumber(){ return m_number; }
	public Utility.PrimaryType getType(){ return m_type; }
	public bool getInverse(){ return m_inverse; }
}

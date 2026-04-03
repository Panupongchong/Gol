using System.Collections.Generic;

public class Line
{
	public List<Block> LeftBlock;
	public List<Block> RightBlock;
	public int Answer;

	private int left = 0;
	private int right = 0;

	public Line()
	{
		LeftBlock = new List<Block>();
		RightBlock = new List<Block>();
	}

	public void AddBlock(Block block, int side) //0 = left, 1 = right
	{
		if (side == 0)
		{
			LeftBlock.Add(block);
			left += block.GetInverse() ? -block.GetNumber() : block.GetNumber();
		}
		else
		{
			RightBlock.Add(block);
			right += block.GetInverse() ? -block.GetNumber() : block.GetNumber();
		}
		Answer = left > right ? 0 : (left < right ? 1 : 2);
	}
}

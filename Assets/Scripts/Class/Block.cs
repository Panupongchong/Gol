public class Block
{

	private int Number;
	private Utility.PrimaryType Type;
	private bool Inverse;
	private bool Mirror;

	public Block(int number, Utility.PrimaryType type, bool mir)
	{
		Number = number;
		Type = type;
		Inverse = type == Utility.PrimaryType.Inv;
		Mirror = mir;
	}

	public int GetNumber() { return Number; }
	public Utility.PrimaryType GetBlockType() { return Type; }
	public bool GetInverse() { return Inverse; }
}

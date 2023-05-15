public class Block
{
    public readonly Constants.Blocks type;
    private float durability;

    public Block(Constants.Blocks type, float durability)
    {
        this.type = type;
        this.durability = durability;
    }
}

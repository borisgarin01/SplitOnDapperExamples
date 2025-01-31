namespace SplitOn;
public sealed record StackSborka
{
    public int Prikaz { get; set; }
    public byte Num_List { get; set; }
    public PrikazItem PrikazForStackSborka { get; set; }
}

namespace AthelornTheSorceSmith.Assets.Scripts.Core.SaveSystem.Encoders
{
    public interface ISaveSystemEncoder
    {
        byte[] Encode(byte[] data);
        byte[] Decode(byte[] encodedData);
    }
}
namespace RequestBatcher.DTO
{
    public interface IExample
    {
        string Id { get; }
        int Payload { get; }
    }
    
    public class Example : IExample
    {
        public string Id { get; }
        public int Payload { get; }

        public Example(string id, int payload)
        {
            Id = id;
            Payload = payload;
        }
    }
}
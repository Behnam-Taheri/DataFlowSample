namespace DataFlow.Requests
{
    public record BufferBlockRequest
    {
        public BufferBlockRequest()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get;private set; }
    }
}

namespace Application.Common.Requests
{
    public class RequestResult<T> : RequestResult
    {
        public T Result { get; set; }
    }

    public class RequestResult : IRequestResult
    {
        public int StatusCodes { get; set; }

        public string Message { get; set; }

        public static RequestResult<T> Create<T>(T result)
        {
            return new RequestResult<T>
            {
                Result = result
            };
        }
    }
}

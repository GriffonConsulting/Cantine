namespace Application.Common.Requests
{
    public interface IRequestResult
    {
        int StatusCodes { get; set; }
        string Message { get; set; }
    }
}

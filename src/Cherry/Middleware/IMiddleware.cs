namespace Cherry
{
    public interface IMiddleware
    {
        Task HandleRequestAsync(
            HttpRequest req,
            HttpResponse res);
    }
}

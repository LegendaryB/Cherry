namespace Cherry.Middleware
{
    public interface IMiddleware
    {
        Task HandleRequestAsync(
            HttpRequest req,
            HttpResponse res);
    }
}

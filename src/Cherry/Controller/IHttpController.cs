namespace Cherry.Controller
{
    public interface IHttpController
    {
        Task PostAsync(HttpRequest req, HttpResponse res);
        Task PutAsync(HttpRequest req, HttpResponse res);
        Task GetAsync(HttpRequest req, HttpResponse res);
        Task DeleteAsync(HttpRequest req, HttpResponse res);
        Task HeadAsync(HttpRequest req, HttpResponse res);
        Task PatchAsync(HttpRequest req, HttpResponse res);
        Task OptionsAsync(HttpRequest req, HttpResponse res);
        Task TraceAsync(HttpRequest req, HttpResponse res);
    }
}
namespace tacttvm.Service
{
    public interface ITranslationService
    {
        public string UrlService { get; set; }
        public string GetText(string text);
    }
}

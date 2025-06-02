namespace ProniaBB209.Services
{
    public interface ILayoutService
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
    }
}

using Humanizer.Localisation;
using JuanApp.Data;
using JuanApp.Models;

namespace JuanApp.Services
{
    public class LayoutService(JuanDbContext juanDbContext)
    {
        public Dictionary<string, string> GetSettings()
        {
            return juanDbContext.Settings
                .ToDictionary(x => x.Key, x => x.Value);
        }
        public List<Category> GetCategories()
        {
            return juanDbContext.Categories.ToList();
        }
    }
}

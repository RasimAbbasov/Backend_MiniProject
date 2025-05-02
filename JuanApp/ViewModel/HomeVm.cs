using JuanApp.Models;

namespace JuanApp.ViewModel
{
    public class HomeVm
    {
        public List<Slider> Sliders { get; set; }
        public List<Feature> Features { get; set; }
        public List<Product> NewProducts { get; set; }
        public List<Product> TopProducts { get; set; }
    }
}

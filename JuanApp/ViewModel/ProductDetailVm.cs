using JuanApp.Models;

namespace JuanApp.ViewModel
{
    public class ProductDetailVm
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
    }
}

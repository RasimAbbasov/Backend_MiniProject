﻿namespace JuanApp.Models
{
    public class DbBasketItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int Count { get; set; }
    }
}

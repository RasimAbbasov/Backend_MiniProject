namespace JuanApp.Models
{
    public class BaseEntity
    {
        public virtual int Id { get; set; }
        public DateTime? CreateDate { get; set; }=DateTime.Now;
        public DateTime? UpdateDate { get; set; }
    }
}

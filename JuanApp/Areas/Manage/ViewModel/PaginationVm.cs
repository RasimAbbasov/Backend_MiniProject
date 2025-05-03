namespace JuanApp.Areas.Manage.ViewModel
{
        public class PaginationVm<T>
        {
            public int CurrentPage { get; set; }
            public int PageCount { get; set; }
            public bool HasNext { get; set; }
            public bool HasPrevious { get; set; }
            public List<T> Items { get; set; }

            public PaginationVm(List<T> items, int page, int pageCount)
            {
                Items = items;
                CurrentPage = page;
                PageCount = pageCount;
                HasNext = CurrentPage < PageCount;
                HasPrevious = CurrentPage > 1;

            }

            public static PaginationVm<T> Paginate(IQueryable<T> query, int page, int take)
            {
                var datas = query.Skip((page - 1) * take).Take(take).ToList();
                var pageCount = (int)Math.Ceiling((decimal)query.Count() / take);
                return new PaginationVm<T>(datas, page, pageCount);
            }
         }
}

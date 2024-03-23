namespace SV20T1020080.Web.Models
{/// <summary>
/// đầu vào tìm kiếm
/// </summary>
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;    

        public string SearchValue { get; set; } 
    }
    /// <summary>
    /// đầu vào cho tìm kiếm mặt hàng
    /// </summary>
    public class ProductSearchInput : PaginationSearchInput
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
    }

}

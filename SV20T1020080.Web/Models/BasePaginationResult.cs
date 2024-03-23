
using SV20T1020080.DomainModels;

namespace SV20T1020080.Web.Models
{
    /// <summary>
    /// lớp cha cho cắc lớp biểu diễn dữ liệu kết quả tìm kiếm phân trang
    /// </summary>
    public abstract class BasePaginationResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
        public int RowCount { get; set; }   

        public int PageCount
        {
            get
            {
                if(PageSize == 0)
                    return 1;
                int c=RowCount/PageSize;
                if (RowCount % PageSize > 0)
                    c += 1;
                return c;
            }
        }
    }
    /// <summary>
    /// keets quar timf kieems vaf laasy danh sachs khachs hangf
    /// </summary>
    public class CustomerSearchResult:BasePaginationResult
    {
        public List<Customer> DataCustomer { get; set;}
    }
    public class CategorySearchResult : BasePaginationResult
    {
        public List<Category> DataCategory { get; set; }
    }
    public class EmployeeSearchResult : BasePaginationResult
    {
        public List<Employee> DataEmployee { get; set; }
    }
    public class SupplierSearchResult : BasePaginationResult
    {
        public List<Supplier> DataSupplier { get; set; }
    }
    public class ShipperSearchResult : BasePaginationResult
    {
        public List<Shipper> DataShipper { get; set; }
    }
    public class ProductSearchResult : BasePaginationResult
    {
        public List<Product> DataProduct { get; set; }
    }
}

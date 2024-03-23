using SV20T1020080.DataLayers;
using SV20T1020080.DataLayers.SQLServer;
using SV20T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020080.BusinessLayers
{
/// <summary>
/// Cung cấp các chức năng xử lý dữ liệu chung
/// tỉnh/thành,khách hàng,nhà cung cấp,loại hàng,người giao hàng,nhân viên
/// </summary>
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Shipper>  shipperDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Category> categoryDB;
        /// <summary>
        /// ctor câu hỏi :static contrustor hoạt động như nào?
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;
            provinceDB = new ProvinceDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
        }
        public static  List<Province> ListofProvince()
        {
            return provinceDB.List().ToList();
        }
        /// <summary>
        /// tìm kiếm thông tin nhà cung cấp
        /// </summary>
        /// <param name="rowcount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowcount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowcount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }
        
        /// <summary>
        /// lấy thông tin nhà cung cấp theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        /// <summary>
        /// thêm nhà cung cấp
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier supplier)
        {
            return supplierDB.Add(supplier);

        }
        /// <summary>
        /// xóa nahf cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.IsUsed(id))
            {
                return false;
            }
            return supplierDB.Delete(id);
        }
        /// <summary>
        /// cập nhật nahf cung cấp
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier supplier)
        {
            return supplierDB.Update(supplier);
        }
        /// <summary>
        /// kiểm tra nahf cung cấp có đang sử dụng k
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedsupplier(int id)
        {
            return supplierDB.IsUsed(id);
        }




        /// <summary>
        /// tìm kiếm và lấy danh sách khách hàng
        /// </summary>
        /// <param name="rowcount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowcount ,int page=1,int pageSize=0,string searchValue="")
        {
            rowcount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// lấy thoonbg tin khách hàng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// thêm khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer customer)
        {
            return customerDB.Add(customer);

        }
        /// <summary>
        /// xóa khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if(customerDB.IsUsed(id))
            {
                return false;
            }
            return customerDB.Delete(id);
        }
        /// <summary>
        /// cập nhật khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer customer)
        {
            return customerDB.Update(customer);
        }
        /// <summary>
        /// kiểm tra khách hàng có đang sử dụng k
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCustomer(int id)
        {
            return customerDB.IsUsed(id);
        }



        /// <summary>
        /// tìm kiếm và lấy danh sách người giao hàng
        /// </summary>
        /// <param name="rowcount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShipper(out int rowcount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowcount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// lấy thông  tin người giao  hàng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        /// <summary>
        /// thêm khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper shipper)
        {
            return shipperDB.Add(shipper);

        }
        /// <summary>
        /// xóa khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            if (shipperDB.IsUsed(id))
            {
                return false;
            }
            return shipperDB.Delete(id);
        }
        /// <summary>
        /// cập nhật người giao hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper shipper)
        {
            return shipperDB.Update(shipper);
        }
        /// <summary>
        /// kiểm tra người giao hàng có đang sử dụng k
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedShipper(int id)
        {
            return shipperDB.IsUsed(id);
        }



        /// <summary>
        /// tìm kiếm và lấy danh sách nhân viên
        /// </summary>
        /// <param name="rowcount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowcount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowcount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// lấy thoonbg tin nahan viên theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        /// <summary>
        /// thêmnahan viên
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee employee)
        {
            return employeeDB.Add(employee);

        }
        /// <summary>
        /// xóa nahan viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.IsUsed(id))
            {
                return false;
            }
            return employeeDB.Delete(id);
        }
        /// <summary>
        /// cập nhật nhân viên
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee employee)
        {
            return employeeDB.Update(employee);
        }
        /// <summary>
        /// kiểm tra khách hàng có đang sử dụng k
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedEmployee(int id)
        {
            return employeeDB.IsUsed(id);
        }



        /// <summary>
        /// tìm kiếm và lấy danh sách loại hàng
        /// </summary>
        /// <param name="rowcount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowcount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowcount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// lấy thoonbg tin loại hàng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// thêm loại hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static int AddCategory(Category category)
        {
            return categoryDB.Add(category);

        }
        /// <summary>
        /// xóaloại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            
            return categoryDB.Delete(id);
        }
        /// <summary>
        /// cập nhật loại hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category category)
        {
            return categoryDB.Update(category);
        }
        /// <summary>
        /// kiểm tra khách hàng có đang sử dụng k
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCategory(int id)
        {
            return categoryDB.IsUsed(id);
        }
    }
}

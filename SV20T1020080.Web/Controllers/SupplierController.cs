using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020080.BusinessLayers;
using SV20T1020080.DomainModels;
using SV20T1020080.Web.AppCodes;
using SV20T1020080.Web.Models;

namespace SV20T1020080.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Employee},{WebUserRoles.Administrator}")]
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string SUPPLIER_SEARCH = "supplier_search";//Tên biến dùng để luu trong session
        public IActionResult Index()
        {
            // lấy đầu vào tìm kiếm hiện đang lưu lại trong sesion
            PaginationSearchInput input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);
            // trường hợp trong session chưa có điều kiện thì tạo điều kiện mới
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SupplierSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                DataSupplier = data
            };
            // lưu lại điều kiện  tìm kiếm vào trong session
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            Supplier model = new Supplier()
            {
                SupplierID = 0,
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            Supplier? model = CommonDataService.GetSupplier(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetSupplier(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.AllowDelete = !CommonDataService.IsUsedsupplier(id);
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Supplier data)
        {
            try
            {
                //kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState(nếu có)
                if (string.IsNullOrWhiteSpace(data.SupplierName))
                    ModelState.AddModelError("SupplierName", "Tên nhà cung cấp không được để trống");

                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError("ContactName", "Tên giao dịch không được để trống");

                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError("Email", "Vui lòng nhập  Email của khách hàng");

                if (string.IsNullOrWhiteSpace(data.Provice))
                    ModelState.AddModelError("Provice", "Vui lòng chọn tỉnh thành");
                // thong qua thuộc tính Isvalid của modelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";

                    return View("Edit", data);
                }
                if (data.SupplierID == 0)
                {
                    int id = CommonDataService.AddSupplier(data);
                }
                else
                {
                    bool result = CommonDataService.UpdateSupplier(data);
                   
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }
    }
}

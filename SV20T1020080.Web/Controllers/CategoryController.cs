using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020080.BusinessLayers;
using SV20T1020080.DomainModels;
using SV20T1020080.Web.AppCodes;
using SV20T1020080.Web.Models;
using System.Drawing.Printing;

namespace SV20T1020080.Web.Controllers
{
    [Authorize (Roles =$"{WebUserRoles.Employee},{WebUserRoles.Administrator}")]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string CATEGORY_SEARCH = "category_search";//Tên biến dùng để luu trong session
        public IActionResult Index()
        {
            // lấy đầu vào tìm kiếm hiện đang lưu lại trong sesion
            PaginationSearchInput input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
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
            var data = CommonDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CategorySearchResult ()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                DataCategory= data
            };
            // lưu lại điều kiện  tìm kiếm vào trong session
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            Category model = new Category()
            {
                CategoryID = 0,
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            Category? model = CommonDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCategory(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.AllowDelete = !CommonDataService.IsUsedCategory(id);
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Category data)
        {
            try
            {
                //kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState(nếu có)
                if (string.IsNullOrWhiteSpace(data.CategoryName))
                    ModelState.AddModelError("CategoryName", "Tên loại hàng không được để trống");

                if (string.IsNullOrWhiteSpace(data.Description))
                    ModelState.AddModelError("Description", "Vui lòng nhập mô tả");

                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng";

                    return View("Edit", data);
                }

                if (data.CategoryID == 0)
                {
                    int id = CommonDataService.AddCategory(data);
                }
                else
                {
                    bool result = CommonDataService.UpdateCategory(data);
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

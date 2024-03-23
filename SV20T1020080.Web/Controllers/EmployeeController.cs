using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020080.BusinessLayers;
using SV20T1020080.DomainModels;
using SV20T1020080.Web.AppCodes;
using SV20T1020080.Web.Models;

namespace SV20T1020080.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string EMPLOYEE_SEARCH = "employee_search";//Tên biến dùng để luu trong session
        public IActionResult Index()
        {
            // lấy đầu vào tìm kiếm hiện đang lưu lại trong sesion
            PaginationSearchInput input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
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
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                DataEmployee = data
            };
            // lưu lại điều kiện  tìm kiếm vào trong session
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            Employee model = new Employee()
            {
                EmployeeID = 0,
                BirthDate=new DateTime(1990,1,1),
                Photo= "nophoto.jpg"
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            Employee? model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            if(string.IsNullOrEmpty(model.Photo))
            {
                model.Photo = "nophoto.jpg";
            }
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.AllowDelete = !CommonDataService.IsUsedEmployee(id);
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Employee data,string birthDateInput,IFormFile uploadPhoto)
        {
            //kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState(nếu có)
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError("FullName", "Họ tên không được để trống");

            
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError("Email", "Vui lòng nhập  Email của khách hàng");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

                return View("Edit", data);
            }
            // Xuwr ly ngay
            DateTime? birthDate= birthDateInput.ToDateTime();
            if (birthDate.HasValue)
                data.BirthDate = birthDate.Value;
            // xu ly anh upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho employee
            if (uploadPhoto != null)
            {
                string fileName=$"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";//tên file sẽ luu
                string folder =Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"image\employees");//đường dẫn đến thư mục lưu file
                string filePath=Path.Combine(folder, fileName); //đường dẫn đén file cần lưu D:\\images\employees\phôt.png
                using (var stream=new FileStream(filePath,FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (data.EmployeeID == 0)
            {
                int id = CommonDataService.AddEmployee(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result=CommonDataService.UpdateEmployee(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng với khách hàng khác");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }
    }
}

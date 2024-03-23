using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020080.BusinessLayers;
using SV20T1020080.DomainModels;
using SV20T1020080.Web.AppCodes;
using SV20T1020080.Web.Models;
using System.Reflection;

namespace SV20T1020080.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Employee},{WebUserRoles.Administrator}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH = "product_search";//Tên biến dùng để luu trong session
        public IActionResult Index()
        {
            ProductSearchResult input = ApplicationContext.GetSessionData<ProductSearchResult>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchResult()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "",input.CategoryID, input.SupplierID);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                DataProduct = data
            };
            // lưu lại điều kiện  tìm kiếm vào trong session
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            Product model = new Product()
            {
                ProductID = 0,
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            ViewBag.IsEdit = true;
            Product? model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.AllowDelete = !ProductDataService.InUsedProduct(id);
            return View(model);
        }
        public IActionResult Save(Product data, IFormFile uploadPhoto)
        {
            //kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState(nếu có)
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError("ProductName", "Tên mặt hàng không được để trống");

            if (string.IsNullOrWhiteSpace(data.CategoryID.ToString()))
                ModelState.AddModelError("CategoryID", "Vui lòng chọn loại hàng");

            if (string.IsNullOrWhiteSpace(data.SupplierID.ToString()))
                ModelState.AddModelError("SupplierID", "Vui lòng chọn nhà cung cấp");

            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError("Unit", "Vui lòng nhập đơn vị tính");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";

                return View("Edit", data);
            }
            // xu ly anh upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho employee
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";//tên file sẽ luu
                string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"image\products");//đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); //đường dẫn đén file cần lưu D:\\images\employees\phôt.png
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (data.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(data);
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(data);
            }
            return RedirectToAction("Index");
        }


        public IActionResult Photo(int id, string method, int photoId = 0)
        {
            
            switch (method)
            {
                case "add":
                    {
                        ViewBag.Title = "Bỏ sung ảnh";
                        ProductPhoto model = new ProductPhoto()
                        {
                            PhotoID = 0,
                            ProductID = id
                        };
                        return View(model);
                    }
                case "edit":
                    {
                        ViewBag.Title = "Thay đổi ảnh";
                        ProductPhoto? model = ProductDataService.GetPhoto(photoId);
                        
                        if (model == null)
                            return RedirectToAction("Index");
                        return View(model);
                    }
                    
                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    //Todo:Xóa ảnh trực tiếp k cần confirm
                    return RedirectToAction("Edit", new { id = id });
                default: return RedirectToAction("Index");
            }
        }
        public IActionResult SavePhoto(ProductPhoto data, IFormFile uploadPhoto)
        {
            if (uploadPhoto != null)
            {
                string filename = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"image/products");
                string filePath = Path.Combine(folder, filename);// Đương dẫn đến file cần lưu

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = filename;
            }
            if (string.IsNullOrWhiteSpace(data.Photo))
            {
                ModelState.AddModelError("Photo", "Ảnh không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.Description))
            {
                ModelState.AddModelError("Description", "Mô tả không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự không được để trống");
            }
            //Xử lý ảnh upload
           
            if (!ModelState.IsValid)
            {
                return View("Photo", data);
            }
            if (data.PhotoID == 0)
            {
                ProductDataService.AddPhoto(data);
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            else
            {
                ProductDataService.UpdatePhoto(data);
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
        }
        public IActionResult Attribute(int id, string method, int attributeId = 0)
        {
            
            switch (method)
            {
                case "add":
                    {
                        ViewBag.Title = "Bổ sung thuộc tính";
                        ProductAttribute model = new ProductAttribute()
                        {
                            AttributeID = 0,
                            ProductID= id
                        };
                        return View(model);
                    }
                case "edit":
                    {
                        ViewBag.Title = "Thay đổi thuộc tính";
                        ProductAttribute? model = ProductDataService.GetAttribute(attributeId);
                        if (model == null)
                            return RedirectToAction("Index");
                        return View(model);
                    }

                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    //Todo:Xóa ảnh trực tiếp k cần confirm
                    return RedirectToAction("Edit", new { id = id });
                default: return RedirectToAction("Index");
            }
        }
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
            {
                ModelState.AddModelError("AttributeName", "Tên thuộc tính không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
            {
                ModelState.AddModelError("AttributeValue", "Giá trị không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự không được để trống");
            }

            if (!ModelState.IsValid)
            {

                return View("Attribute", data);
            }
            if (data.AttributeID == 0)
            {
                long id = ProductDataService.AddAttribute(data);
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(data);
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
        
           
        }
    }
}

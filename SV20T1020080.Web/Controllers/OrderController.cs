using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SV20T1020080.BusinessLayers;
using SV20T1020080.DomainModels;
using SV20T1020080.Web.AppCodes;
using SV20T1020080.Web.Models;

namespace SV20T1020080.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Employee}")]
    public class OrderController : Controller
    {
       
        //Số dòng trên 1 trang khi hiển thị danh sách đơn hàng
        private const int ORDER_PAGE_SIZE = 20;
        //Tên biến session để lưu điều kiện tìm kiếm đơn hàng
        private const string ORDER_SEARCH = "order_search";

        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH = "product_search_for_sale";
        private const string SHOPPING_CART = "shopping_cart";
        public IActionResult Index()
        {
            OrderSearchInput? input = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH);
            if (input == null)
            {
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = ORDER_PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    DateRange = string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}",
                                                DateTime.Today.AddMonths(-1),
                                                DateTime.Today)
                };
            }
            
            return View(input);
        }
        /// <summary>
        /// Thực hiện chức năng tìm kiếm đơn hàng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult Search(OrderSearchInput input)
        {
            int rowCount = 0;
            var data = OrderDataService.ListOrders(out rowCount, input.Page, input.PageSize,
                                                   input.Status, input.FromTime, input.ToTime, input.SearchValue ?? "");
            var model = new OrderSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Status = input.Status,
                TimeRange = input.DateRange ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH, input);
            return View(model);
        }


        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details,
            };
            return View(model);
        }
        /// <summary>
        /// chuyển đơn hàng sang trạng thái đã được duyệt 
        /// </summary>
        /// <param name="id">MÃ đơn hàng</param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);
            if (!result)
            {
                TempData["Message"] = "Khong the duyet don hang nay";
            }
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Chuyển đơn hàng sang trạng thái kết thúc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if (!result)
                TempData["Message"] = "Khong the ghi nhan trang thai ket thuc cho don hang nay";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Chuyển đơn hàng sang trại thái hủy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
                TempData["Message"] = "Khong the thuc hien thao tac huy doi voi don hang nay";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// chuyển đơn hàng sang trạng thái bị từ chối
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id);
            if (!result)
                TempData["Message"] = "Khong the thuc hien thao tac tu choi doi voi don hang nay";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// xóa đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể xóa đơn hàng này";
                return RedirectToAction("Details", new { id });
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// xóa mặt hàng ra khỏi đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IActionResult DeleteDetail(int id = 0, int productId = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productId);
            if (!result)
                TempData["Message"] = "Không thể xóa mặt hàng này ra khỏi đơn hàng";

            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// giao diện để sửa đổi thông tin mặt hàng được bán trong đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);
            return View(model);
        }
        public IActionResult UpdateDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
                return Json("Số lượng bán không hợp lệ");
            if (salePrice < 0)
                return Json("gia ban khong hop le");
            bool result = OrderDataService.SaveOrderDetail(orderID, productID, quantity, salePrice);
            if (!result)
                return Json("Không được thay đổi thông tin mặt hàng");

            var model = OrderDataService.GetOrderDetail(orderID, productID);
            return Json("");
        }
        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {
            ViewBag.OrderID = id;
            return View();
        }
        public IActionResult EditAddress(int id = 0)
        {
            ViewBag.OrderID = id;
            return View();
        }
        [HttpPost]
        public IActionResult SaveAddress(int OrderID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            ViewBag.OrderID = OrderID;
            bool result = OrderDataService.SaveAddress(OrderID, deliveryProvince, deliveryAddress);

            return RedirectToAction("Details", new { OrderID });
        }
        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (shipperID <= 0)
                return Json("Vui lòng chọn người giao hàng");
            bool result = OrderDataService.ShipOrder(id, shipperID);
            if (!result)
                return Json("Đơn hàng không cho phép chuyển cho người giao hàng này");
            return Json("");
        }
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = "",
                };
            }
            return View(input);
        }
        public IActionResult SearchProduct(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                DataProduct = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }
        private List<OrderDetail> GetShoppingCart()
        {
            var shoppingcart = ApplicationContext.GetSessionData<List<OrderDetail>>(SHOPPING_CART);
            if (shoppingcart == null)
            {
                shoppingcart = new List<OrderDetail>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingcart);
            }
            return shoppingcart;
        }
        public IActionResult ShowShoppingCart()
        {
            var model = GetShoppingCart();
            return View(model);
        }
        public IActionResult AddToCart(OrderDetail data)
        {
            if (data.SalePrice <= 0 || data.Quantity <= 0)
                return Json("Giá bán không hợp lệ");
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == data.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(data);
            }
            else
            {
                existsProduct.Quantity += data.Quantity;
                existsProduct.SalePrice = data.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống không thể lập đơn hàng");
            if (customerID <= 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập thông tin đầy đủ");
            int employeeID = Convert.ToInt32(User.GetUserData()?.UserId);
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, shoppingCart);
            ClearCart();
            return Json(orderID);

        }


    }
}

using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace SV20T1020080.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Create()
        {
            var model = new Models.Person()
            {
                name = "Nhat quang",
                BirthDate =new DateTime(1990, 5, 2),
                Salary = 10.2m
            };
            return View(model);
        }
        public IActionResult Save(Models.Person model,string BirthDateInput="")
        {
           //chuyen birthdateInput sang gia tri kieu ngay
           DateTime?dvalue=stringtoDateTime(BirthDateInput);
            if(dvalue.HasValue)
            {
                model.BirthDate=dvalue.Value;
            }
            return Json(model);
        }
        private DateTime? stringtoDateTime(string s,string formats = "d/M/YYYY;d-M-yyyy;y.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using SV20T1020080.BusinessLayers;

namespace SV20T1020080.Web
{
    public static class SelectListHelper
    {
        /// <summary>
        /// danh sách tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> Provinces()
        {
            List<SelectListItem > list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn tỉnh/thành--"
            });
            foreach (var item in CommonDataService.ListofProvince())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName
                });
            }
            return list;
        }
        public static List<SelectListItem> Suppliers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn tỉnh/thành--"
            });
            foreach (var item in CommonDataService.ListofProvince())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName
                });
            }
            return list;
        }
    }
}

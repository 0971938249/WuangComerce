using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020080.BusinessLayers
{/// <summary>
/// khởi tạo lưu trữ các thông tin cấu hình BusinessLayer
/// </summary>
    public static class Configuration
    {/// <summary>
    /// chuỗi kết thông số kết nối CSDL
    /// </summary>
        public static string ConnectionString { get;private set; } = "";
        /// <summary>
        /// khởi tạo cấu hình cho businesslayer
        /// hàm này phải được gọi trước khi ứng dụng chạy
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            Configuration.ConnectionString = connectionString;
        }
    }
}
// static class là gì? khác với classs thong thường
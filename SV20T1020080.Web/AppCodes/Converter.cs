using System.Globalization;

namespace SV20T1020080.Web
{
    public static class Converter
    {
        /// <summary>
        /// Chuyển đổi chuổi s sang giá trị kiểu DateTime 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
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

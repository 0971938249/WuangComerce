using SV20T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020080.DataLayers
{
    public interface IUserAccountDAL
    {
        /// &lt;summary&gt;
        /// Xác thực tài khoản đăng nhập của người dùng.
        /// Hàm trả về thông tin tài khoản nếu xác thực thành công,
        /// ngược lại hàm trả về null
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;userName&quot;&gt;&lt;/param&gt;
        /// &lt;param name=&quot;password&quot;&gt;&lt;/param&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        UserAccount Authorize(string userName, string password);
        /// &lt;summary&gt;
        /// Đổi mật khẩu
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;userName&quot;&gt;&lt;/param&gt;
        /// &lt;param name=&quot;oldPassword&quot;&gt;&lt;/param&gt;
        /// &lt;param name=&quot;newPassword&quot;&gt;&lt;/param&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }
}

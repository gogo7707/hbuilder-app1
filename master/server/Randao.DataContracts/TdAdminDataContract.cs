using System;

namespace Randao.DataContracts
{
	/// <summary>
	/// 管理员表
	/// </summary>
    public class TdAdminDataContract
    {
        public System.Guid UserId { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public System.DateTime RegTime { get; set; }
        public Boolean IsApproved { get; set; }
        public String Email { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }

    }
}

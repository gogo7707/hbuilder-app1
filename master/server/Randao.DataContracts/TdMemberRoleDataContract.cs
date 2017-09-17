using System;

namespace Randao.DataContracts
{
	/// <summary>
	/// ³ÉÔ±½ÇÉ«
	/// </summary>
    public class TdMemberRoleDataContract
    {
        public System.Guid RoleId { get; set; }
        public Int32 MaxCaseCount { get; set; }
        public Int64 RoleKeyId { get; set; }
        public Int64 MinCreditCount { get; set; }
        public Int64 MaxAttention { get; set; }
        public String RoleName { get; set; }
        public String Description { get; set; }
    }
}

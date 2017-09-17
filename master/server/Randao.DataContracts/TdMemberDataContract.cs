using System;
using System.Runtime.Serialization;

namespace Randao.DataContracts
{
	/// <summary>
	/// 成员信息
	/// </summary>
	[Serializable, DataContract(Name = "user_info")]
    public class TdMemberDataContract
    {
		[DataMember(Name = "user_id")]
		public Guid UserId { get; set; }

		[DataMember(Name = "role_id")]
		public Guid RoleId { get; set; }

		[DataMember(Name = "gender")]
        public Int32 Gender { get; set; }

        public Int32 CaseCount { get; set; }

        public System.DateTime LastLoginDate { get; set; }

		[DataMember(Name = "reg_time")]
        public System.DateTime RegTime { get; set; }

		[DataMember(Name = "birthday")]
        public System.DateTime Birthday { get; set; }

		[DataMember(Name = "is_approved")]
        public Boolean IsApproved { get; set; }

		[DataMember(Name = "user_key_id")]
        public Int64 UserKeyId { get; set; }

        public Int64 LoginCount { get; set; }

        public Int64 CreditCount { get; set; }

        [DataMember(Name = "attention_count")]
        public Int64 AttentionCount { get; set; }

		[DataMember(Name = "email")]
        public String Email { get; set; }

        public String RegIP { get; set; }

        public String LastLoginIp { get; set; }

		[DataMember(Name = "QQ")]
        public String QQ { get; set; }

		[DataMember(Name = "phone")]
        public String Phone { get; set; }

		[DataMember(Name = "location")]
        public String Location { get; set; }

        public String WebSite { get; set; }

		[DataMember(Name = "face_img")]
        public String FaceImg { get; set; }

		[DataMember(Name = "user_name")]
        public String UserName { get; set; }

        public String Password { get; set; }

		[DataMember(Name = "nick_name")]
        public String NickName { get; set; }
    }
}

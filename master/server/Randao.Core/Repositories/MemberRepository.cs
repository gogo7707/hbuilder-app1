using Randao.Core.Data;
using Randao.DataContracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Randao.Core.Repositories
{
	internal class MemberRepository : Singleton<MemberRepository>
	{
		private MemberRepository() { }
		public TdMemberDataContract GetMember(string userName)
		{
			string sql = @"SELECT UserKeyId,UserId,RoleId,UserName,Password,Email,IsApproved,LastLoginDate,RegTime,NickName,RegIP,Gender,LastLoginIp,Birthday,QQ,Phone,Location,
						   WebSite,LoginCount,CaseCount,FaceImg,CreditCount,AttentionCount FROM tdMember WHERE UserName=@UserName";

			TdMemberDataContract member = null;

			using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, new SqlParameter("@UserName", userName)))
			{
				member = EntityHelper.GetEntity<TdMemberDataContract>(dr);
			}

			return member;
		}

		public TdMemberDataContract GetMember(long userKeyId)
		{
			string sql = @"SELECT UserKeyId,UserId,RoleId,UserName,Password,Email,IsApproved,LastLoginDate,RegTime,NickName,RegIP,Gender,LastLoginIp,Birthday,QQ,Phone,Location,
						   WebSite,LoginCount,CaseCount,FaceImg,CreditCount,AttentionCount FROM tdMember WHERE UserKeyId=@UserKeyId";

			TdMemberDataContract member = null;

			using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, new SqlParameter("@UserKeyId", userKeyId)))
			{
				member = EntityHelper.GetEntity<TdMemberDataContract>(dr);
			}

			return member;
		}

		public List<TdMemberDataContract> GetMember(IEnumerable<long> userIdList)
		{
			var _list = new List<TdMemberDataContract>();

			if (null == userIdList || !userIdList.Any())
			{
				return _list;
			}

			string sql = @"SELECT UserKeyId,UserId,RoleId,UserName,Password,Email,IsApproved,LastLoginDate,RegTime,NickName,RegIP,Gender,LastLoginIp,Birthday,QQ,Phone,Location,
						   WebSite,LoginCount,CaseCount,FaceImg,CreditCount,AttentionCount FROM tdMember WHERE UserKeyId IN ({0})";

			sql = string.Format(sql, string.Join(",", userIdList));

			using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql))
			{
				_list = EntityHelper.GetList<TdMemberDataContract>(dr);
			}

			return _list;
		}

		public bool InsertMember(TdMemberDataContract meber)
		{
			string sql = @"INSERT INTO TdMember(UserId,RoleId,Gender,CaseCount,LastLoginDate,RegTime,IsApproved,LoginCount,CreditCount,AttentionCount,Email,RegIP,LastLoginIp,QQ,Phone,Location,WebSite,FaceImg,UserName,Password,NickName)
						   VALUES(@UserId,@RoleId,@Gender,@CaseCount,@LastLoginDate,@RegTime,@IsApproved,@LoginCount,@CreditCount,@AttentionCount,@Email,@RegIP,@LastLoginIp,@QQ,@Phone,@Location,@WebSite,@FaceImg,@UserName,@Password,@NickName);SELECT @@IDENTITY";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@UserId", meber.UserId),
				new SqlParameter("@RoleId", meber.RoleId),
				new SqlParameter("@Gender", meber.Gender),
				new SqlParameter("@CaseCount", meber.CaseCount),
				new SqlParameter("@LastLoginDate", meber.LastLoginDate),
				new SqlParameter("@RegTime", meber.RegTime),
				//new SqlParameter("@Birthday", meber.Birthday),
				new SqlParameter("@IsApproved", meber.IsApproved),
				new SqlParameter("@LoginCount", meber.LoginCount),
				new SqlParameter("@CreditCount", meber.CreditCount),
				new SqlParameter("@AttentionCount", meber.AttentionCount),
				new SqlParameter("@Email", meber.Email),
				new SqlParameter("@RegIP", meber.RegIP),
				new SqlParameter("@LastLoginIp", meber.LastLoginIp),
				new SqlParameter("@QQ", meber.QQ),
				new SqlParameter("@Phone", meber.Phone),
				new SqlParameter("@Location", meber.Location),
				new SqlParameter("@WebSite", meber.WebSite),
				new SqlParameter("@FaceImg", meber.FaceImg),
				new SqlParameter("@UserName", meber.UserName),
				new SqlParameter("@Password", meber.Password),
				new SqlParameter("@NickName", meber.NickName)
			};

			meber.UserKeyId = Convert.ToInt64(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm));

			return meber.UserKeyId > 0;
		}

		public bool UpdateLoginInfo(long userKeyId, DateTime lastLoginDate, string lastLoginIp)
		{
			string sql = @"UPDATE TdMember SET LastLoginDate=@LastLoginDate,LastLoginIp=@LastLoginIp,LoginCount=LoginCount+1
						   WHERE UserKeyId = @UserKeyId";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@LastLoginDate", lastLoginDate),
				new SqlParameter("@LastLoginIp", lastLoginIp),
				new SqlParameter("@UserKeyId", userKeyId)
			};

			return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm) > 0;
		}

        public bool UpdateFaceImg(long userKeyId, string faceImgUrl)
        {
            string sql = "UPDATE TdMember SET FaceImg = @FaceImg WHERE UserKeyId = @UserKeyId";

            var _parm = new SqlParameter[] { 
				new SqlParameter("@FaceImg", faceImgUrl),
				new SqlParameter("@UserKeyId", userKeyId)
			};

            return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm) > 0;
        }
	}
}

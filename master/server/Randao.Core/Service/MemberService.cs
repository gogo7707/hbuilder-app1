using Randao.Core.FormsAuth;
using Randao.Core.Repositories;
using Randao.DataContracts;
using System;
using System.Collections.Generic;

namespace Randao.Core.Service
{
	/// <summary>
	/// 用户业务类
	/// </summary>
	public class MemberService
	{
		/// <summary>
		/// 获取用户信息
		/// </summary>
		public TdMemberDataContract GetMember(long userKeyId)
		{
			return MemberRepository.Instance.GetMember(userKeyId);
		}

		/// <summary>
		/// 批量获取用户信息
		/// </summary>
		public List<TdMemberDataContract> GetMember(IEnumerable<long> userIdList)
		{
			return MemberRepository.Instance.GetMember(userIdList);
		}

        /// <summary>
        /// 修改头像地址
        /// </summary>
        /// <param name="userKeyId"></param>
        /// <param name="faceImgUrl"></param>
        /// <returns></returns>
        public bool UpdateFaceImg(long userKeyId, string faceImgUrl)
        {
            return MemberRepository.Instance.UpdateFaceImg(userKeyId, faceImgUrl);
        }

		/// <summary>
		/// 用户登录
		/// </summary>
		/// <returns>返回空则登陆失败</returns>
        public ReturnResult<TdMemberDataContract> Login(string userName, string passWord, string loginIp)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(passWord))
            {
                return new ReturnResult<TdMemberDataContract>(101, null, "用户名或者密码错误");
            }

            var member = MemberRepository.Instance.GetMember(userName);
            if (null != member && member.Password.Equals(passWord.GetEncrypt())) //密码解密
            {
                //更新登陆信息
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                    MemberRepository.Instance.UpdateLoginInfo(member.UserKeyId, DateTime.Now, loginIp)
                );
                //FormsAuthenticationWrapper.Instance.SetAuthCookie(member.UserKeyId.ToString(), true);
                return new ReturnResult<TdMemberDataContract>(member);
            }
            return new ReturnResult<TdMemberDataContract>(102, null, "用户名或者密码错误");
        }

		/// <summary>
		/// 用户注册
		/// </summary>
		public ReturnResult<long> RegisterUser(TdMemberDataContract member)
		{
			if (string.IsNullOrWhiteSpace(member.UserName))
			{
				return new ReturnResult<long>(101, 0, "参数异常,用户名不允许为空");
			}
			if (string.IsNullOrWhiteSpace(member.Password))
			{
				return new ReturnResult<long>(102, 0, "参数异常,密码不允许为空");
			}
			if (null != MemberRepository.Instance.GetMember(member.UserName))
			{
				return new ReturnResult<long>(103, 0, "用户名已存在");
			}

			member.RoleId = Guid.Empty;
			member.RegTime = DateTime.Now;
			member.LastLoginDate = DateTime.Now;
			member.UserId = Guid.NewGuid();
			member.Password = member.Password.GetEncrypt();
			member.NickName = member.UserName;

			if (MemberRepository.Instance.InsertMember(member))
			{
				//FormsAuthenticationWrapper.Instance.SetAuthCookie(member.UserKeyId.ToString(), true);
				return new ReturnResult<long>(member.UserKeyId);
			}
			return new ReturnResult<long>(104, 0, "注册失败");
		}

		/// <summary>
		/// 获取当前登录用户ID
		/// </summary>
		/// <returns></returns>
		public long GetCurrentLoginUserKeyId()
		{
			return new FormsAuthenticationService().userKeyId;
		}

		/// <summary>
		/// 获取当前登录用户
		/// </summary>
		/// <returns></returns>
		public TdMemberDataContract GetCurrentLoginUser()
		{
			var userKeyId = this.GetCurrentLoginUserKeyId();

			if (userKeyId < 1)
			{
				return null;
			}

			return MemberRepository.Instance.GetMember(userKeyId);
		}
	}
}

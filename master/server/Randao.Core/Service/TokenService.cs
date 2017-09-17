using Randao.Core.Repositories;
using Randao.DataContracts;
using System;
using System.Transactions;

namespace Randao.Core.Service
{
	public class TokenService
	{
		private const long ACCESS_TOKEN_EXPIRES = 3110400;//accesstoken有效期为360天
		private const long REFRESH_TOKEN_EXPIRES = 6220800;//refreshToken有效期为360天

		private MemberService memberService;
		public TokenService(MemberService memberService)
		{
			this.memberService = memberService;
		}

		public OauthToken GetAccessToken(string accessToken)
		{
			if (string.IsNullOrWhiteSpace(accessToken))
			{
				return null;
			}
			return TokenRepository.Instance.GetAccessToken(accessToken);
		}

		public RefreshTokenDataContract GetRefreshToken(string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
			{
				return null;
			}
			return TokenRepository.Instance.GetRefreshToken(refreshToken);
		}

		public ReturnResult<OauthToken> CreateAccessToken(string userName, string passWord, string ip)
		{
			var memberInfo = memberService.Login(userName, passWord, ip);

			if (null == memberInfo || null == memberInfo.Data || memberInfo.Code != 0)
			{
				return new ReturnResult<OauthToken>(101, null, "用户名或者密码错误");
			}

			return this.CreateAccessToken(memberInfo.Data);
		}

		public ReturnResult<OauthToken> CreateAccessToken(TdMemberDataContract memberInfo)
		{
			if (null == memberInfo || memberInfo.UserKeyId < 1)
			{
				return new ReturnResult<OauthToken>(101, null, "参数memberInfo错误");
			}

			OauthToken accessToken = new OauthToken();
			accessToken.ClientID = 10001;//目前默认为10001
			accessToken.UserKeyId = memberInfo.UserKeyId;
			accessToken.AccessToken = CreateToken();
			accessToken.Expires = DateTime.Now.Epoch() + ACCESS_TOKEN_EXPIRES;
			accessToken.Scope = "all";//权限默认
			accessToken.User = memberInfo;

			RefreshTokenDataContract refreshToken = new RefreshTokenDataContract();
			refreshToken.ClientID = accessToken.ClientID;
			refreshToken.UserKeyID = accessToken.UserKeyId;
			refreshToken.RefreshToken = CreateToken();
			refreshToken.Expires = DateTime.Now.Epoch() + REFRESH_TOKEN_EXPIRES;

			accessToken.RefreshToken = refreshToken.RefreshToken;

			TokenRepository.Instance.InsertAccessToken(accessToken);
			TokenRepository.Instance.InsertRefreshToken(refreshToken);

			return new ReturnResult<OauthToken>(accessToken);
		}

		public ReturnResult<OauthToken> RefreshAccessToken(string refreshToken, int clientId)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
			{
				return new ReturnResult<OauthToken>(21, null, "参数refreshToken错误");
			}
			var rToken = TokenRepository.Instance.GetRefreshToken(refreshToken);
			if (null == rToken)
			{
				return new ReturnResult<OauthToken>(22, null, "参数refreshToken错误");
			}
			if (rToken.ClientID != clientId)
			{
				return new ReturnResult<OauthToken>(23, null, "参数clientId错误");
			}
			if (rToken.Expires < DateTime.Now.Epoch())
			{
				return new ReturnResult<OauthToken>(24, null, "refreshToken已过期,请重新登录");
			}
			var user = memberService.GetMember(rToken.UserKeyID);
			if (null == user)
			{
				return new ReturnResult<OauthToken>(25, null, "当前用户已失效,请重新登录");
			}
			return this.CreateAccessToken(user);
		}

		private string CreateToken()
		{
			return string.Concat(Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")).ToLower().CutString(40);
		}
	}
}

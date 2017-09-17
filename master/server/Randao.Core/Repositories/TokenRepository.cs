using Randao.Core.Data;
using Randao.DataContracts;
using System.Data;
using System.Data.SqlClient;

namespace Randao.Core.Repositories
{
	internal class TokenRepository : Singleton<TokenRepository>
	{
		private TokenRepository() { }

		public OauthToken GetAccessToken(string accessToken)
		{
			string sql = "SELECT AccessToken,ClientId,UserKeyId,Expires,Scope FROM access_tokens WHERE AccessToken = @accessToken";

			OauthToken model = null;

			using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, new SqlParameter("accessToken", accessToken)))
			{
				model = EntityHelper.GetEntity<OauthToken>(dr);
			}

			return model;
		}

		public RefreshTokenDataContract GetRefreshToken(string refreshToken)
		{
			string sql = "SELECT RefreshToken,ClientId,UserKeyId,Expires FROM refresh_tokens WHERE RefreshToken = @refreshToken";

			RefreshTokenDataContract model = null;

			using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, new SqlParameter("refreshToken", refreshToken)))
			{
				model = EntityHelper.GetEntity<RefreshTokenDataContract>(dr);
			}

			return model;
		}

		public bool InsertAccessToken(OauthToken accessToken)
		{
			string sql = @"INSERT INTO access_tokens(AccessToken,ClientId,UserKeyId,Expires,Scope)
						   VALUES(@AccessToken,@ClientId,@UserKeyId,@Expires,@Scope)";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@AccessToken", accessToken.AccessToken),
				new SqlParameter("@ClientId", accessToken.ClientID),
				new SqlParameter("@UserKeyId", accessToken.UserKeyId),
				new SqlParameter("@Expires",accessToken.Expires),
				new SqlParameter("@Scope", accessToken.Scope)
			};

			return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm) > 0;
		}

		public bool InsertRefreshToken(RefreshTokenDataContract refreshToken)
		{
			string sql = @"INSERT INTO refresh_tokens(RefreshToken,ClientId,UserKeyId,Expires)
						   VALUES(@RefreshToken,@ClientId,@UserKeyId,@Expires)";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@RefreshToken", refreshToken.RefreshToken),
				new SqlParameter("@ClientId", refreshToken.ClientID),
				new SqlParameter("@UserKeyId", refreshToken.UserKeyID),
				new SqlParameter("@Expires",refreshToken.Expires)
			};

			return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm) > 0;
		}
	}
}

using Autofac;
using Randao.Core.FormsAuth;
using Randao.Core.Service;

namespace Randao.Core
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<ArticleAttentionService>();
			builder.RegisterType<ArticleCategoryService>();
			builder.RegisterType<ArticleCollectService>();
			builder.RegisterType<ArticleCommentService>();
			builder.RegisterType<ArticleContentService>();
			builder.RegisterType<MemberService>();
			builder.RegisterType<TokenService>();
			
			base.Load(builder);
		}
	}
}

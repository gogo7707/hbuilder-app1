using Randao.Core.Repositories;
using Randao.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randao.Core.Service
{
	public class ArticleCategoryService
	{
		public ArticleCategoryDataContract GetArticleCategory(int categoryId)
		{
			return ArticleCategoryRepository.Instance.GetArticleCategory(categoryId);
		}

		public List<ArticleCategoryDataContract> GetWapArticleCategoryList()
		{
			return ArticleCategoryRepository.Instance.GetWapArticleCategoryList();
		}

		internal int GetRootCategoryId(int categoryId)
		{
			var articleCategory = ArticleCategoryRepository.Instance.GetArticleCategory(categoryId);
			if (null == articleCategory)
			{
				return -1;
			}
			var categoryList = articleCategory.parentIdList.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
			if (!categoryList.Any())
			{
				return -1;
			}
			if (categoryList.First().Equals("0"))
			{
				return categoryId;
			}
			return Convert.ToInt32(categoryList.First());
		}

		internal List<int> GetParentCategoryList(int categoryId)
		{
			var articleCategory = ArticleCategoryRepository.Instance.GetArticleCategory(categoryId);
			if (null == articleCategory)
			{
				return new List<int>();
			}
			return articleCategory.parentIdList.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
		}
	}
}

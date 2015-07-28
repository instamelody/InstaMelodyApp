using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InstaMelody.Business.Properties;
using InstaMelody.Data;
using InstaMelody.Infrastructure;
using InstaMelody.Model;
using NLog;

namespace InstaMelody.Business
{
    public class CategoryBll
    {
        #region Public Methods

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">No categories found.</exception>
        public IList<Category> GetAllCategories()
        {
            var dal = new Categories();
            var parents = dal.GetParentCategories();
            if (parents == null || !parents.Any())
            {
                InstaMelodyLogger.Log("No categories found for GetAllCategories().", LogLevel.Error);
                throw new DataException("No categories found.");
            }

            var categories = new List<Category>();

            foreach (var category in parents)
            {
                category.Children = GetChildCategoriesById(category.Id);
                categories.Add(category);
            }

            return categories;
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public Category GetCategory(Category category)
        {
            Category result = null;

            var dal = new Categories();
            if (!category.Id.Equals(default(int)))
            {
                result = dal.GetCategoryById(category.Id);
            }

            if (result == null
                && !string.IsNullOrWhiteSpace(category.Name))
            {
                result = dal.GetCategoryByNameAndParent(category.Name, category.ParentId);
            }

            return result;
        }

        /// <summary>
        /// Gets the child categories.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">No category Id provided</exception>
        public IList<Category> GetChildCategories(Category category)
        {
            if (!category.Id.Equals(default(int))) 
                return GetChildCategoriesById(category.Id);

            InstaMelodyLogger.Log("No category Id provided for GetChildCategories().", LogLevel.Error);
            throw new DataException("No category Id provided.");
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="adminToken">The admin token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Cannot add or update categories withouth the correct access token.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public Category AddCategory(Category category, Guid adminToken)
        {
            if (!adminToken.Equals(Settings.Default.AdminAccessToken))
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot add categories withouth the correct access token. Token: {0}", 
                        adminToken), LogLevel.Error);
                throw new UnauthorizedAccessException("Cannot add categories withouth the correct access token.");
            }

            var dal = new Categories();
            if (!category.Id.Equals(default(int)))
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot add a Category with an Id value provided. Id: {0}, Token: {1}", 
                        category.Id, adminToken), LogLevel.Error);
                throw new DataException("Cannot add a Category with an Id value provided.");
            }
            else
            {
                var matching = dal.GetCategoryByNameAndParent(category.Name, category.ParentId);
                if (matching != null)
                {
                    InstaMelodyLogger.Log(
                        string.Format("Cannot add a duplicate Category. Name: {0}, Token: {1}",
                            category.Name, adminToken), LogLevel.Error);
                    throw new DataException(string.Format("Cannot add a duplicate Category with Name: {0}", matching.Name));
                }

                // add category
                category.IsDeleted = false;
                category.DateCreated = DateTime.UtcNow;
                category.DateModified = DateTime.UtcNow;

                var addedCategory = dal.AddCategory(category);
                return addedCategory;
            }
        }

        /// <summary>
        /// Updates the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="adminToken">The admin token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Cannot add or update categories withouth the correct access token.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public Category UpdateCategory(Category category, Guid adminToken)
        {
            if (!adminToken.Equals(Settings.Default.AdminAccessToken))
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot add or update categories withouth the correct access token. Token: {0}",
                        adminToken), LogLevel.Error);
                throw new UnauthorizedAccessException("Cannot add or update categories withouth the correct access token.");
            }

            var dal = new Categories();
            var findCategory = dal.GetCategoryById(category.Id);
            if (findCategory == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Unable to find Category in database. Id: {0}, Token: {1}",
                        category.Id, adminToken), LogLevel.Error);
                throw new DataException(string.Format("Unable to find Category: {0}.", category.Id));
            }

            var matching = dal.GetCategoryByNameAndParent(category.Name, category.ParentId);
            if (matching != null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot update a Category to have duplicate information. Id: {0}, Name: {1}, Token: {2}",
                        matching.Id, matching.Name, adminToken), LogLevel.Error);
                throw new DataException(
                    string.Format("Cannot update a Category to have duplicate information. Id: {0}, Name: {1}", 
                        matching.Id, matching.Name));
            }

            // update category
            category.DateModified = DateTime.UtcNow;

            var updatedCategory = dal.UpdateCategory(category);
            return updatedCategory;
        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="adminToken">The admin token.</param>
        /// <exception cref="System.UnauthorizedAccessException">Cannot delete categories withouth the correct access token.</exception>
        /// <exception cref="System.Data.DataException">No category Id provided</exception>
        public void DeleteCategory(Category category, Guid adminToken)
        {
            if (!adminToken.Equals(Settings.Default.AdminAccessToken))
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot delete categories withouth the correct access token. Token: {0}",
                        adminToken), LogLevel.Error);
                throw new UnauthorizedAccessException("Cannot delete categories withouth the correct access token.");
            }

            if (category.Id.Equals(default(int)))
            {
                InstaMelodyLogger.Log(
                    string.Format("No category Id provided. Token: {0}",
                        adminToken), LogLevel.Error);
                throw new DataException("No category Id provided.");
            }

            var dal = new Categories();
            dal.DeleteCategory(category.Id);

            var retrieve = dal.GetCategoryById(category.Id);
            if (retrieve == null) return;

            InstaMelodyLogger.Log(
                string.Format("Unable to delete Category. CategoryId: {0}, Token: {1}",
                    retrieve.Id, adminToken), LogLevel.Error);
            throw new DataException(string.Format("Unable to delete Category: {0}", retrieve.Id));
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the child categories by identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        private IList<Category> GetChildCategoriesById(int categoryId)
        {
            var children = new List<Category>();

            var dal = new Categories();
            var results = dal.GetChildCategories(categoryId);
            if (results.Any())
            {
                children.AddRange(results);
            }

            foreach (var category in children)
            {
                category.Children = GetChildCategoriesById(category.Id);
            }

            return children;
        }

        #endregion Private Methods
    }
}

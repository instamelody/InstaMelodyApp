using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using InstaMelody.API.Constants;
using InstaMelody.Business;
using InstaMelody.Infrastructure;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;

using NLog;

using Utilities = InstaMelody.Infrastructure.Utilities;

namespace InstaMelody.API.Controllers
{
    [RoutePrefix(Routes.PrefixCategory01)]
    public class CategoriesController : ApiController
    {
        /// <summary>
        /// Gets the time now.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponseMessage" />.
        /// </returns>
        [HttpGet]
        [Route(Routes.RouteDateTime)]
        public HttpResponseMessage GetTimeNow()
        {
            var timeStamp = DateTime.UtcNow;
            var result = new ApiTime
            {
                DateAndTime = timeStamp,
                UnixDateAndTime = Utilities.UnixTimestampFromDateTime(timeStamp)
            };

            var response = this.Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteNew)]
        public HttpResponseMessage AddCategory(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Create Category - Category Id: {0}, Token: {1}", request.Category.Id, request.Token), 
                        LogLevel.Trace);

                    var bll = new CategoryBll();
                    var result = bll.AddCategory(request.Category, request.Token);

                    response = this.Request.CreateResponse(HttpStatusCode.OK, result);

                }
                catch (Exception exc)
                {
                    if (exc is UnauthorizedAccessException)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, exc.Message);
                    }
                    else if (exc is DataException)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, exc.Message);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message, exc);
                    }
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL AddCategory request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullCategory);
            }

            return response;
        }

        /// <summary>
        /// Updates the category.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUpdate)]
        public HttpResponseMessage UpdateCategory(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Update Category - Category Id: {0}, Token: {1}", request.Category.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new CategoryBll();
                    var result = bll.UpdateCategory(request.Category, request.Token);

                    if (result == null || result.Id.Equals(default(int)))
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, 
                            string.Format(Exceptions.FailedUpdateCategory, request.Category.Id));
                    }
                    else
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                    }

                }
                catch (Exception exc)
                {
                    if (exc is UnauthorizedAccessException)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, exc.Message);
                    }
                    else if (exc is DataException)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, 
                            string.Format(Exceptions.FailedUpdateCategory, request.Category.Id));
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message, exc);
                    }
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL UpdateCategory request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullCategory);
            }

            return response;
        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteDelete)]
        public HttpResponseMessage DeleteCategory(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Delete Category - Category Id: {0}, Token: {1}", request.Category.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new CategoryBll();
                    bll.DeleteCategory(request.Category, request.Token);

                    response = this.Request.CreateResponse(HttpStatusCode.Accepted);

                }
                catch (Exception exc)
                {
                    if (exc is UnauthorizedAccessException)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, exc.Message);
                    }
                    else if (exc is DataException)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, 
                            string.Format(Exceptions.FailedDeleteCategory, request.Category.Id));
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message, exc);
                    }
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL DeleteCategory request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullCategory);
            }

            return response;
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteBlank)]
        public HttpResponseMessage GetCategories()
        {
            HttpResponseMessage response;

            try
            {
                var bll = new CategoryBll();
                var results = bll.GetAllCategories();

                if (results == null || !results.Any())
                {
                    response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedFindCategories);
                }
                else
                {
                    response = this.Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }
            catch (Exception exc)
            {
                if (exc is DataException)
                {
                    response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedFindCategories);
                }
                else
                {
                    response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message, exc);
                }
            }
            

            return response;
        }

        /// <summary>
        /// Gets the child categories.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteChildCategories)]
        public HttpResponseMessage GetChildCategories()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var categoryId = nvc["id"];

            InstaMelodyLogger.Log(
                string.Format("Get Child Categories - Category Id: {0}", categoryId), 
                LogLevel.Trace);

            int _categoryId;
            int.TryParse(categoryId, out _categoryId);

            if (string.IsNullOrWhiteSpace(categoryId) || _categoryId.Equals(int.MinValue))
            {
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedCategoryParameter);
            }
            else
            {
                var bll = new CategoryBll();
                var results = bll.GetChildCategories(new Category
                {
                    Id = _categoryId
                });

                if (results == null || !results.Any())
                {
                    response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, 
                        string.Format(Exceptions.FailedFindChildCategories, _categoryId));
                }
                else
                {
                    response = this.Request.CreateResponse(HttpStatusCode.OK, results);
                }
            }

            return response;
        }
    }
}

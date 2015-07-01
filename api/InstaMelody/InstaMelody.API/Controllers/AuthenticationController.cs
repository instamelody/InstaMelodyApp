using System;
using System.Data;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

using InstaMelody.API.Constants;
using InstaMelody.Business;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Infrastructure;

using NLog;

using Utilities = InstaMelody.Infrastructure.Utilities;

namespace InstaMelody.API.Controllers
{
    [RoutePrefix(Routes.PrefixAuth01)]
    public class AuthenticationController : ApiController
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
        /// Authenticates the a user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUpdate)]
        public HttpResponseMessage Authenticate(User request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format(
                            "Authentication request - User: {0}, Password: {1}.",
                            request.DisplayName ?? "NULL",
                            request.Password ?? "NULL"),
                        LogLevel.Trace);

                    var bll = new AuthenticationBLL();
                    var result = bll.Authenticate(request.DisplayName, request.EmailAddress, request.Password);

                    if (result.Token.Equals(default(Guid)))
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedAuthentication);
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
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, exc.Message);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message, exc);
                    }
                    response.ReasonPhrase = exc.Message;
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL Authentication request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.NoContent, Exceptions.NullAuth);
            }

            return response;
        }

        /// <summary>
        /// Validates the session.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteBlank)]
        public HttpResponseMessage ValidateSession()
        {
            HttpResponseMessage response;

            NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            if (nvc.AllKeys.Any())
            {
                try
                {
                    var token = nvc["token"];
                    var userId = nvc["id"];

                    InstaMelodyLogger.Log(
                        string.Format(
                            "Auth Validation request - User: {0}, Token: {1}.",
                            userId ?? "NULL",
                            token ?? "NULL"),
                        LogLevel.Trace);

                    if (token == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                    }
                    else
                    {
                        var _token = new Guid();
                        var _userId = new Guid();
                        Guid.TryParse(token, out _token);
                        Guid.TryParse(userId, out _userId);

                        var sessionUser = Business.Utilities.GetUserBySession(_token);
                        if (sessionUser == null || sessionUser.Id == default(Guid) || sessionUser.Id != _userId)
                        {
                            response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                        }
                        else
                        {
                            var bll = new AuthenticationBLL();
                            var result = bll.ValidateSession(_userId, _token);

                            if (result.Token.Equals(default(Guid)))
                            {
                                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                            }
                            else
                            {
                                response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (exc is UnauthorizedAccessException)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, exc.Message);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message, exc);
                    }
                    response.ReasonPhrase = exc.Message;
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL AuthValidation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.NoContent, Exceptions.NullAuth);
            }

            return response;
        }

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteEndSession)]
        public HttpResponseMessage EndSession(UserSession request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format(
                            "End Session - User: {0}, Token: {1}.",
                            request.UserId.ToString() ?? "NULL",
                            request.Token.ToString() ?? "NULL"),
                        LogLevel.Trace);

                    var bll = new AuthenticationBLL();
                    bll.EndSession(request.UserId, request.Token);

                    var result = new ApiMessage
                    {
                        Message = string.Format("User {0} successfully logged out.", request.UserId)
                    };

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
                    response.ReasonPhrase = exc.Message;
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL Authentication request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.NoContent, Exceptions.NullAuth);
            }

            return response;
        }

        /// <summary>
        /// Updates the user password.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUpdatePassword)]
        public HttpResponseMessage UpdateUserPassword(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.UserPassword != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Update User Password - Token: {0}.", request.Token),
                        LogLevel.Trace);

                    var bll = new AuthenticationBLL();
                    var result = bll.UpdatePassword(request.UserPassword, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedUpdatePassword);
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
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, exc.Message);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message, exc);
                    }
                    response.ReasonPhrase = exc.Message;
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL UpdatePassword request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.NoContent, Exceptions.NullUpdatePassword);
            }

            return response;
        }

        [HttpPost]
        [Route(Routes.RouteResetPassword)]
        public HttpResponseMessage ResetUserPassword(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }
    }
}

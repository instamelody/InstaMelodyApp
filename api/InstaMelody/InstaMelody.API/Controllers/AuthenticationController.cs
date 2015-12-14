using System;
using System.Data;
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
        [Route(Routes.RouteUser)]
        public HttpResponseMessage Authenticate(User request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format(
                            "Authentication request - Display Name: {0}, Email Address: {1}, Password: {2}, Device Token: {3}, Facebook Token: {4}, Twitter Token: {5}",
                            request.DisplayName ?? "NULL",
                            request.EmailAddress ?? "NULL",
                            request.Password ?? "NULL",
                            request.DeviceToken,
                            request.FacebookToken ?? "NULL",
                            request.TwitterToken ?? "NULL"),
                        LogLevel.Trace);

                    var bll = new AuthenticationBll();
                    var result = new ApiToken();

                    if (!string.IsNullOrEmpty(request.DisplayName) || !string.IsNullOrEmpty(request.EmailAddress))
                        result = bll.Authenticate(request.DisplayName, request.EmailAddress, request.Password, request.DeviceToken);
                    else if (!string.IsNullOrEmpty(request.FacebookToken) || !string.IsNullOrEmpty(request.TwitterToken))
                        result = bll.Authenticate(request.Id, request.FacebookToken, request.TwitterToken, request.DeviceToken);

                    if (result == null || result.Token.Equals(default(Guid)))
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
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

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            if (nvc.AllKeys.Any())
            {
                try
                {
                    var id = nvc["id"];
                    var token = nvc["token"];
                    var deviceToken = nvc["deviceToken"];

                    InstaMelodyLogger.Log(
                        string.Format(
                            "Auth Validation request - User Id: {0}, Token: {1}, Device Token: {2}",
                            id ?? "NULL",
                            token ?? "NULL",
                            deviceToken ?? "NULL"),
                        LogLevel.Trace);

                    if (token == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                    }
                    else
                    {
                        var _token = new Guid();
                        Guid.TryParse(token, out _token);

                        var _userId = new Guid();
                        Guid.TryParse(id, out _userId);

                        var sessionUser = Business.Utilities.GetUserBySession(_token);
                        if (sessionUser == null || sessionUser.Id == default(Guid))
                        {
                            response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                        }
                        else
                        {
                            var bll = new AuthenticationBll();
                            var result = string.IsNullOrWhiteSpace(deviceToken) 
                                ? bll.ValidateSession(_userId, _token) 
                                : bll.ValidateSession(_token, deviceToken);

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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
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
                            "End Session - User Id: {0}, Token: {1}, Device Token: {2}",
                            request.UserId,
                            request.Token,
                            request.DeviceToken),
                        LogLevel.Trace);

                    var bll = new AuthenticationBll();

                    if (!string.IsNullOrWhiteSpace(request.DeviceToken))
                        bll.EndSession(request.Token, request.DeviceToken);
                    else
                        bll.EndSession(request.UserId, request.Token);

                    var result = new ApiMessage
                    {
                        Message = "User successfully logged out."
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
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

                    var bll = new AuthenticationBll();
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL UpdatePassword request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.NoContent, Exceptions.NullUpdatePassword);
            }

            return response;
        }

        /// <summary>
        /// Resets the user password.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteResetPassword)]
        public HttpResponseMessage ResetUserPassword(User request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    var user = string.IsNullOrWhiteSpace(request.DisplayName)
                        ? request.EmailAddress
                        : request.DisplayName;
                    InstaMelodyLogger.Log(
                        string.Format("Reset User Password - User: {0}.", user),
                        LogLevel.Trace);

                    var bll = new AuthenticationBll();

                    try
                    {
                        var u = bll.ResetPassword(request);
                        response = this.Request.CreateResponse(HttpStatusCode.OK, new ApiMessage
                        {
                            Message = string.Format("An email has been sent to {0} with a temporary password.", u.EmailAddress)
                        });
                    }
                    catch (Exception)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedResetPassword);
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL UpdatePassword request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.NoContent, Exceptions.NullUpdatePassword);
            }

            return response;
        }
    }
}

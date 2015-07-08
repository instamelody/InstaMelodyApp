using System;
using System.Data;
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
    [RoutePrefix(Routes.PrefixMelody01)]
    public class MelodiesController : ApiController
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
        /// Gets the melodies.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteBlank)]
        public HttpResponseMessage GetMelodies()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            int _id;
            var id = nvc["id"];
            int.TryParse(id, out _id);

            int _groupId;
            var groupId = nvc["groupId"];
            int.TryParse(groupId, out _groupId);

            var fileName = nvc["fileName"];
            var groupName = nvc["groupName"];

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetMelodies request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get Melodies - Token: {0}", _token),
                        LogLevel.Trace);

                    var bll = new MelodyBLL();

                    object result;
                    if (!_groupId.Equals(default(int)) || !string.IsNullOrWhiteSpace(groupName))
                    {
                        result = bll.GetFileGroup(new FileGroup { Id = _groupId, Name = groupName });
                    }
                    else if (!_id.Equals(default(int)) || !string.IsNullOrWhiteSpace(fileName))
                    {
                        result = bll.GetBaseMelody(new Melody { Id = _id, FileName = fileName });
                    }
                    else
                    {
                        result = bll.GetFileGroups();
                    }

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedGetMelodies);
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

            return response;
        }

        /// <summary>
        /// Gets the user melodies.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteUser)]
        public HttpResponseMessage GetUserMelodies()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            Guid _melodyId;
            var melodyId = nvc["id"];
            Guid.TryParse(melodyId, out _melodyId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetUserMelodies request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get User Melodies - Token: {0}, MelodyId: {1}", _token, _melodyId),
                        LogLevel.Trace);

                    var bll = new MelodyBLL();
                    object result;

                    if (_melodyId.Equals(default(Guid)))
                    {
                        result = bll.GetUserMelodies(_token);
                    }
                    else
                    {
                        result = bll.GetUserMelody(new UserMelody { Id = _melodyId }, _token);
                    }

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedGetUserMelodies);
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

            return response;
        }

        /// <summary>
        /// Adds the user melody.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPut]
        [HttpPost]
        [Route(Routes.RouteNew)]
        public HttpResponseMessage AddUserMelody(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Add User Melody - Token: {0}",
                            request.Token),
                        LogLevel.Trace);

                    var bll = new MelodyBLL();
                    var result = bll.CreateUserMelody(request.UserMelody, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedCreateMelody);
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
                    else if (exc is DataException || exc is ArgumentException)
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
                InstaMelodyLogger.Log("Received NULL AddUserMelody request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullMelodies);
            }

            return response;
        }

        /// <summary>
        /// Deletes the user melody.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteDelete)]
        public HttpResponseMessage DeleteUserMelody(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Delete User Melody - Token: {0}, User Melody Id: {1}",
                            request.Token,
                            request.UserMelody.Id),
                        LogLevel.Trace);

                    var bll = new MelodyBLL();
                    bll.DeleteUserMelody(request.UserMelody, request.Token);

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
                InstaMelodyLogger.Log("Received NULL DeleteUserMelody request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullMelodies);
            }

            return response;
        }
    }
}

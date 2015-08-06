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

            int _catId;
            var catId = nvc["categoryId"];
            int.TryParse(catId, out _catId);

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

                    var bll = new MelodyBll();

                    object result;
                    if (!_groupId.Equals(default(int)) || !string.IsNullOrWhiteSpace(groupName))
                    {
                        result = bll.GetFileGroup(new FileGroup {Id = _groupId, Name = groupName}, _token);
                    }
                    else if (!_catId.Equals(default(int)))
                    {
                        result = bll.GetBaseMelodiesByCategory(new Category {Id = _catId}, _token);
                    }
                    else if (!_id.Equals(default(int)) || !string.IsNullOrWhiteSpace(fileName))
                    {
                        result = bll.GetBaseMelody(new Melody {Id = _id, FileName = fileName}, _token);
                    }
                    else
                    {
                        result = bll.GetFileGroups(_token);
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

                    var bll = new MelodyBll();
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

                    var bll = new MelodyBll();
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

                    var bll = new MelodyBll();
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
                    
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL DeleteUserMelody request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullMelodies);
            }

            return response;
        }

        /// <summary>
        /// Gets the loop.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteLoop)]
        public HttpResponseMessage GetLoop()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            Guid _loopId;
            var loopId = nvc["id"];
            Guid.TryParse(loopId, out _loopId);

            Guid _userId;
            var userId = nvc["userId"];
            Guid.TryParse(userId, out _userId);

            var name = nvc["name"];

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetLoop request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get User Loop - Token: {0}, MelodyId: {1}", _token, _loopId),
                        LogLevel.Trace);

                    var bll = new MelodyBll();
                    object result;

                    if (_loopId.Equals(default(Guid)) 
                        && _userId.Equals(default(Guid)) 
                        && string.IsNullOrWhiteSpace(name))
                    {
                        result = bll.GetUserLoops(_token);
                    }
                    else
                    {
                        result = bll.GetLoop(new UserLoop
                        {
                            Id = _loopId,
                            Name = name,
                            UserId = _userId
                        }, _token);
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
                    
                }
            }

            return response;
        }

        /// <summary>
        /// Adds the user loop.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteLoop)]
        public HttpResponseMessage AddUserLoop(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Add User Loop - Token: {0}",
                            request.Token),
                        LogLevel.Trace);

                    var bll = new MelodyBll();
                    var result = bll.CreateLoop(request.Loop, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedCreateLoop);
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
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL CreateUserLoop request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullLoop);
            }

            return response;
        }

        /// <summary>
        /// Attaches a new part to a user loop.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteLoopAttach)]
        public HttpResponseMessage AttachToUserLoop(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Loop != null && request.LoopPart != null)
            {
                try
                {
                    // Log call
                    var loop = (!request.Loop.Id.Equals(default(Guid)))
                        ? request.Loop.Id
                        : (object)request.Loop.Name;
                    InstaMelodyLogger.Log(
                        string.Format("Attach to User Loop - Token: {0}, UserLoop: {1}",
                            request.Token,
                            loop),
                        LogLevel.Trace);

                    var bll = new MelodyBll();
                    var result = bll.AttachPartToLoop(request.Loop, request.LoopPart, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedAttachToLoop, loop));
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
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL AttachToUserLoop request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullLoop);
            }

            return response;
        }

        /// <summary>
        /// Deletes the loop.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteLoopDelete)]
        public HttpResponseMessage DeleteLoop(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Loop != null)
            {
                try
                {
                    var bll = new MelodyBll();
                    if (request.UserMelody != null)
                    {
                        var part = request.UserMelody.Id.Equals(default(Guid))
                            ? request.UserMelody.Name
                            : request.UserMelody.Id.ToString();
                        InstaMelodyLogger.Log(
                            string.Format("Delete Loop Part - Token: {0}, LoopPart: {1}",
                                request.Token,
                                part),
                            LogLevel.Trace);

                        var result = bll.DeletePartFromLoop(request.Loop, request.UserMelody, request.Token);
                        response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                    else
                    {
                        var loop = request.Loop.Id.Equals(default(Guid))
                            ? request.Loop.Name
                            : request.Loop.Id.ToString();
                        InstaMelodyLogger.Log(
                            string.Format("Delete Loop - Token: {0}, Loop: {1}",
                                request.Token,
                                loop), 
                            LogLevel.Trace);

                        bll.DeleteLoop(request.Loop, request.Token);
                        response = this.Request.CreateResponse(HttpStatusCode.Accepted);
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
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL DeleteLoop request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullLoop);
            }

            return response;
        }
    }
}

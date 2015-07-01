using System;
using System.Collections.Generic;
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
            var token = nvc["token"];
            var id = nvc["id"];
            var categoryId = nvc["categoryId"];

            Guid _token;
            Guid.TryParse(token, out _token);

            int _id;
            int.TryParse(id, out _id);

            int _categoryId;
            int.TryParse(categoryId, out _categoryId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetMelodies request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No Token provided.");
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get Melodies - Token: {0}", _token),
                        LogLevel.Trace);

                    IList<Melody> results = null;
                    Melody result = null;

                    var bll = new MelodyBLL();
                    if (id != null && !_id.Equals(default(int)))
                    {
                        // get melody by id
                        result = bll.GetBaseMelody(new Melody { Id = _id }, _token);
                    }
                    else if (categoryId != null && !_categoryId.Equals(default(int)))
                    {
                        // get melodies by category
                        results = bll.GetBaseMelodies(_token, new Category{ Id = _categoryId });
                    }
                    else
                    {
                        // get all melodies
                        results = bll.GetBaseMelodies(_token);
                    }

                    if (result != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                    else if (results != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, results);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedGetMelodies));
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
            var token = nvc["token"];
            var id = nvc["id"];
            var categoryId = nvc["categoryId"];

            Guid _token;
            Guid.TryParse(token, out _token);

            int _id;
            int.TryParse(id, out _id);

            int _categoryId;
            int.TryParse(categoryId, out _categoryId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetUserMelodies request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No Token provided.");
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get User Melodies - Token: {0}", _token),
                        LogLevel.Trace);

                    IList<Melody> results = null;
                    Melody result = null;

                    var bll = new MelodyBLL();
                    if (id != null && !_id.Equals(default(int)))
                    {
                        // get melody by id
                        result = bll.GetMelody(new Melody { Id = _id }, _token);
                    }
                    else if (categoryId != null && !_categoryId.Equals(default(int)))
                    {
                        // get melodies by category
                        results = bll.GetUserMelodies(_token, new Category { Id = _categoryId });
                    }
                    else
                    {
                        // get all melodies
                        results = bll.GetUserMelodies(_token);
                    }

                    if (result != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                    else if (results != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, results);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedGetUserMelodies));
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
        /// Creates the user melody.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [HttpPut]
        [Route(Routes.RouteNew)]
        public HttpResponseMessage CreateUserMelody(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format(
                            "Create User Melody - FileName: {0}, Token: {1}.",
                            request.Melody != null ? request.Melody.FileName : "NULL",
                            request.Token),
                        LogLevel.Trace);

                    var bll = new MelodyBLL();
                    var result = bll.CreateUserMelody(request.Melody, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedCreateMelody);
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
                InstaMelodyLogger.Log("Received NULL CreateUserMelody request", LogLevel.Trace);
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
                    InstaMelodyLogger.Log(
                        string.Format(
                            "Delete User Melody - Id: {0}, FileName: {1}, Token: {2}.",
                            request.Melody != null ? request.Melody.Id.ToString() : "NULL",
                            request.Melody != null ? request.Melody.FileName : "NULL",
                            request.Token),
                        LogLevel.Trace);

                    var bll = new MelodyBLL();
                    bll.DeleteUserMelody(request.Melody, request.Token);

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

        [HttpPost]
        [HttpPut]
        [Route(Routes.RouteLoop)]
        public HttpResponseMessage CreateMelodyLoop(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route(Routes.RouteLoop)]
        public HttpResponseMessage GetMelodyLoop()
        {
            // TODO: 
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(Routes.RouteLoopAdd)]
        public HttpResponseMessage AttatchToMelodyLoop(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(Routes.RouteLoopUpdate)]
        public HttpResponseMessage UpdateMelodyLoop(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(Routes.RouteLoopDelete)]
        public HttpResponseMessage DeleteMelodyLoop(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }
    }
}

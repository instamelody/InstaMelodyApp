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
    [RoutePrefix(Routes.PrefixMessage01)]
    public class MessagesController : ApiController
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
        /// Gets the messages by user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteAll)]
        public HttpResponseMessage GetMessagesByUser(ApiRequest request)
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var token = nvc["token"];
            var userId = nvc["id"];
            var userName = nvc["displayName"];
            var userEmail = nvc["emailAddress"];
            var threaded = nvc["threaded"];

            if (string.IsNullOrEmpty(token)
                || (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(userEmail)))
            {
                response = token == null
                    ? this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation)
                    : this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullIncompleteMessage);
            }
            else
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Get Message For User - Token: {0}", token),
                        LogLevel.Trace);

                    Guid _userId;
                    Guid _token;
                    Guid.TryParse(userId, out _userId);
                    Guid.TryParse(token, out _token);

                    bool _threaded;
                    bool.TryParse(threaded, out _threaded);

                    var bll = new MessageBLL();
                    var results = bll.GetMessagesByUser(new User
                        {
                            Id = _userId,
                            DisplayName = userName,
                            EmailAddress = userEmail
                        }, _token, _threaded);

                    if (results != null && results.Any())
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, results);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, 
                            string.Format(Exceptions.FailedGetMessages, _token));
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
        /// Sends the message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPut]
        [HttpPost]
        [Route(Routes.RouteNew)]
        public HttpResponseMessage SendMessage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.UserMessage != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Send Message - Recipient Id: {0}, Token: {1}", 
                            request.UserMessage.RecipientId, request.Token),
                        LogLevel.Trace);

                    var bll = new MessageBLL();
                    var result = bll.SendMessageToUser(request.UserMessage.RecipientId, 
                        request.Message ?? request.UserMessage.Message, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedSendMessage, request.UserMessage.RecipientId));
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
                InstaMelodyLogger.Log("Received NULL SendMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullIncompleteMessage);
            }

            return response;
        }

        /// <summary>
        /// Reads the message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteRead)]
        public HttpResponseMessage ReadMessage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.UserMessage != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Read Message - UserMessageId: {0}, Token: {1}",
                            request.UserMessage.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new MessageBLL();
                    var result = bll.ReadMessage(request.UserMessage, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, 
                            string.Format(Exceptions.FailedReadMessage, request.UserMessage.Id));
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
                InstaMelodyLogger.Log("Received NULL ReadMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullIncompleteMessage);
            }

            return response;
        }

        /// <summary>
        /// Replies to message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteReply)]
        public HttpResponseMessage ReplyToMessage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.UserMessage != null 
                && request.Message != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Send Reply Message - Sender Id: {0}, Recipient Id: {1}, Token: {2}",
                            request.UserMessage.UserId, request.UserMessage.RecipientId, request.Token),
                        LogLevel.Trace);

                    var bll = new MessageBLL();
                    var result = bll.ReplyToMessage(request.UserMessage, request.Message, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedReplyMessage, 
                                (request.UserMessage == null 
                                || request.UserMessage.Message == null 
                                || request.UserMessage.Message.Id.Equals(default(Guid))) 
                                    ? request.Message.Id 
                                    : request.UserMessage.Message.Id));
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
                InstaMelodyLogger.Log("Received NULL or incomplete ReplyToMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullIncompleteMessage);
            }

            return response;
        }

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteDelete)]
        public HttpResponseMessage DeleteMessage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.UserMessage != null
                && request.Message != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Delete Message - Message Id: {0}, Token: {1}",
                            request.UserMessage.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new MessageBLL();
                    bll.DeleteUserMessage(request.UserMessage, request.Token);

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
                InstaMelodyLogger.Log("Received NULL or incomplete DeleteMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullIncompleteMessage);
            }

            return response;
        }

        public HttpResponseMessage SendPostToStation(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage SendMessageToStation(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage GetMessagesByStation(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage GetPostsByStation(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage DeletePost(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }
    }
}

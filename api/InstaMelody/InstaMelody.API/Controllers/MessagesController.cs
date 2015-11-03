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
        /// Creates the chat.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteChat)]
        public HttpResponseMessage CreateChat(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Create Chat - Token: {0}", request.Token),
                        LogLevel.Trace);

                    var bll = new MessageBll();

                    var chatName = request.Chat != null
                        ? request.Chat.Name
                        : null;

                    object result = null;
                    if (request.Users != null && request.Users.Count > 0)
                    {
                        result = bll.StartChat(request.Users, request.Message, request.Token, chatName);
                    }
                    else if (request.User != null)
                    {
                        result = bll.StartChat(request.User, request.Message, request.Token, chatName);
                    }

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedCreateChat);
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
            else
            {
                InstaMelodyLogger.Log("Received NULL CreateChat request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullChat);
            }

            return response;
        }

        /// <summary>
        /// Gets the chat.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteChat)]
        public HttpResponseMessage GetChat()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            Guid _chat;
            var chat = nvc["id"];
            Guid.TryParse(chat, out _chat);

            int _limit;
            var limit = nvc["limit"];
            int.TryParse(limit, out _limit);

            int _fromId;
            var fromId = nvc["fromId"];
            int.TryParse(fromId, out _fromId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetChat request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, 
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get Chat - Token: {0}, Chat: {1}", _token, _chat),
                        LogLevel.Trace);

                    var bll = new MessageBll();
                    object result;

                    if (_chat == default(Guid))
                    {
                        result = bll.GetAllUserChats(_token);
                    }
                    else
                    {
                        result = bll.GetChat(new Chat { Id = _chat }, _token, _limit, _fromId);
                    }

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
                        //response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, exc.Message);
                        response = this.Request.CreateResponse(HttpStatusCode.OK);
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
        /// Adds the user to chat.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteChatUser)]
        public HttpResponseMessage AddUserToChat(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Add User To Chat - Token: {0}, Chat: {1}, User: {2}", 
                            request.Token,
                            request.Chat.Id,
                            request.User.Id.Equals(default(Guid)) 
                                ? request.User.DisplayName 
                                : request.User.Id.ToString()),
                        LogLevel.Trace);

                    var bll = new MessageBll();
                    var result = bll.AddUserToChat(request.Chat, request.User, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedAddUserToChat);
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
            else
            {
                InstaMelodyLogger.Log("Received NULL AddUserToChat request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullChat);
            }

            return response;
        }

        /// <summary>
        /// Sends the chat message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteChatMessage)]
        public HttpResponseMessage SendChatMessage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Send Chat Message - Chat: {0}, Token: {1}", request.Chat.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new MessageBll();
                    var result = bll.SendChatMessage(request.Chat, request.Message, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedSendChatMessage);
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
            else
            {
                InstaMelodyLogger.Log("Received NULL SendChatMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullChat);
            }

            return response;
        }

        /// <summary>
        /// Gets the chat message.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteChatMessage)]
        public HttpResponseMessage GetChatMessage()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            Guid _chatId;
            var chatId = nvc["chatId"];
            Guid.TryParse(chatId, out _chatId);

            int _messageId;
            var messageId = nvc["messageId"];
            int.TryParse(messageId, out _messageId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetChatMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get Chat Message - Token: {0}, Chat: {1}, Message Id: {2}", 
                            _token, _chatId, _messageId),
                        LogLevel.Trace);

                    var bll = new MessageBll();
                    var results = bll.GetChatMessage(new Chat {Id = _chatId}, new ChatMessage {Id = _messageId}, _token);

                    if (results == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedGetChatMessage, _chatId, _messageId));
                    }
                    else
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, results);
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
        /// Removes the user from chat.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteChatRemove)]
        public HttpResponseMessage RemoveUserFromChat(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Remove User From Chat - Chat: {0}, Token: {1}", request.Chat.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new MessageBll();
                    bll.RemoveUserFromChat(request.Chat, request.Token);

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
                InstaMelodyLogger.Log("Received NULL RemoveUserFromChat request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullChat);
            }

            return response;
        }
    }
}

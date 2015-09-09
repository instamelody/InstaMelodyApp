using System;
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
    [RoutePrefix(Routes.PrefixUser01)]
    public class UsersController : ApiController
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
        /// Creates a user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPut]
        [HttpPost]
        [Route(Routes.RouteNew)]
        public HttpResponseMessage CreateUser(User request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format(
                            "Create User - User: {0}, Email: {1}.",
                            request.DisplayName ?? "NULL",
                            request.EmailAddress ?? "NULL"),
                        LogLevel.Trace);

                    var bll = new UserBll();
                    var result = bll.AddNewUser(request);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedNewUser);
                    }
                    else
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                    }

                }
                catch (Exception exc)
                {
                    if (exc is ArgumentException || exc is DataException)
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
                InstaMelodyLogger.Log("Received NULL CreateUser request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }

            return response;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteBlank)]
        public HttpResponseMessage GetUser()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var token = nvc["token"];
            var userId = nvc["id"];
            var userName = nvc["displayName"];
            var userEmail = nvc["emailAddress"];

            if (string.IsNullOrEmpty(token)
                || (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(userEmail)))
            {
                response = token == null 
                    ? this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation) 
                    : this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }
            else
            {
                try
                {
                    InstaMelodyLogger.Log(
                           string.Format(
                               "GetUser request - Token: {0}, User Id: {1}, Email Address: {2}, DisplayName: {3}.",
                               token ?? "NULL",
                               userId ?? "NULL",
                               userEmail ?? "NULL",
                               userName ?? "NULL"),
                           LogLevel.Trace);

                    var bll = new UserBll();
                    Guid _userId;
                    Guid _token;
                    Guid.TryParse(token, out _token);
                    Guid.TryParse(userId, out _userId);

                    var result = bll.GetUser(new User
                    {
                        Id = _userId,
                        DisplayName = userName,
                        EmailAddress = userEmail
                    }, _token);
                    if (result != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedFindUsers);
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
        /// Finds the user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteFind)]
        public HttpResponseMessage FindUser()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var token = nvc["token"];
            var userName = nvc["displayName"];
            var userEmail = nvc["emailAddress"];

            if (string.IsNullOrEmpty(token)
                || (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(userEmail)))
            {
                response = token == null
                    ? this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation)
                    : this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }
            else
            {
                try
                {
                    InstaMelodyLogger.Log(
                           string.Format(
                               "FindUser request - Token: {0}, Email Address: {2}, DisplayName: {3}.",
                               token ?? "NULL",
                               userEmail ?? "NULL",
                               userName ?? "NULL"),
                           LogLevel.Trace);

                    var bll = new UserBll();

                    Guid _token;
                    Guid.TryParse(token, out _token);

                    var result = bll.FindUser(new User
                    {
                        DisplayName = userName,
                        EmailAddress = userEmail
                    }, _token);

                    if (result != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedFindUsers);
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
        /// Updates the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUpdate)]
        public HttpResponseMessage UpdateUser(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Update User - Token: {0}", request.Token),
                        LogLevel.Trace);

                    if (request.Token.Equals(default(Guid)) || request.User == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedUpdateUser);
                        return response;
                    }

                    var bll = new UserBll();
                    var result = bll.UpdateUser(request.User, request.Token);

                    if (result == null || result.Equals(default(User)))
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedUpdateUser);
                    }
                    else
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                }
                catch (Exception exc)
                {
                    if (exc is ArgumentException || exc is UnauthorizedAccessException || exc is DataException)
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
                InstaMelodyLogger.Log("Received NULL UpdateUser request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }

            return response;
        }

        /// <summary>
        /// Updates the user profile image.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUpdateUserImage)]
        public HttpResponseMessage UpdateUserProfileImage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Update User Profile Image - Token: {0}", request.Token),
                        LogLevel.Trace);

                    var image = (request.User != null && request.User.Image != null)
                        ? request.User.Image
                        : request.Image;

                    if (request.Token.Equals(default(Guid)) || image == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedUpdateUser);
                        return response;
                    }

                    var bll = new UserBll();
                    var result = bll.UpdateUserImage(image, request.Token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedUpdateUser);
                    }
                    else
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                }
                catch (Exception exc)
                {
                    if (exc is ArgumentException || exc is UnauthorizedAccessException || exc is DataException)
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
                InstaMelodyLogger.Log("Received NULL UpdateUser request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }

            return response;
        }

        /// <summary>
        /// Deletes the user profile.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteDelete)]
        public HttpResponseMessage DeleteUserProfile(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Delete User - User: {0}", request.User.Id),
                        LogLevel.Trace);

                    if (request.Token.Equals(default(Guid)))
                    {
                        return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                    }

                    var bll = new UserBll();
                    bll.DeleteUser(request.User, request.Token);

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
                InstaMelodyLogger.Log("Received NULL DeleteUserProfile request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }

            return response;
        }

        /// <summary>
        /// Requests the friend.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteFriendRequest)]
        public HttpResponseMessage RequestFriend(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Request Friend - Friend: {0}, Token: {1}", request.User.Id, request.Token),
                        LogLevel.Trace);

                    if (request.Token.Equals(default(Guid)))
                    {
                        return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                    }

                    var bll = new UserBll();
                    var friend = bll.RequestFriend(request.User, request.Token);

                    if (friend != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, new ApiMessage
                        {
                            Message = string.Format("Friend request sent to {0}.", friend)
                        });
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedAddFriend);
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
                InstaMelodyLogger.Log("Received NULL RequestFriend request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }

            return response;
        }

        /// <summary>
        /// Approves the friend request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteFriendApprove)]
        public HttpResponseMessage ApproveFriendRequest(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Approve Friend Request - User: {0}, Token: {1}", request.User.Id, request.Token),
                        LogLevel.Trace);

                    if (request.Token.Equals(default(Guid)))
                    {
                        return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                    }

                    var bll = new UserBll();
                    var friend = bll.ApproveFriendRequest(request.User, request.Token);

                    if (friend != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, new ApiMessage
                        {
                            Message = string.Format("Requested friend {0} was approved.", friend.DisplayName)
                        });
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedApproveFriend);
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
                InstaMelodyLogger.Log("Received NULL ApproveFriendRequest request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }

            return response;
        }

        /// <summary>
        /// Denies the friend request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteFriendDeny)]
        public HttpResponseMessage DenyFriendRequest(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Deny Friend Request - User: {0}, Token: {1}", request.User.Id, request.Token),
                        LogLevel.Trace);

                    if (request.Token.Equals(default(Guid)))
                    {
                        return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                    }

                    var bll = new UserBll();
                    var friend = bll.DenyFriendRequest(request.User, request.Token);

                    if (!string.IsNullOrEmpty(friend))
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, new ApiMessage
                        {
                            Message = string.Format("Requested friend {0} was denied.", friend)
                        });
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedDenyFriend);
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
                InstaMelodyLogger.Log("Received NULL DenyFriendRequest request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }

            return response;
        }

        /// <summary>
        /// Deletes the friend.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteFriendDelete)]
        public HttpResponseMessage DeleteFriend(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Delete Friend - Friend: {0}, Token: {1}", request.User.Id, request.Token),
                        LogLevel.Trace);

                    if (request.Token.Equals(default(Guid)))
                    {
                        return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
                    }

                    var bll = new UserBll();
                    var friend = bll.DeleteFriend(request.User, request.Token);

                    if (!string.IsNullOrEmpty(friend))
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, new ApiMessage
                        {
                            Message = string.Format("Requested friend {0} was deleted.", friend)
                        });
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedDeleteFriend);
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
                InstaMelodyLogger.Log("Received NULL DeleteFriend request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullUser);
            }

            return response;
        }

        /// <summary>
        /// Gets the user friends.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteUserFriends)]
        public HttpResponseMessage GetUserFriends()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var token = nvc["token"];

            InstaMelodyLogger.Log(
                string.Format("Get Friends - Token: {0}", token),
                LogLevel.Trace);

            if (string.IsNullOrEmpty(token))
            {
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
            }
            else
            {
                try
                {
                    var bll = new UserBll();

                    Guid _token;
                    Guid.TryParse(token, out _token);
                    var friends = bll.GetFriendsByUser(_token);
                    response = this.Request.CreateResponse(HttpStatusCode.OK, friends);
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
        /// Gets the user friends.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RoutePendingFriends)]
        public HttpResponseMessage GetPendingUserFriends()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var token = nvc["token"];

            InstaMelodyLogger.Log(
                string.Format("Get Pending Friends - Token: {0}", token),
                LogLevel.Trace);

            if (string.IsNullOrEmpty(token))
            {
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
            }
            else
            {
                try
                {
                    var bll = new UserBll();

                    Guid _token;
                    Guid.TryParse(token, out _token);
                    var friends = bll.GetPendingFriendsByUser(_token);
                    response = this.Request.CreateResponse(HttpStatusCode.OK, friends);
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
        /// Adds the application purchase receipt.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [HttpPut]
        [Route(Routes.RouteAppReceipt)]
        public HttpResponseMessage AddAppPurchaseReceipt(ApiReceiptDataRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Add App Purchase Receipt - Receipt Data: {0}, Token: {1}", request.ReceiptData, request.Token),
                        LogLevel.Trace);

                    var bll = new UserBll();
                    var receipt = bll.CreateAppPurchaseReceipt(request.ReceiptData, request.Token);
                    if (receipt != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, receipt);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedCreateReceipt);
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
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL Receipt request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullReceipt);
            }

            return response;
        }

        /// <summary>
        /// Gets the application purchase receipts.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteAppReceipt)]
        public HttpResponseMessage GetAppPurchaseReceipts()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var token = nvc["token"];

            InstaMelodyLogger.Log(
                string.Format("Get App Purchase Receipt - Token: {0}", token),
                LogLevel.Trace);

            if (string.IsNullOrEmpty(token))
            {
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Exceptions.FailedValidation);
            }
            else
            {
                try
                {
                    Guid _token;
                    Guid.TryParse(token, out _token);

                    var bll = new UserBll();
                    var data = bll.GetAppPurchaseReceiptsForUser(_token);

                    response = this.Request.CreateResponse(HttpStatusCode.OK, data);
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
        /// Validates the application purchase receipt.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteValidateAppReceipt)]
        public HttpResponseMessage ValidateAppPurchaseReceipts(ApiReceiptDataRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    var bll = new UserBll();
                    object validation;
                    if (request.Receipt != null || !string.IsNullOrWhiteSpace(request.ReceiptData))
                    {
                        InstaMelodyLogger.Log(
                            string.Format("Validate App Purchase Receipt - Token: {0}, Receipt Id: {1}, Receipt Data: {2}", 
                                request.Token, 
                                request.Receipt != null ? request.Receipt.Id.ToString() : "NULL",
                                request.Receipt != null ? request.Receipt.ReceiptData : request.ReceiptData),
                            LogLevel.Trace);

                        var receipt = request.Receipt ?? new UserAppPurchaseReceipt
                        {
                            ReceiptData = request.ReceiptData
                        };

                        validation = bll.ValidateAppPurchaseReceipt(receipt, request.Token);
                    }
                    else
                    {
                        InstaMelodyLogger.Log(
                            string.Format("Validate App Purchase Receipt - Token: {0}", request.Token),
                            LogLevel.Trace);
                        validation = bll.ValidateAllAppPurchaseReceipts(request.Token);
                    }

                    if (validation != null)
                    {
                        response = this.Request.CreateResponse(HttpStatusCode.OK, validation);
                    }
                    else
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedValidateReceipt);
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
                InstaMelodyLogger.Log("Received NULL Receipt request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullReceipt);
            }

            return response;
        }

        /// <summary>
        /// Deletes the application purchase receipt.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteDeleteAppReceipt)]
        public HttpResponseMessage DeleteAppPurchaseReceipt(ApiReceiptDataRequest request)
        {
            HttpResponseMessage response;

            if (request != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Delete App Purchase Receipt - Token: {0}, Receipt Id: {1}, Receipt Data: {2}",
                            request.Token,
                            request.Receipt != null ? request.Receipt.Id.ToString() : "NULL",
                            request.Receipt != null ? request.Receipt.ReceiptData : request.ReceiptData),
                        LogLevel.Trace);

                    var receipt = request.Receipt ?? new UserAppPurchaseReceipt
                    {
                        ReceiptData = request.ReceiptData
                    };

                    var bll = new UserBll();
                    bll.DeleteAppPurchaseReceipt(receipt, request.Token);

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
                InstaMelodyLogger.Log("Received NULL Receipt request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullReceipt);
            }

            return response;
        }
    }
}

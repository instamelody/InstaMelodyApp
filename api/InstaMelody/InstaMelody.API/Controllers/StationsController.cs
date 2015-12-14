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

namespace InstaMelody.API.Controllers
{
    [RoutePrefix(Routes.PrefixStation01)]
    public class StationsController : ApiController
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
                UnixDateAndTime = Infrastructure.Utilities.UnixTimestampFromDateTime(timeStamp)
            };

            var response = this.Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        /// <summary>
        /// Creates the station.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [HttpPut]
        [Route(Routes.RouteNew)]
        public HttpResponseMessage CreateStation(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Create Station - Station: {0}, Token: {1}",
                            request.Station.Name, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    var result = bll.CreateStation(request.Station, request.Token, request.Image, request.Categories);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedCreateStation);
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL CreateStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Updates the station.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUpdate)]
        public HttpResponseMessage UpdateStation(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Update Station - Station: {0}, Token: {1}",
                            request.Station.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    var result = bll.UpdateStation(request.Station, request.Token, request.Image, request.Categories);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedUpdateStation, request.Station.Id));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL UpdateStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Publishes the station.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RoutePublish)]
        public HttpResponseMessage PublishStation(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Publish Station - Station: {0}, Token: {1}",
                            station, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.PublishStation(request.Station, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedPublishStation, station));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL PublishStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Unpublishes the station.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUnpublish)]
        public HttpResponseMessage UnpublishStation(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Unpublish Station - Station: {0}, Token: {1}",
                            station, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.UnpublishStation(request.Station, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedUnpublishStation, station));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL UnpublishStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Deletes the station.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteDelete)]
        public HttpResponseMessage DeleteStation(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Delete Station - Station: {0}, Token: {1}", 
                            station, request.Token), 
                        LogLevel.Trace);

                    var bll = new StationBll();
                    bll.DeleteStation(request.Station, request.Token);

                    response = this.Request.CreateResponse(HttpStatusCode.Accepted);
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL DeleteStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Deletes the station categories.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteDeleteCategories)]
        public HttpResponseMessage DeleteStationCategories(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Delete Station Categories - Station: {0}, Token: {1}",
                            station, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.RemoveStationFromCategories(request.Station, request.Categories, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, 
                            string.Format(Exceptions.FailedRemoveCategories, station));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL DeleteStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Gets the stations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteBlank)]
        public HttpResponseMessage GetStations()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            int _stationId;
            var stationId = nvc["stationId"];
            int.TryParse(stationId, out _stationId);

            var stationName = nvc["name"];

            Guid _userId;
            var userId = nvc["userId"];
            Guid.TryParse(userId, out _userId);

            var userName = nvc["displayName"];
            var userEmail = nvc["email"];

            int _categoryId;
            var categoryId = nvc["categoryId"];
            int.TryParse(categoryId, out _categoryId);

            int _categoryParentId;
            var categoryParentId = nvc["categoryParentId"];
            int.TryParse(categoryParentId, out _categoryParentId);

            var categoryName = nvc["category"];

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetStations request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get Stations - Token: {0}", _token),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    object result;

                    if (!_stationId.Equals(default(int))
                        || (!string.IsNullOrWhiteSpace(stationName)
                            && !_userId.Equals(default(Guid))))
                    {
                        // get stations by station info
                        result = bll.FindStation(new Station { Id = _stationId, Name = stationName, UserId = _userId }, _token);
                    }
                    else if (!_userId.Equals(default(Guid)) 
                        || !string.IsNullOrWhiteSpace(userName) 
                        || !string.IsNullOrWhiteSpace(userEmail))
                    {
                        // get stations by user info
                        result = bll.GetStationsByUser(new User { Id = _userId, DisplayName = userName, EmailAddress = userEmail }, _token);
                    }
                    else if (!_categoryId.Equals(default(int))
                        || !string.IsNullOrWhiteSpace(categoryName))
                    {
                        // get stations by category info
                        result = bll.GetStationsByCategory(new Category { Id = _categoryId, Name = categoryName, ParentId = _categoryParentId }, _token);
                    }
                    else
                    {
                        // get stations by session user
                        result = bll.GetStationsByUser(_token);
                    }

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedGetStations);
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

            return response;
        }

        /// <summary>
        /// Gets the newest stations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteNewest)]
        public HttpResponseMessage GetNewestStations()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            int _limit;
            var limit = nvc["limit"];
            int.TryParse(limit, out _limit);

            int _catId;
            var catId = nvc["categoryId"];
            int.TryParse(catId, out _catId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetNewestStations request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get Newest Stations - Token: {0}", _token),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    var result = bll.GetNewestStations(_token, _limit, _catId);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedGetStations);
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

            return response;
        }

        /// <summary>
        /// Gets the top stations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteTop)]
        public HttpResponseMessage GetTopStations()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            int _limit;
            var limit = nvc["limit"];
            int.TryParse(limit, out _limit);

            int _catId;
            var catId = nvc["categoryId"];
            int.TryParse(catId, out _catId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetTopStations request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get Top Stations - Token: {0}", _token),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    var result = bll.GetTopStations(_token, _catId, _limit);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedGetStations);
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

            return response;
        }

        /// <summary>
        /// Gets all stations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteAll)]
        public HttpResponseMessage GetAllStations()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetAllStations request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get All Stations - Token: {0}", _token),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    var result = bll.GetAllStations(_token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedGetStations);
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

            return response;
        }

        /// <summary>
        /// Gets the station followers.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteFollowers)]
        public HttpResponseMessage GetStationFollowers()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            int _stationId;
            var stationId = nvc["id"];
            int.TryParse(stationId, out _stationId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetStationFollowers request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get StationFollowers - Token: {0}, StationId: {1}", _token, _stationId),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    var result = bll.GetStationFollowers(new Station {Id = _stationId}, _token);

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, 
                            string.Format(Exceptions.FailedGetStationFollowers, _stationId));
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

            return response;
        }

        /// <summary>
        /// Likes the station.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteLike)]
        public HttpResponseMessage LikeStation(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Like Station - Station: {0}, Token: {1}",
                            station, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.LikeUnlikeStation(request.Station, request.Token, isLike: true);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedLikeStation, station));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL LikeStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Unlikes the station.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUnlike)]
        public HttpResponseMessage UnlikeStation(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Unlike Station - Station: {0}, Token: {1}",
                            station, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.LikeUnlikeStation(request.Station, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedUnlikeStation, station));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL UnlikeStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Follows the station.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteFollow)]
        public HttpResponseMessage FollowStation(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Follow Station - Station: {0}, Token: {1}",
                            station, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.FollowStation(request.Station, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedFollowStation, station));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL FollowStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Unfollows the station.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUnfollow)]
        public HttpResponseMessage UnfollowStation(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Unfollow Station - Station: {0}, Token: {1}",
                            station, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.UnfollowStation(request.Station, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedUnfollowStation, station));
                    }
                    else
                    {
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL UnfollowStation request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Creates the station post.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [HttpPut]
        [Route(Routes.RoutePost)]
        public HttpResponseMessage CreateStationPost(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Create Station Post - Station: {0}, Token: {1}",
                            station, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.SendPostToStation(request.Station, request.Message, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedCreatePost, station));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL CreateStationPost request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Creates the station message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [HttpPut]
        [Route(Routes.RouteMessage)]
        public HttpResponseMessage CreateStationMessage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.Station != null)
            {
                try
                {
                    var station = request.Station.Id.Equals(default(int))
                        ? request.Station.Name
                        : request.Station.Id.ToString();
                    InstaMelodyLogger.Log(
                        string.Format("Create Station Message - Station: {0}, Token: {1}",
                            station, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.SendMessageToStation(request.Station, request.Message, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedSendMessage, station));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL CreateStationMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Creates the station message reply.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteReplyMessage)]
        [Route(Routes.RouteReplyPost)]
        public HttpResponseMessage CreateStationMessageReply(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.StationMessage != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Reply to Station Message - Station Message: {0}, Token: {1}",
                            request.StationMessage.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.ReplyToStationMessage(request.StationMessage, request.Message, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedReplyStationMessage, request.StationMessage.Id));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL CreateStationMessageReply request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Gets the station posts.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RoutePosts)]
        public HttpResponseMessage GetStationPosts()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            int _stationId;
            var stationId = nvc["id"];
            int.TryParse(stationId, out _stationId);

            int _postId;
            var postId = nvc["postId"];
            int.TryParse(postId, out _postId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetStationPosts request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get Station Posts - Token: {0}, StationId: {1}", _token, _stationId),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    object result;
                    if (_postId != default(int))
                    {
                        result = bll.GetStationMessage(new StationMessage { Id = _postId }, _token);
                    }
                    else
                    {
                        result = bll.GetStationPosts(new Station { Id = _stationId }, _token);
                    }

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedGetStationPosts, _stationId));
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

            return response;
        }

        /// <summary>
        /// Gets the station messages.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Routes.RouteMessages)]
        public HttpResponseMessage GetStationMessages()
        {
            HttpResponseMessage response;

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Guid _token;
            var token = nvc["token"];
            Guid.TryParse(token, out _token);

            int _stationId;
            var stationId = nvc["id"];
            int.TryParse(stationId, out _stationId);

            int _messageId;
            var messageId = nvc["messageId"];
            int.TryParse(messageId, out _messageId);

            if (_token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Received NULL GetStationMessages request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    Exceptions.FailedAuthentication);
            }
            else
            {
                try
                {
                    // Log call
                    InstaMelodyLogger.Log(
                        string.Format("Get Station Messages - Token: {0}, StationId: {1}", _token, _stationId),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    object result;
                    if (_messageId != default(int))
                    {
                        result = bll.GetStationMessage(new StationMessage { Id = _messageId }, _token);
                    }
                    else
                    {
                        result = bll.GetStationMessages(new Station { Id = _stationId }, _token);
                    }

                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedGetStationMessages, _stationId));
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

            return response;
        }

        /// <summary>
        /// Likes the station message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteLikeMessage)]
        public HttpResponseMessage LikeStationMessage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.StationMessage != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Like Station Message - Station Message: {0}, Token: {1}",
                            request.StationMessage.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.LikeStationMessage(request.StationMessage, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedLikePost, request.StationMessage.Id));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL LikeStationMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Unlikes the station message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUnlikeMessage)]
        public HttpResponseMessage UnlikeStationMessage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.StationMessage != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Unlike Station Message - Station Message: {0}, Token: {1}",
                            request.StationMessage.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();

                    var result = bll.UnlikeStationMessage(request.StationMessage, request.Token);
                    if (result == null)
                    {
                        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            string.Format(Exceptions.FailedUnlikePost, request.StationMessage.Id));
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL UnlikeStationMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }

        /// <summary>
        /// Deletes the station message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteDeleteMessage)]
        [Route(Routes.RouteDeletePost)]
        public HttpResponseMessage DeleteStationMessage(ApiRequest request)
        {
            HttpResponseMessage response;

            if (request != null && request.StationMessage != null)
            {
                try
                {
                    InstaMelodyLogger.Log(
                        string.Format("Delete Station Message - Station Message: {0}, Token: {1}",
                            request.StationMessage.Id, request.Token),
                        LogLevel.Trace);

                    var bll = new StationBll();
                    bll.DeleteStationMessage(request.StationMessage, request.Token);

                    response = this.Request.CreateResponse(HttpStatusCode.Accepted);
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
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);
                }
            }
            else
            {
                InstaMelodyLogger.Log("Received NULL DeleteStationMessage request", LogLevel.Trace);
                response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.NullStations);
            }

            return response;
        }
    }
}

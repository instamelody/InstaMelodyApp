using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                UnixDateAndTime = Utilities.UnixTimestampFromDateTime(timeStamp)
            };

            var response = this.Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        public HttpResponseMessage CreateStation(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage UpdateStation(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage UpdateStationImage(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage DeleteStation(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage FollowStation(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage UnfollowStation(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage GetStationFollowers(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage GetAllStations(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage GetStationsByCategory(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage GetStationsByName(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }

        public HttpResponseMessage GetStationByUser(ApiRequest request)
        {
            // TODO: 
            throw new NotImplementedException();
        }
    }
}

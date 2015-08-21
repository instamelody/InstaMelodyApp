using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using InstaMelody.API.Constants;
using InstaMelody.Infrastructure;
using InstaMelody.Model.ApiModels;
using NLog;

namespace InstaMelody.API.Controllers
{
    [RoutePrefix("Test")]
    public class TestController : ApiController
    {
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

        [HttpGet]
        [Route("PushNotification")]
        public HttpResponseMessage PushNotification()
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var token = nvc["token"];
            //a6cf725b84e3de90cdabb63f3f05bbee8b953800ff767fa9ad56cd9a87bfd7f1

            if (string.IsNullOrEmpty(token))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "Please provide a token URL parameter.");
            }

            try
            {
                Business.Utilities.SendPushNotification(token);
                return Request.CreateResponse(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                InstaMelodyLogger.Log(string.Format("Failure sending push notification: {0}", ex.InnerException), LogLevel.Error);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("Email")]
        public HttpResponseMessage Email()
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var email = nvc["email"];

            if (string.IsNullOrEmpty(email))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "Please provide an email URL parameter with a valid email address.");
            }

            try
            {
                Business.Utilities.SendEmail(
                    email,
                    new MailAddress("noreply@instamelody.com", "InstaMelody"),
                    "InstaMelody Test Email",
                    "This is a test email from the API.");

                return Request.CreateResponse(HttpStatusCode.OK, string.Format("Email sent to {0}.", email));
            }
            catch (Exception ex)
            {
                InstaMelodyLogger.Log(string.Format("Failure sending email: {0}, {1}", ex.InnerException, ex.InnerException.InnerException), LogLevel.Error);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using InstaMelody.Business.Properties;
using InstaMelody.Model;
using InstaMelody.Data;
using InstaMelody.Infrastructure;
using InstaMelody.Model.Enums;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using LogLevel = NLog.LogLevel;

namespace InstaMelody.Business
{
    public static class Utilities
    {
        /// <summary>
        /// The get user by session.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="includeSensitiveInfo">if set to <c>true</c> [include sensitive information].</param>
        /// <returns>
        /// The <see cref="User" />.
        /// </returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.</exception>
        public static User GetUserBySession(Guid sessionId, bool includeSensitiveInfo = false)
        {
            User result = null;

            var dal = new UserSessions();
            var session = dal.FindByToken(sessionId);
            if (session != null)
            {
                var userDal = new Users();
                var findUser = userDal.FindById(session.UserId);
                if (findUser != null && !findUser.Id.Equals(default(Guid)))
                {
                    result = findUser;
                    result.DeviceToken = session.DeviceToken;
                    if (!includeSensitiveInfo)
                    {
                        result = result.StripSensitiveInfo();
                    }
                }
            }

            if (result != null) return result;

            InstaMelodyLogger.Log(string.Format("Could not validate session. Session Token: {0}", sessionId), LogLevel.Error);
            throw new UnauthorizedAccessException("Could not validate session.");
        }

        /// <summary>
        /// Ares the users friends.
        /// </summary>
        /// <param name="user1">The user1.</param>
        /// <param name="user2">The user2.</param>
        /// <returns></returns>
        public static bool AreUsersFriends(Guid user1, Guid user2)
        {
            var bll = new UserFriends();
            return bll.AreUsersFriends(user1, user2);
        }

        /// <summary>
        /// Updates the user object.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="withUserObject">The with user object.</param>
        /// <returns></returns>
        public static User UpdateUserObject(User user, User withUserObject)
        {
            var updatedUser = new User
            {
                Id = user.Id,
                EmailAddress = !string.IsNullOrWhiteSpace(withUserObject.EmailAddress)
                    ? withUserObject.EmailAddress
                    : user.EmailAddress,
                DisplayName = !string.IsNullOrWhiteSpace(withUserObject.DisplayName)
                    ? withUserObject.DisplayName
                    : user.DisplayName,
                FirstName = !string.IsNullOrWhiteSpace(withUserObject.FirstName)
                    ? withUserObject.FirstName
                    : user.FirstName,
                LastName = !string.IsNullOrWhiteSpace(withUserObject.LastName)
                    ? withUserObject.LastName
                    : user.LastName,
                PhoneNumber = !string.IsNullOrWhiteSpace(withUserObject.PhoneNumber)
                    ? withUserObject.PhoneNumber
                    : user.PhoneNumber,
                TwitterUsername = !string.IsNullOrWhiteSpace(withUserObject.TwitterUsername)
                    ? withUserObject.TwitterUsername
                    : user.TwitterUsername,
                TwitterUserId = !string.IsNullOrWhiteSpace(withUserObject.TwitterUserId)
                    ? withUserObject.TwitterUserId
                    : user.TwitterUserId,
                TwitterToken = !string.IsNullOrWhiteSpace(withUserObject.TwitterToken)
                    ? withUserObject.TwitterToken
                    : user.TwitterToken,
                TwitterSecret = !string.IsNullOrWhiteSpace(withUserObject.TwitterSecret)
                    ? withUserObject.TwitterSecret
                    : user.TwitterSecret,
                FacebookUserId = !string.IsNullOrWhiteSpace(withUserObject.FacebookUserId)
                    ? withUserObject.FacebookUserId
                    : user.FacebookUserId,
                FacebookToken = !string.IsNullOrWhiteSpace(withUserObject.FacebookToken)
                    ? withUserObject.FacebookToken
                    : user.FacebookToken,
                DateModified = DateTime.UtcNow
            };

            return updatedUser;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="fileContentMediaType">Type of the file content media.</param>
        /// <returns></returns>
        public static string GetFilePath(string mimeType, string fileContentMediaType)
        {
            switch (mimeType)
            {
                // images
                case "image/gif":
                case "image/jpeg":
                case "image/pjpeg":
                case "image/png":
                case "image/x-png":
                case "image/bmp":
                case "image/x-xbitmap":
                    return string.Format("{0}/{1}",
                        Settings.Default.BaseFileUploadFolder,
                        Settings.Default.ImageFileUploadFolder);

                // audio
                case "audio/x-aiff":
                case "audio/basic":
                case "audio/mid":
                case "audio/wav":
                    return string.Format("{0}/{1}",
                        Settings.Default.BaseFileUploadFolder,
                        Settings.Default.AudioFileUploadFolder);
                case "application/octet-stream":
                    if (fileContentMediaType.Equals("audio/mp3")
                        || fileContentMediaType.Equals("audio/x-m4a")
                        || fileContentMediaType.Equals("audio/m4a"))
                    {
                        return string.Format("{0}/{1}",
                            Settings.Default.BaseFileUploadFolder,
                            Settings.Default.AudioFileUploadFolder);
                    }
                    break;

                // video
                case "video/avi":
                case "video/mpeg":
                    return string.Format("{0}/{1}",
                         Settings.Default.BaseFileUploadFolder,
                         Settings.Default.VideoFileUploadFolder);

            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns></returns>
        public static string GetFilePath(string fileName, MediaTypeEnum mediaType)
        {
            var mediaPath = string.Empty;
            switch (mediaType)
            {
                case MediaTypeEnum.Image:
                    mediaPath = Settings.Default.ImageFileUploadFolder;
                    break;
                case MediaTypeEnum.Video:
                    mediaPath = Settings.Default.VideoFileUploadFolder;
                    break;
                case MediaTypeEnum.Melody:
                    mediaPath = Settings.Default.AudioFileUploadFolder;
                    break;
            }
            return string.Format("{0}/{1}/{2}", 
                Settings.Default.BaseFileUploadFolder,
                mediaPath, fileName);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="toAddress">To address.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        public static void SendEmail(string toAddress, MailAddress fromAddress, string subject, string body, bool isHtml = true)
        {
            using (var message = new MailMessage())
            {
                message.To.Add(toAddress);
                message.From = fromAddress;
                message.Subject = subject;
                message.IsBodyHtml = isHtml;
                message.Body = body;

                using (var cli = new SmtpClient())
                {
                    cli.Host = Settings.Default.SMTPMailHost;
                    cli.Send(message);
                }
            }
        }

        public static void SendPushNotification(Guid userId)
        {
            var token = GetDeviceTokens(new List<Guid> {userId});
            if (token == null || !token.Any())
            {
                return;
            }

            SendApplePushNotification(token.First());
        }

        public static void SendPushNotification(IList<Guid> userIds)
        {
            var tokens = GetDeviceTokens(userIds);
            foreach (var token in tokens)
            {
                SendApplePushNotification(token);
            }
        }

        private static IList<string> GetDeviceTokens(IEnumerable<Guid> userIds)
        {
            var results = new List<string>();

            var dal = new UserSessions();
            foreach (var id in userIds)
            {
                var sessions = dal.FindActiveSessions(id);
                if (sessions == null || !sessions.Any())
                {
                    continue;
                }

                results.AddRange(
                    from userSession in sessions 
                    where !string.IsNullOrWhiteSpace(userSession.DeviceToken) 
                    select userSession.DeviceToken);
            }

            return results;
        }

        #region APNS Private Methods

        private static void SendApplePushNotification(string deviceToken)
        {
            // TODO: setup push notifications
            var push = new PushBroker();

            //Wire up the events for all the services that the broker registers
            push.OnNotificationSent += NotificationSent;
            push.OnChannelException += ChannelException;
            push.OnServiceException += ServiceException;
            push.OnNotificationFailed += NotificationFailed;
            push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            push.OnChannelCreated += ChannelCreated;
            push.OnChannelDestroyed += ChannelDestroyed;


            //-------------------------
            // APPLE NOTIFICATIONS
            //-------------------------
            //Configure and start Apple APNS
            // IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to generate one for connecting to Sandbox,
            //   and one for connecting to Production.  You must use the right one, to match the provisioning profile you build your
            //   app with!
            // TODO: setup certificates and add to project. Call certificates here
#if DEBUG

#else

#endif
            var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources/PushSharp.Apns.Sandbox.p12"));
            //IMPORTANT: If you are using a Development provisioning Profile, you must use the Sandbox push notification server 
            //  (so you would leave the first arg in the ctor of ApplePushChannelSettings as 'false')
            //  If you are using an AdHoc or AppStore provisioning profile, you must use the Production push notification server
            //  (so you would change the first arg in the ctor of ApplePushChannelSettings to 'true')
            // TODO: get certificate password and setup in BLL settings file
            push.RegisterAppleService(new ApplePushChannelSettings(appleCert, "CERTIFICATE PASSWORD HERE")); //Extension method
            //Fluent construction of an iOS notification
            //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets generated within your iOS app itself when the Application Delegate
            //  for registered for remote notifications is called, and the device token is passed back to you
            // TODO: setup push notification to send correct data
            push.QueueNotification(new AppleNotification()
                                       .ForDeviceToken(deviceToken)
                                       .WithAlert("Hello World!")
                                       .WithBadge(7)
                                       .WithSound("sound.caf"));
        }

        private static void NotificationSent(object sender, INotification notification)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Sent: {0} -> {1}", 
                    sender, 
                    notification), 
                LogLevel.Trace);
        }

        private static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Failure: {0} -> {1} -> {2}", 
                    sender, 
                    notificationFailureException.Message, 
                    notification), 
                LogLevel.Error);
        }

        private static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Channel Exception: {0} -> {1}",
                    sender,
                    exception),
                LogLevel.Error);
        }

        private static void ServiceException(object sender, Exception exception)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Service Exception: {0} -> {1}",
                    sender,
                    exception),
                LogLevel.Error);
        }

        private static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Device Subscription Expired: {0} -> {1}",
                    sender,
                    expiredDeviceSubscriptionId),
                LogLevel.Error);

            // TODO: delete device token from session
        }

        private static void ChannelDestroyed(object sender)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Channel Destroyed for: {0}",
                    sender),
                LogLevel.Error);
        }

        private static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Channel Created for: {0}",
                    sender),
                LogLevel.Error);
        } 

        #endregion APNS Private Methods
    }
}

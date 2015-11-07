using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using InstaMelody.Business.Properties;
using InstaMelody.Model;
using InstaMelody.Data;
using InstaMelody.Infrastructure;
using InstaMelody.Infrastructure.Enums;
using InstaMelody.Model.Enums;
using Newtonsoft.Json.Linq;
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
        /// Gets the user activity.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="afterDateTime">The after date time.</param>
        /// <param name="fetchActivityForFriends">if set to <c>true</c> [fetch activity for friends].</param>
        /// <returns></returns>
        public static IList<Model.UserActivity> GetUserActivity(Guid sessionToken, bool fetchActivityForFriends, DateTime? afterDateTime = null)
        {
            // TODO: wire this method to API layer
            var sessionUser = GetUserBySession(sessionToken);

            var dal = new Data.UserActivity();
            IList<Model.UserActivity> activity;

            if (fetchActivityForFriends)
            {
                var friendsDal = new UserFriends();
                var friends = friendsDal.GetUserFriends(sessionUser.Id);
                var friendIds = friends.Select(f => f.Id).ToList();

                activity = (afterDateTime != null && afterDateTime > DateTime.MinValue)
                    ? dal.GetActivity(friendIds, (DateTime)afterDateTime)
                    : dal.GetActivity(friendIds);
            }
            else
            {
                // fetch activity for self;
                activity = (afterDateTime != null && afterDateTime > DateTime.MinValue)
                    ? dal.GetActivity(sessionUser.Id, (DateTime)afterDateTime) 
                    : dal.GetActivity(sessionUser.Id);
            }

            if (activity == null) return activity;

            foreach (var userActivity in activity)
            {
                userActivity.Activity = FormatUserActivity(userActivity);
            }

            return activity;
        }

        /// <summary>
        /// Formats the user activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <returns></returns>
        private static string FormatUserActivity(Model.UserActivity activity)
        {
            if (activity == null) return string.Empty;

            var activityMessage = string.Empty;
            switch(activity.ActivityType)
            {
                case ActivityTypeEnum.Friend:
                    activityMessage = Constants.ActivityFriend;
                    break;

                case ActivityTypeEnum.StationLike:
                    activityMessage = Constants.ActivityStationLike;
                    break;

                case ActivityTypeEnum.StationMessageUserLike:
                    activityMessage = Constants.ActivityStationMessageUserLike;
                    break;

                case ActivityTypeEnum.StationPost:
                    activityMessage = Constants.ActivityStationPost;
                    break;

                case ActivityTypeEnum.StationPostReply:
                    activityMessage = Constants.ActivityStationPostReply;
                    break;
            }

            return string.Format(activityMessage,
                activity.UserDisplayName ?? activity.UserId.ToString(),
                activity.EntityName);
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
            try
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
                        cli.Port = 25;
                        cli.EnableSsl = false;
                        cli.DeliveryMethod = SmtpDeliveryMethod.Network;
                        cli.Send(message);
                    }
                }
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failure sending email. To: {0}, From: {1}, Subject: {2}, Body: {3}, Host: {4}",
                        toAddress, fromAddress.Address, subject, body, Settings.Default.SMTPMailHost), 
                    LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Sends the push notification.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        public static void SendPushNotification(string deviceToken)
        {
            SendApplePushNotification(deviceToken);
        }

        /// <summary>
        /// Sends the push notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        public static void SendPushNotification(Guid userId, APNSTypeEnum type, params object[] data)
        {
            var token = GetDeviceTokens(new List<Guid> {userId});
            if (token == null || !token.Any())
            {
                return;
            }

            var key = Infrastructure.Utilities.GetApnsTypeString(type);
            SendApplePushNotification(token.First(), key, data);
            InstaMelodyLogger.Log(string.Format("Push notification triggered: Token: {0}, Type: {1}", 
                token, key), LogLevel.Trace);
        }

        /// <summary>
        /// Sends the push notification.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        public static void SendPushNotification(IList<Guid> userIds, APNSTypeEnum type, params object[] data)
        {
            var tokens = GetDeviceTokens(userIds);
            var key = Infrastructure.Utilities.GetApnsTypeString(type);
            foreach (var token in tokens)
            {
                SendApplePushNotification(token, key, data);
                InstaMelodyLogger.Log(string.Format("Push notification triggered: Token: {0}, Type: {1}",
                    token, key), LogLevel.Trace);
            }
        }

        /// <summary>
        /// Sends the push notification.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <param name="senderDisplayName">Display name of the sender.</param>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        public static void SendPushNotification(IList<Guid> userIds, string senderDisplayName, APNSTypeEnum type, params object[] data)
        {
            var tokens = GetDeviceTokens(userIds);
            var key = Infrastructure.Utilities.GetApnsTypeString(type);
            var alert = Infrastructure.Utilities.GetApnsAlertString(type, senderDisplayName);
            foreach (var token in tokens)
            {
                if (alert == null)
                {
                    SendApplePushNotification(token, key, data);
                }
                else
                {
                    SendApplePushNotification(token, alert, key, data);
                }
                InstaMelodyLogger.Log(string.Format("Push notification triggered: Token: {0}, Type: {1}",
                    token, key), LogLevel.Trace);
            }
        }

        /// <summary>
        /// Sends the push notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="senderDisplayName">Display name of the sender.</param>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        public static void SendPushNotification(Guid userId, string senderDisplayName, APNSTypeEnum type, params object[] data)
        {
            var token = GetDeviceTokens(new List<Guid> { userId });
            if (token == null || !token.Any())
            {
                return;
            }

            var key = Infrastructure.Utilities.GetApnsTypeString(type);
            var alert = Infrastructure.Utilities.GetApnsAlertString(type, senderDisplayName);
            if (alert == null)
            {
                SendApplePushNotification(token.First(), key, data);
            }
            else
            {
                SendApplePushNotification(token.First(), alert, key, data);
            }
            InstaMelodyLogger.Log(string.Format("Push notification triggered: Token: {0}, Type: {1}",
                token, key), LogLevel.Trace);
        }

        /// <summary>
        /// Gets the device tokens.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sends the apple push notification.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        private static void SendApplePushNotification(string deviceToken, string key, params object[] values)
        {
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

#if DEBUG
            var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.APNSDevCertificate));
            push.RegisterAppleService(new ApplePushChannelSettings(appleCert, Settings.Default.APNSDevCertPassword));
#else
            var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.APNSProdCertificate));
            push.RegisterAppleService(new ApplePushChannelSettings(appleCert, Settings.Default.APNSProdCertPassword));
#endif

            //Fluent construction of an iOS notification
            //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets generated within your iOS app itself when the Application Delegate
            //  for registered for remote notifications is called, and the device token is passed back to you

            var alert = string.Empty;

            var notification = new AppleNotification()
                                .ForDeviceToken(deviceToken)
                                .WithCustomItem(key, values);

            push.QueueNotification(notification);
        }

        private static void SendApplePushNotification(string deviceToken, string alert, string key, params object[] values)
        {
            var push = new PushBroker();

            push.OnNotificationSent += NotificationSent;
            push.OnChannelException += ChannelException;
            push.OnServiceException += ServiceException;
            push.OnNotificationFailed += NotificationFailed;
            push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            push.OnChannelCreated += ChannelCreated;
            push.OnChannelDestroyed += ChannelDestroyed;

#if DEBUG
            var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.APNSDevCertificate));
            push.RegisterAppleService(new ApplePushChannelSettings(appleCert, Settings.Default.APNSDevCertPassword));
#else
            var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.APNSProdCertificate));
            push.RegisterAppleService(new ApplePushChannelSettings(appleCert, Settings.Default.APNSProdCertPassword));
#endif

            var notification = new AppleNotification()
                                .ForDeviceToken(deviceToken)
                                .WithCustomItem(key, values)
                                .WithAlert(alert);

            push.QueueNotification(notification);
        }

        /// <summary>
        /// Sends the apple push notification.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        private static void SendApplePushNotification(string deviceToken)
        {
            var push = new PushBroker();
            push.OnNotificationSent += NotificationSent;
            push.OnChannelException += ChannelException;
            push.OnServiceException += ServiceException;
            push.OnNotificationFailed += NotificationFailed;
            push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            push.OnChannelCreated += ChannelCreated;
            push.OnChannelDestroyed += ChannelDestroyed;

#if DEBUG
            var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.APNSDevCertificate));
            push.RegisterAppleService(new ApplePushChannelSettings(appleCert, Settings.Default.APNSDevCertPassword));
#else
            var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.APNSProdCertificate));
            push.RegisterAppleService(new ApplePushChannelSettings(appleCert, Settings.Default.APNSProdCertPassword));
#endif
            push.QueueNotification(new AppleNotification()
                                       .ForDeviceToken(deviceToken)
                                       .WithAlert("Hello From InstaMelody API BLL!"));
        }

        /// <summary>
        /// Notifications the sent.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="notification">The notification.</param>
        private static void NotificationSent(object sender, INotification notification)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Sent: {0} -> {1}", 
                    sender, 
                    notification), 
                LogLevel.Trace);
        }

        /// <summary>
        /// Notifications the failed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="notificationFailureException">The notification failure exception.</param>
        private static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Failure: {0} -> {1} -> {2}", 
                    sender, 
                    notificationFailureException.Message, 
                    notification), 
                LogLevel.Error);
        }

        /// <summary>
        /// Channels the exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="exception">The exception.</param>
        private static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Channel Exception: {0} -> {1}",
                    sender,
                    exception),
                LogLevel.Error);
        }

        /// <summary>
        /// Services the exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="exception">The exception.</param>
        private static void ServiceException(object sender, Exception exception)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Service Exception: {0} -> {1}",
                    sender,
                    exception),
                LogLevel.Error);
        }

        /// <summary>
        /// Devices the subscription expired.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="expiredDeviceSubscriptionId">The expired device subscription identifier.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="notification">The notification.</param>
        private static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Device Subscription Expired: {0} -> {1}",
                    sender,
                    expiredDeviceSubscriptionId),
                LogLevel.Error);
        }

        /// <summary>
        /// Channels the destroyed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private static void ChannelDestroyed(object sender)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Channel Destroyed for: {0}",
                    sender),
                LogLevel.Error);
        }

        /// <summary>
        /// Channels the created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="pushChannel">The push channel.</param>
        private static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            InstaMelodyLogger.Log(
                string.Format("APNS Channel Created for: {0}",
                    sender),
                LogLevel.Trace);
        } 

        #endregion APNS Private Methods
    }
}

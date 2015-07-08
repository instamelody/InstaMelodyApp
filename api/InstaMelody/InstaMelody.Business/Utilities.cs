using System;
using InstaMelody.Business.Properties;
using InstaMelody.Model;
using InstaMelody.Data;
using InstaMelody.Model.Enums;

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
                    if (!includeSensitiveInfo)
                    {
                        result = result.StripSensitiveInfo();
                    }
                }
            }

            if (result == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            return result;
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
    }
}

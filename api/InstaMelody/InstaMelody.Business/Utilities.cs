using System;
using InstaMelody.Business.Properties;
using InstaMelody.Model;
using InstaMelody.Data;

namespace InstaMelody.Business
{
    public static class Utilities
    {
        /// <summary>
        /// The get user by session.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        public static User GetUserBySession(Guid sessionId)
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
                    result = result.StripSensitiveInfo();
                }
            }

            return result;
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
    }
}

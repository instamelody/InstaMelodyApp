using System;
using System.Data;
using InstaMelody.Business.Properties;
using InstaMelody.Data;
using InstaMelody.Infrastructure;
using InstaMelody.Model;
using InstaMelody.Model.Enums;
using NLog;

namespace InstaMelody.Business
{
    public class FileBll
    {
        /// <summary>
        /// Determines whether a file can be accepted to be uploaded.
        /// If an exception is thrown within this function the file upload token is automatically deleted.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="fileUploadToken">The file upload token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.
        /// or
        /// File Upload Token has expired.
        /// or
        /// File Upload Token does not belong to this User.</exception>
        public bool CanUploadFile(Guid sessionToken, Guid fileUploadToken)
        {
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                ExpireToken(fileUploadToken);
                throw;
            }
            

            var token = GetTokenInfo(fileUploadToken);
            if (token.IsExpired)
            {
                ExpireToken(fileUploadToken);
                InstaMelodyLogger.Log(
                    string.Format("File Upload Token has expired. Session Token: {0}, Upload Token: {1}",
                        sessionToken, fileUploadToken), LogLevel.Error);
                throw new UnauthorizedAccessException("File Upload Token has expired.");
            }

            if (!sessionUser.Id.Equals(token.UserId))
            {
                ExpireToken(fileUploadToken);
                InstaMelodyLogger.Log(string.Format("File Upload Token does not belong to this User. Session Token: {0}, Upload Token: {1}",
                        sessionToken, fileUploadToken), LogLevel.Error);
                throw new UnauthorizedAccessException("File Upload Token does not belong to this User.");
            }

            return true;
        }

        /// <summary>
        /// Determines whether the file being uploaded matches the token file name.
        /// If an exception is thrown within this function the file upload token is automatically deleted.
        /// </summary>
        /// <param name="fileUploadToken">The file upload token.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// File Upload Token has expired.
        /// or
        /// The transmitted file name does not match the token file name.
        /// </exception>
        public bool IsValidUploadFileName(Guid fileUploadToken, string fileName)
        {
            var token = GetTokenInfo(fileUploadToken);
            return token != null && token.FileName.Equals(fileName);
        }

        /// <summary>
        /// Deletes the associated file information.
        /// </summary>
        /// <param name="token">The token.</param>
        public void DeleteAssociatedFileInfo(Guid token)
        {
            var dal = new FileUploadTokens();
            var foundToken = dal.GetTokenDetails(token);
            if (foundToken == null)
            {
                InstaMelodyLogger.Log(string.Format("Token cannot be found. Token: {0}", token), LogLevel.Error);
                throw new ArgumentException("Token cannot be found.");
            }

            switch (foundToken.MediaType)
            {
                case FileUploadTypeEnum.MessageImage:
                    DeleteMessageImageRecords(foundToken.FileName);
                    break;
                case FileUploadTypeEnum.MessageMelody:
                    DeleteMessageMelodyRecords(foundToken.FileName);
                    break;
                case FileUploadTypeEnum.MessageVideo:
                    DeleteMessageVideoRecords(foundToken.FileName);
                    break;
                case FileUploadTypeEnum.UserImage:
                    DeleteUserImageRecords(foundToken.UserId, foundToken.FileName);
                    break;
            }
        }

        /// <summary>
        /// Creates the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">
        /// File name and Media Type must be defined to create a token.
        /// or
        /// Failed to create a File Upload Token.
        /// </exception>
        public FileUploadToken CreateToken(FileUploadToken token)
        {
            if (token.UserId.Equals(default(Guid))
                ||string.IsNullOrWhiteSpace(token.FileName)
                || token.MediaType.Equals(default(FileUploadTypeEnum))
                || token.MediaType.Equals(FileUploadTypeEnum.Unknown))
            {
                InstaMelodyLogger.Log(
                    string.Format("User Id, File Name, and Media Type must be defined to create a token. User Id: {0}, File Name: {1}, Media Type: {2}",
                        token.UserId, token.FileName, token.MediaType), LogLevel.Error);
                throw new ArgumentException("User Id, File Name, and Media Type must be defined to create a token.");
            }

            var dal = new FileUploadTokens();
            var expires = DateTime.UtcNow.AddMinutes(Settings.Default.MinutesFileUploadTokenIsActive);
            var createdToken = dal.CreateToken(token.UserId, token.FileName, token.MediaType, expires);
            if (createdToken == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to create a File Upload Token. User Id: {0}, File Name: {1}, Media Type: {2}",
                        token.UserId, token.FileName, token.MediaType), LogLevel.Error);
                throw new DataException("Failed to create a File Upload Token.");
            }

            return createdToken;
        }

        /// <summary>
        /// Gets the token information.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">A valid token Guid must be provided.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public FileUploadToken GetTokenInfo(Guid token)
        {
            if (token.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("GetTokenInfo() token Guid is default.", LogLevel.Error);
                throw new ArgumentException("A valid token Guid must be provided.");
            }

            var dal = new FileUploadTokens();
            var foundToken = dal.GetTokenDetails(token);
            if (foundToken == null || foundToken.IsDeleted)
            {
                InstaMelodyLogger.Log(string.Format("No valid token found. Token: {0}", token), LogLevel.Error);
                throw new DataException(string.Format("No token found with Guid {0}.", token));
            }

            return foundToken;
        }

        /// <summary>
        /// Expires the token.
        /// </summary>
        /// <param name="token">The token.</param>
        public void ExpireToken(Guid token)
        {
            var dal = new FileUploadTokens();
            dal.ExpireToken(token);
        }

        /// <summary>
        /// Expires the token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mediaType">Type of the media.</param>
        public void ExpireToken(Guid userId, string fileName, MediaTypeEnum mediaType)
        {
            var dal = new FileUploadTokens();
            var foundToken = dal.FindToken(userId, fileName, mediaType);
            if (foundToken != null)
            {
                ExpireToken(foundToken.Token);
            }
        }

        #region Images

        /// <summary>
        /// Adds the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">
        /// Cannot create image with empty File Name.
        /// or
        /// Cannot add new image with existing File Name.
        /// or
        /// Failed to create Image.
        /// </exception>
        public Image AddImage(Image image)
        {
            if (string.IsNullOrWhiteSpace(image.FileName))
            {
                InstaMelodyLogger.Log("Cannot create Image with empty File Name.", LogLevel.Error);
                throw new ArgumentException("Cannot create Image with empty File Name.");
            }

            if (DoesImageExistWithFileName(image.FileName))
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot add new Image with existing File Name. File Name: {0}", 
                        image.FileName), LogLevel.Error);
                throw new DataException("Cannot add new Image with existing File Name.");
            }

            var dal = new Images();
            image.DateCreated = DateTime.UtcNow;
            var createdImage = dal.CreateImage(image);
            if (createdImage == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to create Image. File Name: {0}", 
                        image.FileName), LogLevel.Error);
                throw new DataException("Failed to create Image.");
            }

            return createdImage;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">No Image Id or File name provided</exception>
        public Image GetImage(Image image)
        {
            Image result;

            if (image.Id.Equals(default(int)) && string.IsNullOrWhiteSpace(image.FileName))
            {
                InstaMelodyLogger.Log("No Image Id or File name provided. for GetImage()", LogLevel.Error);
                throw new ArgumentException("No Image Id or File name provided.");
            }

            var dal = new Images();
            if (!image.Id.Equals(default(int)))
            {
                result = dal.GetImageById(image.Id);
            }
            else
            {
                result = dal.GetImageByFileName(image.FileName);
            }

            return result;
        }

        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <exception cref="System.Data.DataException">
        /// No Image Id provided.
        /// or
        /// Failed to delete image.
        /// </exception>
        public int DeleteImage(Image image)
        {
            var dal = new Images();
            var foundImage = GetImage(image);
            if (foundImage == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find image to delete. Image Id: {0}, File Name: {1}", 
                        image.Id, image.FileName), LogLevel.Error);
                throw new ArgumentException("Could not find image to delete.");
            }

            dal.DeleteImage(foundImage.Id);

            var getImage = dal.GetImageById(foundImage.Id);
            if (getImage != null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to delete image. Image Id: {0}", 
                        foundImage.Id), LogLevel.Error);
                throw new DataException("Failed to delete image.");
            }

            return foundImage.Id;
        }

        /// <summary>
        /// Checks for duplicate records with the name provided.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private bool DoesImageExistWithFileName(string fileName)
        {
            var dal = new Images();
            var existing = dal.GetImageByFileName(fileName);

            return (existing != null);
        }

        #endregion Images

        #region Videos

        /// <summary>
        /// Adds the video.
        /// </summary>
        /// <param name="video">The video.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Cannot create video with empty File Name.</exception>
        /// <exception cref="System.Data.DataException">
        /// Cannot add new video with existing File Name.
        /// or
        /// Failed to create Image.
        /// </exception>
        public Video AddVideo(Video video)
        {
            if (string.IsNullOrWhiteSpace(video.FileName))
            {
                InstaMelodyLogger.Log("Cannot create Video with empty File Name.", LogLevel.Error);
                throw new ArgumentException("Cannot create Video with empty File Name.");
            }

            if (DoesVideoExistWithFileName(video.FileName))
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot add new Video with existing File Name. File Name: {0}",
                        video.FileName), LogLevel.Error);
                throw new DataException("Cannot add new Video with existing File Name.");
            }

            var dal = new Videos();
            video.DateCreated = DateTime.UtcNow;
            var createdVideo = dal.CreateVideo(video);
            if (createdVideo == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to create Video. File Name: {0}",
                        video.FileName), LogLevel.Error);
                throw new DataException("Failed to create Video.");
            }

            return createdVideo;
        }

        /// <summary>
        /// Gets the video.
        /// </summary>
        /// <param name="video">The video.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">No Video Id or File name provided.</exception>
        public Video GetVideo(Video video)
        {
            Video result;

            if (video.Id.Equals(default(int)) && string.IsNullOrWhiteSpace(video.FileName))
            {
                InstaMelodyLogger.Log("No Video Id or File name provided for GetVideo().", LogLevel.Error);
                throw new ArgumentException("No Video Id or File name provided.");
            }

            var dal = new Videos();
            if (!video.Id.Equals(default(int)))
            {
                result = dal.GetVideoById(video.Id);
            }
            else
            {
                result = dal.GetVideoByFileName(video.FileName);
            }

            return result;
        }

        /// <summary>
        /// Deletes the video.
        /// </summary>
        /// <param name="video">The video.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Could not find video to delete.</exception>
        /// <exception cref="System.Data.DataException">Failed to delete video.</exception>
        public int DeleteVideo(Video video)
        {
            var dal = new Videos();
            var foundVideo = GetVideo(video);
            if (foundVideo == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find video to delete. Video Id: {0}, File Name: {1}", 
                        video.Id, video.FileName), LogLevel.Error);
                throw new ArgumentException("Could not find video to delete.");
            }

            dal.DeleteVideo(foundVideo.Id);

            var getVideo = dal.GetVideoById(foundVideo.Id);
            if (getVideo != null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to delete video. Video Id: {0}", 
                        foundVideo.Id), LogLevel.Error);
                throw new DataException("Failed to delete video.");
            }

            return foundVideo.Id;
        }

        /// <summary>
        /// Doeses the name of the video exist with file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private bool DoesVideoExistWithFileName(string fileName)
        {
            var dal = new Videos();
            var existing = dal.GetVideoByFileName(fileName);

            return (existing != null);
        }

        #endregion Videos

        /// <summary>
        /// Deletes the user image records.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        private void DeleteUserImageRecords(Guid userId, string fileName)
        {
            // delete image record
            DeleteImage(new Image { FileName = fileName });

            // set userimageid column to null
            var userBll = new UserBll();
            userBll.DeleteUserImage(userId);
        }

        /// <summary>
        /// Deletes the user message image records.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void DeleteMessageImageRecords(string fileName)
        {
            // delete image record
            var deletedImgId = DeleteImage(new Image
            {
                FileName = fileName
            });

            // delete message image record
            var messageBll = new MessageBll();
            messageBll.DeleteMessageImage(deletedImgId);
        }

        /// <summary>
        /// Deletes the message video records.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void DeleteMessageVideoRecords(string fileName)
        {
            // delete video record
            var deletedVideoId = DeleteVideo(new Video
            {
                FileName = fileName
            });

            // delete message image record
            var messageBll = new MessageBll();
            messageBll.DeleteMessageVideo(deletedVideoId);
        }

        /// <summary>
        /// Deletes the message melody records.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void DeleteMessageMelodyRecords(string fileName)
        {
            var melodyBll = new MelodyBll();
            var deletedUserMelodyGuid = melodyBll.DeleteUserMelodyByMelodyFileName(fileName);

            var messageBll = new MessageBll();
            messageBll.DeleteMessageUserMelody(deletedUserMelodyGuid);
        }

        /// <summary>
        /// Deletes the old tokens.
        /// </summary>
        private void DeleteOldTokens()
        {
            var dal = new FileUploadTokens();
            dal.DeleteOldTokens();
        }
    }
}

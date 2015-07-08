using System;
using System.Data;
using InstaMelody.Business.Properties;
using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Model.Enums;

namespace InstaMelody.Business
{
    public class FileBLL
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
            User sessionUser = null;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                this.ExpireToken(fileUploadToken);
                throw;
            }
            

            var token = this.GetTokenInfo(fileUploadToken);
            if (token.IsExpired)
            {
                this.ExpireToken(fileUploadToken);
                throw new UnauthorizedAccessException("File Upload Token has expired.");
            }

            if (!sessionUser.Id.Equals(token.UserId))
            {
                this.ExpireToken(fileUploadToken);
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
            var token = this.GetTokenInfo(fileUploadToken);
            if (token != null && token.FileName.Equals(fileName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes the associated file information.
        /// </summary>
        /// <param name="token">The token.</param>
        public void DeleteAssociatedFileInfo(Guid token)
        {
            var dal = new FileUploadTokens();
            var foundToken = dal.GetTokenDetails(token);
            if (foundToken == null) throw new ArgumentException("Token cannot be found.");

            switch (foundToken.MediaType)
            {
                case FileUploadTypeEnum.MessageImage:
                    this.DeleteMessageImageRecords(foundToken.FileName);
                    break;
                case FileUploadTypeEnum.MessageMelody:
                    this.DeleteMessageMelodyRecords(foundToken.FileName);
                    break;
                case FileUploadTypeEnum.MessageVideo:
                    this.DeleteMessageVideoRecords(foundToken.FileName);
                    break;
                case FileUploadTypeEnum.UserImage:
                    this.DeleteUserImageRecords(foundToken.UserId, foundToken.FileName);
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
                throw new ArgumentException("User Id, File Name, and Media Type must be defined to create a token.");
            }

            var dal = new FileUploadTokens();
            var expires = DateTime.UtcNow.AddMinutes(Settings.Default.MinutesFileUploadTokenIsActive);
            var createdToken = dal.CreateToken(token.UserId, token.FileName, token.MediaType, expires);
            if (createdToken == null)
            {
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
                throw new ArgumentException("A valid token Guid must be provided.");
            }

            var dal = new FileUploadTokens();
            var foundToken = dal.GetTokenDetails(token);
            if (foundToken == null || foundToken.IsDeleted)
            {
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
                this.ExpireToken(foundToken.Token);
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
                throw new ArgumentException("Cannot create image with empty File Name.");
            }

            if (this.DoesImageExistWithFileName(image.FileName))
            {
                throw new DataException("Cannot add new image with existing File Name.");
            }

            var dal = new Images();
            image.DateCreated = DateTime.UtcNow;
            var createdImage = dal.CreateImage(image);
            if (createdImage == null)
            {
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
            Image result = null;

            if (image.Id.Equals(default(int)) && string.IsNullOrWhiteSpace(image.FileName))
            {
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
            var foundImage = this.GetImage(image);
            if (foundImage == null)
            {
                throw new ArgumentException("Could not find image to delete.");
            }

            dal.DeleteImage(foundImage.Id);

            var getImage = dal.GetImageById(foundImage.Id);
            if (getImage != null)
            {
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
                throw new ArgumentException("Cannot create video with empty File Name.");
            }

            if (this.DoesVideoExistWithFileName(video.FileName))
            {
                throw new DataException("Cannot add new video with existing File Name.");
            }

            var dal = new Videos();
            video.DateCreated = DateTime.UtcNow;
            var createdVideo = dal.CreateVideo(video);
            if (createdVideo == null)
            {
                throw new DataException("Failed to create Image.");
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
            Video result = null;

            if (video.Id.Equals(default(int)) && string.IsNullOrWhiteSpace(video.FileName))
            {
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
            var foundVideo = this.GetVideo(video);
            if (foundVideo == null)
            {
                throw new ArgumentException("Could not find video to delete.");
            }

            dal.DeleteVideo(foundVideo.Id);

            var getImage = dal.GetVideoById(foundVideo.Id);
            if (getImage != null)
            {
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
            this.DeleteImage(new Image
            {
                FileName = fileName
            });

            // set userimageid column to null
            var userBll = new UserBLL();
            userBll.DeleteUserImage(userId);
        }

        /// <summary>
        /// Deletes the user message image records.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void DeleteMessageImageRecords(string fileName)
        {
            // delete image record
            var deletedImgId = this.DeleteImage(new Image
            {
                FileName = fileName
            });

            // delete message image record
            var messageBll = new MessageBLL();
            messageBll.DeleteMessageImage(deletedImgId);
        }

        /// <summary>
        /// Deletes the message video records.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void DeleteMessageVideoRecords(string fileName)
        {
            // delete video record
            var deletedVideoId = this.DeleteVideo(new Video
            {
                FileName = fileName
            });

            // delete message image record
            var messageBll = new MessageBLL();
            messageBll.DeleteMessageVideo(deletedVideoId);
        }

        /// <summary>
        /// Deletes the message melody records.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void DeleteMessageMelodyRecords(string fileName)
        {
            var melodyBll = new MelodyBLL();
            var deletedUserMelodyGuid = melodyBll.DeleteUserMelodyByMelodyFileName(fileName);

            var messageBll = new MessageBLL();
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

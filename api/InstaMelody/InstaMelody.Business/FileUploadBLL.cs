using System;
using System.Data;
using System.IO;
using InstaMelody.Business.Properties;
using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Model.Enums;

namespace InstaMelody.Business
{
    public class FileUploadBLL
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
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                this.ExpireToken(fileUploadToken);
                throw new UnauthorizedAccessException("Could not validate session.");
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
                    this.DeleteUserMessageImageRecords(foundToken.FileName);
                    break;
                case FileUploadTypeEnum.MessageMelody:
                    this.DeleteUserMessageMelodyRecords(foundToken.FileName);
                    break;
                case FileUploadTypeEnum.StationImage:
                    // TODO:
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

            // TODO: delete old tokens

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
        /// Deletes the user image records.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        private void DeleteUserImageRecords(Guid userId, string fileName)
        {
            // delete image record
            var imageBll = new ImageBLL();
            imageBll.DeleteImage(new Image
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
        private void DeleteUserMessageImageRecords(string fileName)
        {
            // delete image record
            var imageBll = new ImageBLL();
            var deletedImgId = imageBll.DeleteImage(new Image
            {
                FileName = fileName
            });

            // delete message image record
            var messageBll = new MessageBLL();
            messageBll.DeleteUserMessageImage(deletedImgId);
        }

        private void DeleteUserMessageMelodyRecords(string fileName)
        {
            // TODO:
            throw new NotImplementedException();

            //// delete melody record
            //var melodyBll = new MelodyBLL();
            //var deletedMelodyId = melodyBll.DeleteMelody();

            //// delete message melody record
            //var messageBll = new MessageBLL();
            //messageBll.DeleteUserMessageMelody(deletedMelodyId);
        }

        private void DeleteStationImageRecords()
        {
            // TODO:
            throw new NotImplementedException();
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

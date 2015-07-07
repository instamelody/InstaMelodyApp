using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Model.Enums;

namespace InstaMelody.Business
{
    public class MelodyBLL
    {
        #region Public Methods

        /// <summary>
        /// Gets the base melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Could not find the requested melody.</exception>
        public Melody GetBaseMelody(Melody melody)
        {
            var dal = new Melodies();

            Melody result = null;
            if (!melody.Id.Equals(default(int)))
            {
                result = dal.GetMelodyById(melody.Id);
            }

            if (result == null && !string.IsNullOrWhiteSpace(melody.FileName))
            {
                result = dal.GetMelodyByFileName(melody.FileName);
            }

            if (result == null)
            {
                throw new DataException("Could not find the requested melody.");
            }

            return this.GetFilePath(result);
        }

        /// <summary>
        /// Gets the base melodies.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">No melodies found.</exception>
        public IList<Melody> GetBaseMelodies()
        {
            var dal = new Melodies();
            var results = dal.GetAllBaseMelodies();
            if (results == null || !results.Any())
            {
                throw new DataException("No melodies found.");
            }

            return this.GetFilePaths(results);
        }

        /// <summary>
        /// Gets the base melodies by category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">A valid Category was not found.</exception>
        /// <exception cref="System.Data.DataException">No melodies found.</exception>
        public IList<Melody> GetBaseMelodiesByCategory(Category category)
        {
            // get category
            var catBll = new CategoryBLL();
            var foundCategory = catBll.GetCategory(category);
            if (foundCategory == null)
            {
                throw new ArgumentException("A valid Category was not found.");
            }

            // get base melodies for that category
            var dal = new Melodies();
            var results = dal.GetBaseMelodiesByCategoryId(foundCategory.Id);
            if (results == null || !results.Any())
            {
                throw new DataException("No melodies found.");
            }

            return this.GetFilePaths(results);
        }

        /// <summary>
        /// Gets the file group.
        /// </summary>
        /// <param name="fileGroup">The file group.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">A valid File Group must be provided.</exception>
        /// <exception cref="System.Data.DataException">Failed to find the requested File Group.</exception>
        public FileGroup GetFileGroup(FileGroup fileGroup)
        {
            if (fileGroup == null
                || (string.IsNullOrWhiteSpace(fileGroup.Name) && fileGroup.Id.Equals(default(int))))
            {
                throw new ArgumentException("A valid File Group must be provided.");
            }

            FileGroup result = null;

            var dal = new FileGroups();
            if (!fileGroup.Id.Equals(default(int)))
            {
                result = dal.GetFileGroupById(fileGroup.Id);
            }

            if (result == null && !string.IsNullOrWhiteSpace(fileGroup.Name))
            {
                result = dal.GetFileGroupByName(fileGroup.Name);
            }

            if (result == null)
            {
                throw new DataException("Failed to find the requested File Group.");
            }

            result.Melodies = this.GetBaseMelodiesByFileGroup(result.Id);

            return result;
        }

        /// <summary>
        /// Gets the file groups.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to retrieve any File Groups.</exception>
        public IList<FileGroup> GetFileGroups()
        {
            var dal = new FileGroups();
            var results = dal.GetFileGroups();
            if (results == null || !results.Any())
            {
                throw new DataException("Failed to retrieve any File Groups.");
            }

            foreach (var fileGroup in results)
            {
                fileGroup.Melodies = this.GetBaseMelodiesByFileGroup(fileGroup.Id);
            }

            return results;
        }

        /// <summary>
        /// Gets the user melodies.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Could not find any created Melodies for this User</exception>
        public IList<UserMelody> GetUserMelodies(Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var dal = new UserMelodies();
            var userMelodies = dal.GetUserMelodiesByUserId(sessionUser.Id);
            if (userMelodies == null)
            {
                throw new DataException("Could not find any created Melodies for this User");
            }

            // get parts of user melody
            foreach (var userMelody in userMelodies)
            {
                userMelody.Parts = this.GetUserMelodyParts(userMelody.Id);
            }

            return userMelodies;
        }

        /// <summary>
        /// Gets the user melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Could not find the requested Melody with the provided information.</exception>
        public UserMelody GetUserMelody(UserMelody melody, Guid sessionToken)
        {
            Utilities.GetUserBySession(sessionToken);

            return this.GetUserMelody(melody);
        }

        /// <summary>
        /// Creates the user melody.
        /// </summary>
        /// <param name="userMelody">The user melody.</param>
        /// <param name="melody">The melody.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Must provide a valid FileName to create a new User Melody.
        /// or
        /// </exception>
        public ApiUserMelodyFileUpload CreateUserMelody(UserMelody userMelody, Melody melody, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            if (string.IsNullOrEmpty(melody.FileName))
            {
                throw new ArgumentException("Must provide a valid FileName to create a new User Melody.");
            }

            if (string.IsNullOrWhiteSpace(melody.Name))
            {
                melody.Name = melody.FileName;
            }

            if (string.IsNullOrWhiteSpace(userMelody.Name))
            {
                userMelody.Name = melody.Name;
            }

            // validate parts exist
            this.CheckMelodies(userMelody.Parts);

            // find existing user melody
            var existing = this.UserMelodyExistsWithName(sessionUser.Id, userMelody.Name);
            if (existing)
            {
                throw new ArgumentException(
                    string.Format("A User-created Melody already exists with the name: {0}.",
                        userMelody.Name));
            }

            // create new melody
            var createdMelody = this.CreateMelody(melody);
            userMelody.Parts.Add(createdMelody);

            // create user melody
            var dal = new UserMelodies();
            var createdUserMelody = dal.CreateUserMelody(new UserMelody
            {
                Name = userMelody.Name,
                UserId = sessionUser.Id,
                DateCreated = DateTime.UtcNow
            });

            // create user melody parts
            foreach (var part in userMelody.Parts)
            {
                dal.CreateUserMelodyPart(createdUserMelody.Id, part.Id);
            }
            createdUserMelody.Parts = this.GetUserMelodyParts(createdUserMelody.Id);

            // create file upload token
            var fileBll = new FileBLL();
            var createdToken = fileBll.CreateToken(new FileUploadToken
            {
                UserId = sessionUser.Id,
                FileName = createdMelody.FileName,
                MediaType = FileUploadTypeEnum.UserMelody,
                DateCreated = DateTime.UtcNow
            });

            return new ApiUserMelodyFileUpload
            {
                FileUploadToken = createdToken,
                UserMelody = createdUserMelody
            };
        }

        /// <summary>
        /// Deletes the user melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <exception cref="System.UnauthorizedAccessException">Requesting user is not authorized to delete this Melody.</exception>
        public void DeleteUserMelody(UserMelody melody, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var foundMelody = this.GetUserMelody(melody);
            if (!foundMelody.UserId.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException("Requesting user is not authorized to delete this Melody.");
            }

            var dal = new UserMelodies();

            // delete user created melody
            var parts = dal.GetPartsByUserMelodyId(foundMelody.Id);
            if (parts != null && parts.Any())
            {
                var userCreatedMelodies = parts.Where(m => m.IsUserCreated).ToList();
                if (userCreatedMelodies.Any())
                {
                    var mDal = new Melodies();
                    var fileBll = new FileBLL();
                    foreach (var userMelody in userCreatedMelodies)
                    {
                        mDal.DeleteMelody(userMelody.Id);

                        // delete file upload token, if exists
                        fileBll.ExpireToken(sessionUser.Id, userMelody.FileName, MediaTypeEnum.Melody);
                    }
                }
            }

            // delete user melody & parts (handled @ DAL)
            dal.DeleteUserMelody(foundMelody.Id);
        }

        public IList<UserLoop> GetUserLoops(Guid sessionToken)
        {
            // TODO:
            throw new NotImplementedException();

            var sessionUser = Utilities.GetUserBySession(sessionToken);
        }

        public UserLoop GetLoop(UserLoop loop, Guid sessionToken)
        {
            // TODO:
            throw new NotImplementedException();

            var sessionUser = Utilities.GetUserBySession(sessionToken);
        }

        public UserLoop CreateLoop(UserLoop loop, Guid sessionToken)
        {
            // TODO:
            throw new NotImplementedException();

            var sessionUser = Utilities.GetUserBySession(sessionToken);
        }

        public UserLoop AttachPartToLoop(UserLoop loop, UserMelody newPart, Guid sessionToken)
        {
            // TODO:
            throw new NotImplementedException();

            var sessionUser = Utilities.GetUserBySession(sessionToken);
        }

        public UserLoop CreateMelodyAndAttachToLoop(UserMelody userMelody, Melody melody, UserLoop loop, Guid sessionToken)
        {
            // TODO:
            throw new NotImplementedException();

            var sessionUser = Utilities.GetUserBySession(sessionToken);
        }

        public UserLoop DeletePartFromLoop(UserLoop loop, UserMelody part, Guid sessionToken)
        {
            // TODO:
            throw new NotImplementedException();

            var sessionUser = Utilities.GetUserBySession(sessionToken);
        }

        public void DeleteLoop(UserLoop loop, Guid sessionToken)
        {
            // TODO:
            throw new NotImplementedException();

            var sessionUser = Utilities.GetUserBySession(sessionToken);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the base melodies by file group.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">No melodies found.</exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentException">A valid File Group was not found.</exception>
        public IList<Melody> GetBaseMelodiesByFileGroup(int fileGroupId)
        {
            var dal = new Melodies();
            var results = dal.GetBaseMelodiesByFileGroupId(fileGroupId);
            if (results == null || !results.Any())
            {
                throw new DataException("No melodies found.");
            }

            return this.GetFilePaths(results);
        }

        /// <summary>
        /// Creates the melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Data.DataException">Failed to create a new Melody.</exception>
        private Melody CreateMelody(Melody melody)
        {
            var dal = new Melodies();
            var foundMelody = dal.GetMelodyByFileName(melody.FileName);
            if (foundMelody != null)
            {
                throw new ArgumentException(
                    string.Format("A melody already exists with the File Name: {0}.",
                        melody.FileName));
            }


            var result = dal.CreateMelody(new Melody
            {
                FileName = melody.FileName,
                Name = melody.Name,
                IsUserCreated = true,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                Description = melody.Description
            });

            if (result == null)
            {
                throw new DataException("Failed to create a new Melody.");
            }

            return result;
        }

        /// <summary>
        /// Users the name of the melody exists with.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private bool UserMelodyExistsWithName(Guid userId, string name)
        {
            var dal = new UserMelodies();
            var result = dal.GetUserMelodyByUserIdAndName(userId, name);

            return (result != null);
        }

        /// <summary>
        /// Checks the melodies.
        /// </summary>
        /// <param name="melodies">The melodies.</param>
        /// <exception cref="System.ArgumentException"></exception>
        private void CheckMelodies(IList<Melody> melodies)
        {
            var dal = new Melodies();
            foreach (var melody in melodies)
            {
                if (dal.GetMelodyById(melody.Id) == null)
                {
                    throw new ArgumentException(
                    string.Format("The provided User Melody Part: {0} is not a valid Melody.",
                        melody.Id));
                }
            }
        }

        /// <summary>
        /// Gets the user melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Could not find the requested Melody with the provided information.</exception>
        private UserMelody GetUserMelody(UserMelody melody)
        {
            var dal = new UserMelodies();

            var result = dal.GetUserMelodyById(melody.Id);
            if (result == null
                && (!melody.UserId.Equals(default(Guid)) && !string.IsNullOrWhiteSpace(melody.Name)))
            {
                result = dal.GetUserMelodyByUserIdAndName(melody.UserId, melody.Name);
            }

            if (result == null)
            {
                throw new ArgumentException("Could not find the requested Melody with the provided information.");
            }

            // get parts of user melody
            result.Parts = this.GetUserMelodyParts(result.Id);

            return result;
        }

        /// <summary>
        /// Gets the user melody parts.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        private IList<Melody> GetUserMelodyParts(Guid userMelodyId)
        {
            var userMelodyDal = new UserMelodies();
            var parts = userMelodyDal.GetPartsByUserMelodyId(userMelodyId);
            if (parts == null)
            {
                throw new DataException(string.Format("Could not find any parts for User Melody {0}.", userMelodyId));
            }

            return this.GetFilePaths(parts);
        }

        /// <summary>
        /// Gets the file paths.
        /// </summary>
        /// <param name="melodies">The melodies.</param>
        /// <returns></returns>
        private IList<Melody> GetFilePaths(IList<Melody> melodies)
        {
            foreach (var melody in melodies)
            {
                melody.FilePath = Utilities.GetFilePath(melody.FileName, MediaTypeEnum.Melody);
            }

            return melodies;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        private Melody GetFilePath(Melody melody)
        {
            melody.FilePath = Utilities.GetFilePath(melody.FileName, MediaTypeEnum.Melody);

            return melody;
        }

        #endregion Private Methods
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InstaMelody.Business.Properties;
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
                throw new DataException("Could not find any created Melodies for this User.");
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
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Must provide a valid FileName to create a new User Melody.
        /// or</exception>
        public ApiUserMelodyFileUpload CreateUserMelody(UserMelody userMelody, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            return this.CreateUserMelody(userMelody, sessionUser);
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

        /// <summary>
        /// Gets the user loops.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Could not find any created Loops beloging to this User.</exception>
        public IList<UserLoop> GetUserLoops(Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var dal = new UserLoops();
            var loops = dal.GetUserLoopsByUserId(sessionUser.Id);
            if (loops == null || !loops.Any())
            {
                throw new DataException("Could not find any created Loops beloging to this User.");
            }

            foreach (var userLoop in loops)
            {
                userLoop.Parts = this.GetUserLoopParts(userLoop.Id);
            }

            return loops;
        }

        /// <summary>
        /// Gets the loop.
        /// </summary>
        /// <param name="loop">The loop.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        public UserLoop GetLoop(UserLoop loop, Guid sessionToken)
        {
            Utilities.GetUserBySession(sessionToken);

            return this.GetUserLoop(loop);
        }

        /// <summary>
        /// Creates the loop.
        /// </summary>
        /// <param name="loop">The loop.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Cannot create a new Loop without a Loop name.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public object CreateLoop(UserLoop loop, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            
            var dal = new UserLoops();

            // check loop name
            if (string.IsNullOrWhiteSpace(loop.Name))
            {
                throw new ArgumentException("Cannot create a new Loop without a Name.");
            }

            var existing = dal.GetUserLoopByUserIdAndName(sessionUser.Id, loop.Name);
            if (existing != null)
            {
                throw new DataException(
                    string.Format("The user has already created a Loop with the name: {0}.",
                        existing.Name));
            }

            // create loop
            var createdLoop = dal.CreateUserLoop(new UserLoop
            {
                UserId = sessionUser.Id,
                Name = loop.Name,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            });

            // create loop parts
            try
            {
                var parts = this.CreateUserLoopParts(sessionUser, createdLoop.Id, loop.Parts);

                var uploadTokens = (from tuple in parts where tuple.Item2 != null select tuple.Item2).ToList();
                if (uploadTokens.Any())
                {
                    return new ApiLoopFileUpload
                    {
                        Loop = this.GetUserLoop(createdLoop),
                        FileUploadTokens = uploadTokens
                    };
                }

                return this.GetUserLoop(createdLoop);
            }
            catch (Exception)
            {
                dal.DeleteUserLoop(createdLoop.Id);
                throw;
            }
        }

        /// <summary>
        /// Attaches the part to loop.
        /// </summary>
        /// <param name="loop">The loop.</param>
        /// <param name="newPart">The new part.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        public object AttachPartToLoop(UserLoop loop, UserLoopPart newPart, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            // get loop
            var foundLoop = this.GetUserLoop(loop);

            // create loop parts
            try
            {
                var parts = this.CreateUserLoopParts(sessionUser, foundLoop.Id, new List<UserLoopPart> { newPart });

                var uploadTokens = (from tuple in parts where tuple.Item2 != null select tuple.Item2).ToList();
                if (uploadTokens.Any())
                {
                    return new ApiLoopFileUpload
                    {
                        Loop = this.GetUserLoop(foundLoop),
                        FileUploadTokens = uploadTokens
                    };
                }

                return this.GetUserLoop(foundLoop);
            }
            catch (Exception)
            {
                var dal = new UserLoops();
                dal.DeleteUserLoop(foundLoop.Id);
                throw;
            }
        }

        /// <summary>
        /// Deletes the part from loop.
        /// </summary>
        /// <param name="loop">The loop.</param>
        /// <param name="part">The part.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Requesting user is not authorized to delete this part of the Loop.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public UserLoop DeletePartFromLoop(UserLoop loop, UserMelody part, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var foundLoop = this.GetUserLoop(loop);

            var foundUserMelody = this.GetUserMelody(part);
            if (!foundUserMelody.UserId.Equals(sessionUser.Id)
                && !foundLoop.UserId.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException("Requesting user is not authorized to delete this part of the Loop.");
            }

            var foundPart = foundLoop.Parts.FirstOrDefault(p => p.UserMelodyId.Equals(foundUserMelody.Id));
            if (foundPart == null)
            {
                throw new DataException(
                    string.Format("Could not find the requested part of Loop: {0} to be deleted.",
                        foundLoop.Id));
            }

            var dal = new UserLoops();
            dal.DeleteLoopPart(foundPart.Id);

            // TODO: reindex loop parts before returning result

            return this.GetUserLoop(foundLoop);
        }

        /// <summary>
        /// Deletes the loop.
        /// </summary>
        /// <param name="loop">The loop.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <exception cref="System.UnauthorizedAccessException">Requesting user is not authorized to delete this Loop.</exception>
        public void DeleteLoop(UserLoop loop, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var foundLoop = this.GetUserLoop(loop);
            if (!foundLoop.UserId.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException("Requesting user is not authorized to delete this Loop.");
            }

            // delete user loop & parts (handled @ DAL)
            var dal = new UserLoops();
            dal.DeleteUserLoop(foundLoop.Id);
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

        /// <summary>
        /// Gets the user loop.
        /// </summary>
        /// <param name="loop">The loop.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Could not find the requested Loop with the provided information.</exception>
        private UserLoop GetUserLoop(UserLoop loop)
        {
            var dal = new UserLoops();

            var result = dal.GetUserLoopById(loop.Id);
            if (result == null
                && (!loop.UserId.Equals(default(Guid)) && !string.IsNullOrWhiteSpace(loop.Name)))
            {
                result = dal.GetUserLoopByUserIdAndName(loop.UserId, loop.Name);
            }

            if (result == null)
            {
                throw new ArgumentException("Could not find the requested Loop with the provided information.");
            }

            // get user loop parts
            result.Parts = this.GetUserLoopParts(result.Id);

            return result;
        }

        /// <summary>
        /// Gets the user loop parts.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        /// <returns></returns>
        private IList<UserLoopPart> GetUserLoopParts(Guid userLoopId)
        {
            var dal = new UserLoops();

            var results = dal.GetPartsByUserLoopId(userLoopId);
            if (results == null || !results.Any())
            {
                return null;
            }

            // get user melody
            foreach (var userLoopPart in results)
            {
                userLoopPart.UserMelody = 
                    this.GetUserMelody(new UserMelody
                    {
                        Id = userLoopPart.UserMelodyId
                    }, getDeletedMelodies: true);
            }

            return results;
        }

        /// <summary>
        /// Gets the existing or create new user melody.
        /// </summary>
        /// <param name="userMelody">The user melody.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Could not find or create a new Melody to attach to this Loop.</exception>
        private object GetExistingOrCreateNewUserMelody(UserMelody userMelody, User user)
        {
            // determine if melody exists
            object createdOrFoundMelody;
            try
            {
                createdOrFoundMelody = this.GetUserMelody(userMelody);
            }
            catch (Exception)
            {
                createdOrFoundMelody = this.CreateUserMelody(userMelody, user);
            }

            if (createdOrFoundMelody == null)
            {
                throw new ArgumentException("Could not find or create a new Melody to attach to this Loop.");
            }

            return createdOrFoundMelody;
        }

        /// <summary>
        /// Creates the user loop parts.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="loopId">The loop identifier.</param>
        /// <param name="parts">The parts.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Each Part of a Loop must contain a new or existing Melody.</exception>
        private IList<Tuple<UserLoopPart, FileUploadToken>> CreateUserLoopParts(User owner, Guid loopId, IList<UserLoopPart> parts)
        {
            var createdParts = new List<Tuple<UserLoopPart, FileUploadToken>>();
            foreach (var part in parts)
            {
                if (part.UserMelody == null)
                {
                    throw new ArgumentException("Each Part of a Loop must contain a new or existing Melody.");
                }
                var createdMelody = this.GetExistingOrCreateNewUserMelody(part.UserMelody, owner);
                var upload = createdMelody as ApiUserMelodyFileUpload;
                var melody = (upload != null)
                    ? upload.UserMelody
                    : ((UserMelody)createdMelody);
                var token = (upload != null)
                    ? upload.FileUploadToken
                    : null;
                var createdPart = this.CreateUserLoopPart(loopId, melody.Id, part);
                createdParts.Add(new Tuple<UserLoopPart, FileUploadToken>(createdPart, token));
            }
            return createdParts;
        }

        /// <summary>
        /// Creates the user loop part.
        /// </summary>
        /// <param name="loopId">The loop identifier.</param>
        /// <param name="userMelodyId">The user melody identifier.</param>
        /// <param name="part">The part.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create User Loop part.</exception>
        private UserLoopPart CreateUserLoopPart(Guid loopId, Guid userMelodyId, UserLoopPart part)
        {
            var dal = new UserLoops();
            var lastIndex = dal.GetLastOrderIndexForLoop(loopId);

            var newPart = new UserLoopPart
            {
                UserLoopId = loopId,
                UserMelodyId = userMelodyId,
                StartTime = part.StartTime ?? TimeSpan.Zero,
                StartEffect = (part.StartEffect == LoopEffectsEnum.Unknown)
                    ? Settings.Default.LoopPartFirstStartEffect
                    : part.StartEffect,
                StartEffectDuration = part.StartEffectDuration ?? Settings.Default.LoopPartFirstStartDuration,
                EndTime = part.EndTime,
                EndEffect = (part.EndEffect == LoopEffectsEnum.Unknown)
                    ? Settings.Default.LoopPartEndEffect
                    : part.EndEffect,
                EndEffectDuration = part.EndEffectDuration ?? Settings.Default.LoopPartEndEffectDuration,
                OrderIndex = lastIndex + 1,
                DateCreated = DateTime.UtcNow
            };

            if (lastIndex > 0)
            {
                newPart.StartTime = part.StartTime ?? TimeSpan.Zero;
                newPart.StartEffect = (part.StartEffect == LoopEffectsEnum.Unknown)
                    ? Settings.Default.LoopPartStartEffect
                    : part.StartEffect;
                newPart.StartEffectDuration = part.StartEffectDuration ?? Settings.Default.LoopPartStartEffectDuration;
            }

            dal.UpdateUserLoopDateModified(loopId);

            var createdPart = dal.CreateUserLoopPart(newPart);
            if (createdPart == null)
            {
                throw new DataException("Failed to create User Loop part.");
            }

            return createdPart;
        }

        #endregion Private Methods

        #region Internal Methods

        /// <summary>
        /// Gets the user melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <param name="getDeletedMelodies">if set to <c>true</c> [get deleted melodies].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Could not find the requested Melody with the provided information.</exception>
        internal UserMelody GetUserMelody(UserMelody melody, bool getDeletedMelodies = false)
        {
            var dal = new UserMelodies();

            var result = dal.GetUserMelodyById(melody.Id, getDeletedMelodies);
            if (result == null
                && (!melody.UserId.Equals(default(Guid)) && !string.IsNullOrWhiteSpace(melody.Name)))
            {
                result = dal.GetUserMelodyByUserIdAndName(melody.UserId, melody.Name, getDeletedMelodies);
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
        /// Creates the user melody.
        /// </summary>
        /// <param name="userMelody">The user melody.</param>
        /// <param name="sender">The sender.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Must provide a Melody part with a valid FileName to create a new User Melody.
        /// or
        /// </exception>
        internal ApiUserMelodyFileUpload CreateUserMelody(UserMelody userMelody, User sender)
        {
            var melody = userMelody.Parts.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.FileName));
            if (melody == null)
            {
                throw new ArgumentException("Must provide a Melody part with a valid FileName to create a new User Melody.");
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
            var parts = userMelody.Parts.Where(p => !p.Id.Equals(default(int))).ToList();
            this.CheckMelodies(parts);

            // find existing user melody
            var existing = this.UserMelodyExistsWithName(sender.Id, userMelody.Name);
            if (existing)
            {
                throw new ArgumentException(
                    string.Format("A User-created Melody already exists with the name: {0}.",
                        userMelody.Name));
            }

            // create new melody
            var createdMelody = this.CreateMelody(melody);
            parts.Add(createdMelody);

            // create user melody
            var dal = new UserMelodies();
            var createdUserMelody = dal.CreateUserMelody(new UserMelody
            {
                Name = userMelody.Name,
                UserId = sender.Id,
                DateCreated = DateTime.UtcNow
            });

            // create user melody parts
            foreach (var part in parts)
            {
                dal.CreateUserMelodyPart(createdUserMelody.Id, part.Id);
            }
            createdUserMelody.Parts = this.GetUserMelodyParts(createdUserMelody.Id);

            // create file upload token
            var fileBll = new FileBLL();
            var createdToken = fileBll.CreateToken(new FileUploadToken
            {
                UserId = sender.Id,
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
        /// Deletes the name of the user melody by melody file.
        /// </summary>
        /// <param name="melodyFileName">Name of the melody file.</param>
        /// <returns></returns>
        internal Guid DeleteUserMelodyByMelodyFileName(string melodyFileName)
        {
            // get melody by file name
            var dal = new Melodies();
            var melody = dal.GetMelodyByFileName(melodyFileName);
            if (melody == null)
            {
                throw new ArgumentException(
                    string.Format("Could not find a melody with the provided file name: {0}.", 
                        melodyFileName));
            }

            // get user melody by melody id
            var userMelodyDal = new UserMelodies();
            var userMelody = userMelodyDal.GetUserMelodyByMelodyId(melody.Id);
            if (userMelody == null)
            {
                throw new DataException(
                    string.Format("Could not find a User Melody that contained a Melody Part with the file name: {0}",
                        melodyFileName));
            }

            // delete melody file
            dal.DeleteMelody(melody.Id);

            // delete user melody
            userMelodyDal.DeleteUserMelody(userMelody.Id);

            // return user melody guid
            return userMelody.Id;
        }

        #endregion Internal Methods
    }
}

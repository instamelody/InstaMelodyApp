using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InstaMelody.Business.Properties;
using InstaMelody.Data;
using InstaMelody.Infrastructure;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Model.Enums;
using NLog;

namespace InstaMelody.Business
{
    public class MelodyBll
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

            if (result != null) return GetFilePath(result);

            InstaMelodyLogger.Log(
                string.Format("Could not find the requested melody. Id: {0}, File Name: {1}", 
                    melody.Id, melody.FileName), LogLevel.Error);
            throw new DataException("Could not find the requested melody.");
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
            if (results != null && results.Any()) return GetFilePaths(results);

            InstaMelodyLogger.Log("No melodies found for GetBaseMelodies().", LogLevel.Error);
            throw new DataException("No melodies found.");
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
            var catBll = new CategoryBll();
            var foundCategory = catBll.GetCategory(category);
            if (foundCategory == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("A valid Category was not found. Category Id: {0}, Name: {1}",
                        category.Id, category.Name), LogLevel.Error);
                throw new ArgumentException("A valid Category was not found.");
            }

            // get base melodies for that category
            var dal = new Melodies();
            var results = dal.GetBaseMelodiesByCategoryId(foundCategory.Id);
            if (results == null || !results.Any())
            {
                InstaMelodyLogger.Log(
                    string.Format("No melodies found for Category. Category Id: {0}, Name: {1}",
                        category.Id, category.Name), LogLevel.Error);
                throw new DataException("No melodies found.");
            }

            return GetFilePaths(results);
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
                InstaMelodyLogger.Log(
                    "A valid File Group must be provided. File Group Name and Id undefined.", 
                    LogLevel.Error);
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
                InstaMelodyLogger.Log(
                    string.Format("Failed to find the requested File Group. File Group Id: {0}, Name: {1}",
                        fileGroup.Id, fileGroup.Name), LogLevel.Error);
                throw new DataException("Failed to find the requested File Group.");
            }

            result.Melodies = GetBaseMelodiesByFileGroup(result.Id);

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
                InstaMelodyLogger.Log(
                    "Failed to retrieve any File Groups for GetFileGroups().", LogLevel.Error);
                throw new DataException("Failed to retrieve any File Groups.");
            }

            foreach (var fileGroup in results)
            {
                fileGroup.Melodies = GetBaseMelodiesByFileGroup(fileGroup.Id);
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
                InstaMelodyLogger.Log(
                    string.Format("Could not find any created Melodies for this User. User Id: {0}, Token: {1}", 
                        sessionUser.Id, sessionToken), LogLevel.Error);
                throw new DataException("Could not find any created Melodies for this User.");
            }

            // get parts of user melody
            foreach (var userMelody in userMelodies)
            {
                userMelody.Parts = GetUserMelodyParts(userMelody.Id);
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

            return GetUserMelody(melody);
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

            return CreateUserMelody(userMelody, sessionUser);
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

            var foundMelody = GetUserMelody(melody);
            if (!foundMelody.UserId.Equals(sessionUser.Id))
            {
                InstaMelodyLogger.Log(
                    string.Format("Requesting User is not authorized to delete this Melody. User Id: {0}, Melody Id: {1}",
                        sessionUser.Id, foundMelody.Id), LogLevel.Error);
                throw new UnauthorizedAccessException("Requesting User is not authorized to delete this Melody.");
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
                    var fileBll = new FileBll();
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
                InstaMelodyLogger.Log(
                    string.Format("Could not find any created Loops beloging to this User. User Id: {0}", 
                        sessionUser.Id), LogLevel.Error);
                throw new DataException("Could not find any created Loops beloging to this User.");
            }

            foreach (var userLoop in loops)
            {
                userLoop.Parts = GetUserLoopParts(userLoop.Id);
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
            return GetUserLoop(loop);
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
                InstaMelodyLogger.Log(
                    string.Format("Cannot create a new Loop without a Name. User Id: {0}", 
                        sessionUser.Id), LogLevel.Error);
                throw new ArgumentException("Cannot create a new Loop without a Name.");
            }

            var existing = dal.GetUserLoopByUserIdAndName(sessionUser.Id, loop.Name);
            if (existing != null)
            {
                InstaMelodyLogger.Log(
                    string.Format("The user has already created a Loop with a matching name. User Id: {0}, Loop Name: {1}",
                        sessionUser.Id, loop.Name), LogLevel.Error);
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
                var parts = CreateUserLoopParts(sessionUser, createdLoop.Id, loop.Parts);

                var uploadTokens = (from tuple in parts where tuple.Item2 != null select tuple.Item2).ToList();
                if (uploadTokens.Any())
                {
                    return new ApiLoopFileUpload
                    {
                        Loop = GetUserLoop(createdLoop),
                        FileUploadTokens = uploadTokens
                    };
                }

                return GetUserLoop(createdLoop);
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
            var foundLoop = GetUserLoop(loop);

            // create loop parts
            try
            {
                var parts = CreateUserLoopParts(sessionUser, foundLoop.Id, new List<UserLoopPart> { newPart });

                var uploadTokens = (from tuple in parts where tuple.Item2 != null select tuple.Item2).ToList();
                if (uploadTokens.Any())
                {
                    return new ApiLoopFileUpload
                    {
                        Loop = GetUserLoop(foundLoop),
                        FileUploadTokens = uploadTokens
                    };
                }

                return GetUserLoop(foundLoop);
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

            var foundLoop = GetUserLoop(loop);

            var foundUserMelody = GetUserMelody(part);
            if (!foundUserMelody.UserId.Equals(sessionUser.Id)
                && !foundLoop.UserId.Equals(sessionUser.Id))
            {
                InstaMelodyLogger.Log(
                    string.Format("Requesting User is not authorized to delete this part of the Loop. User Id: {0}, Loop Owner Id: {1}, Loop Part Owner Id: {2}",
                        sessionUser.Id, foundLoop.UserId, foundUserMelody.UserId), LogLevel.Error);
                throw new UnauthorizedAccessException("Requesting User is not authorized to delete this part of the Loop.");
            }

            var foundPart = foundLoop.Parts.FirstOrDefault(p => p.UserMelodyId.Equals(foundUserMelody.Id));
            if (foundPart == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find the requested part of the Loop to be deleted. User Id: {0}, Loop Id: {1}, User Melody Id: {2}",
                        sessionUser.Id, foundLoop.Id, foundUserMelody.Id), LogLevel.Error);
                throw new DataException(
                    string.Format("Could not find the requested part of Loop: {0} to be deleted.",
                        foundLoop.Id));
            }

            var dal = new UserLoops();
            dal.DeleteLoopPart(foundPart.Id);

            // TODO: reindex loop parts before returning result

            return GetUserLoop(foundLoop);
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

            var foundLoop = GetUserLoop(loop);
            if (!foundLoop.UserId.Equals(sessionUser.Id))
            {
                InstaMelodyLogger.Log(
                    string.Format("Requesting User is not authorized to delete this Loop. User Id: {0}, Loop Id: {1}",
                        sessionUser.Id, foundLoop.Id), LogLevel.Error);
                throw new UnauthorizedAccessException("Requesting User is not authorized to delete this Loop.");
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
                InstaMelodyLogger.Log(
                    string.Format("No melodies found. File Group Id: {0}", fileGroupId), LogLevel.Error);
                throw new DataException("No melodies found.");
            }

            return GetFilePaths(results);
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
                InstaMelodyLogger.Log(
                    string.Format("A Melody already exists with a matching File Name. File Name: {0}", 
                        melody.FileName), LogLevel.Error);
                throw new ArgumentException(
                    string.Format("A Melody already exists with the File Name: {0}.",
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
                InstaMelodyLogger.Log(
                    string.Format("Failed to create a new Melody. File Name: {0}", 
                        melody.FileName), LogLevel.Error);
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
                    InstaMelodyLogger.Log(
                        string.Format("The provided User Melody Part is not a valid Melody. Melody Id: {0}", 
                            melody.Id), LogLevel.Error);
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
                InstaMelodyLogger.Log(
                    string.Format("Could not find any parts for User Melody. User Melody Id: {0}", 
                        userMelodyId), LogLevel.Error);
                throw new DataException(string.Format("Could not find any parts for User Melody {0}.", userMelodyId));
            }

            return GetFilePaths(parts);
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
                InstaMelodyLogger.Log(
                    string.Format("Could not find the requested Loop with the provided information. Loop Id: {0}, User Id: {1}, Loop Name: {2}",
                        loop.Id, loop.UserId, loop.Name), LogLevel.Error);
                throw new ArgumentException("Could not find the requested Loop with the provided information.");
            }

            // get user loop parts
            result.Parts = GetUserLoopParts(result.Id);

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
                    GetUserMelody(new UserMelody
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
                createdOrFoundMelody = GetUserMelody(userMelody);
            }
            catch (Exception)
            {
                createdOrFoundMelody = CreateUserMelody(userMelody, user);
            }

            if (createdOrFoundMelody == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find or create a new Melody. Melody Id: {0}, Name: {1}", 
                        userMelody.Id, userMelody.Name), LogLevel.Error);
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
                    continue;

                var createdMelody = GetExistingOrCreateNewUserMelody(part.UserMelody, owner);
                var upload = createdMelody as ApiUserMelodyFileUpload;
                var melody = (upload != null)
                    ? upload.UserMelody
                    : ((UserMelody)createdMelody);
                var token = (upload != null)
                    ? upload.FileUploadToken
                    : null;
                var createdPart = CreateUserLoopPart(loopId, melody.Id, part);
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
                InstaMelodyLogger.Log(
                    string.Format("Failed to create User Loop part. Loop Id: {0}, User Melody Id: {1}",
                        loopId, userMelodyId), LogLevel.Error);
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
                InstaMelodyLogger.Log(
                    string.Format("Could not find the requested Melody with the provided information. Melody Id: {0}, Name: {1}, User Id: {2}",
                        melody.Id, melody.Name, melody.UserId), LogLevel.Error);
                throw new ArgumentException("Could not find the requested Melody with the provided information.");
            }

            // get parts of user melody
            result.Parts = GetUserMelodyParts(result.Id);

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
                InstaMelodyLogger.Log(
                    string.Format("Must provide a Melody part with a valid FileName to create a new User Melody. Sender Id: {0}", 
                        sender.Id), LogLevel.Error);
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
            CheckMelodies(parts);

            // find existing user melody
            var existing = UserMelodyExistsWithName(sender.Id, userMelody.Name);
            if (existing)
            {
                InstaMelodyLogger.Log(
                    string.Format("A User-created Melody already exists with a matching name. Name: {0}, User Id: {1}", 
                        userMelody.Name, sender.Id), LogLevel.Error);
                throw new ArgumentException(
                    string.Format("A User-created Melody already exists with the name: {0}.",
                        userMelody.Name));
            }

            // create new melody
            var createdMelody = CreateMelody(melody);
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
            createdUserMelody.Parts = GetUserMelodyParts(createdUserMelody.Id);

            // create file upload token
            var fileBll = new FileBll();
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
                InstaMelodyLogger.Log(
                    string.Format("Could not find a melody with the provided file name. File Name: {0}.", 
                        melodyFileName), LogLevel.Error);
                throw new ArgumentException(
                    string.Format("Could not find a melody with the provided file name: {0}.", 
                        melodyFileName));
            }

            // get user melody by melody id
            var userMelodyDal = new UserMelodies();
            var userMelody = userMelodyDal.GetUserMelodyByMelodyId(melody.Id);
            if (userMelody == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find a User Melody that contained a Melody Part with the file name. File Name: {0}",
                        melodyFileName), LogLevel.Error);
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

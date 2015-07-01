using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using InstaMelody.Business.Properties;
using InstaMelody.Data;

using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Model.Enums;
using ModelUtilities = InstaMelody.Model.Utilities;

namespace InstaMelody.Business
{
    public class MelodyBLL
    {
        #region Public Methods

        /// <summary>
        /// Creates the user melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.Data.DataException">
        /// A melody with the file name provided already exists.
        /// or
        /// A melody has already been created by this User with the same Name.
        /// </exception>
        public ApiUserMelodyFileUpload CreateUserMelody(Melody melody, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || sessionUser == default(User))
            {
                throw new UnauthorizedAccessException(
                    string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            var fileNameMatch = this.GetMelodyByFileName(melody.FileName);
            if (fileNameMatch != null)
            {
                throw new DataException("A melody with the file name provided already exists.");
            }

            var userAndNameMatch = this.GetMelodyByUserIdAndName(sessionUser.Id, melody.Name);
            if (userAndNameMatch != null)
            {
                throw new DataException("A melody has already been created by this User with the same Name.");
            }

            melody.UserId = sessionUser.Id;

            // save melody
            var dal = new Melodies();
            var savedMelody = dal.CreateMelody(melody);

            // save melody categories
            if (melody.Categories != null && melody.Categories.Any())
            {
                this.SaveMelodyCategories(savedMelody.Id, melody.Categories);
            }

            // create file upload token
            var uploadBll = new FileUploadBLL();
            var uploadToken = uploadBll.CreateToken(new FileUploadToken
            {
                UserId = sessionUser.Id,
                FileName = savedMelody.FileName,
                MediaType = FileUploadTypeEnum.UserMelody,
                DateCreated = DateTime.UtcNow
            });

            return new ApiUserMelodyFileUpload
            {
                Melody = this.GetMelody(savedMelody, sessionToken),
                FileUploadToken = uploadToken
            };
        }

        public MelodyLoop CreateUserMelodyLoop()
        {
            // TODO:
            throw new NotImplementedException();
        }

        public MelodyLoop GetMelodyLoop(Guid sessionToken)
        {
            // TODO:
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.ArgumentException">Must provide a valid Melody Id or File Name.
        /// or
        /// Must provide a valid Melody Id or File Name.</exception>
        public Melody GetMelody(Melody melody, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || sessionUser == default(User))
            {
                throw new UnauthorizedAccessException(
                    string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (melody == null
                || (melody.Id.Equals(default(int))
                    && string.IsNullOrWhiteSpace(melody.FileName)))
            {
                throw new ArgumentException("Must provide a valid Melody Id or File Name.");
            }

            var result = this.GetMelodyRecord(melody);
            if (result == null)
            {
                throw new ArgumentException("Must provide a valid Melody Id or File Name.");
            }

            result.Categories = this.GetMelodyCategories(result);
            result.FilePath = this.GetMelodyFilePath(result);

            // get base melody if result has id
            if (result.BaseMelodyId != null && !result.BaseMelodyId.Equals(default(int)))
            {
                result.BaseMelody = this.GetBaseMelody(result);
            }

            return result;
        }

        /// <summary>
        /// Gets the base melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.ArgumentException">
        /// Must provide a valid Melody Id or File Name.
        /// or
        /// Must provide a valid Melody Id or File Name.
        /// or
        /// Requested melody is a User created Melody. Cannot retrieve this record.
        /// </exception>
        public Melody GetBaseMelody(Melody melody, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || sessionUser == default(User))
            {
                throw new UnauthorizedAccessException(
                    string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (melody == null
                || (melody.Id.Equals(default(int))
                    && string.IsNullOrWhiteSpace(melody.FileName)))
            {
                throw new ArgumentException("Must provide a valid Melody Id or File Name.");
            }

            var result = this.GetMelodyRecord(melody);
            if (result == null)
            {
                throw new ArgumentException("Must provide a valid Melody Id or File Name.");
            }

            if (result.IsUserMelody)
            {
                throw new ArgumentException("Requested melody is a User created Melody. Cannot retrieve this record.");
            }

            result.Categories = this.GetMelodyCategories(result);
            result.FilePath = this.GetMelodyFilePath(result);

            return result;
        }

        /// <summary>
        /// Gets the base melodies.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.Data.DataException">Could not find any Melodies.</exception>
        public IList<Melody> GetBaseMelodies(Guid sessionToken, Category category = null)
        {
            IList<Melody> results;

            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || sessionUser == default(User))
            {
                throw new UnauthorizedAccessException(
                    string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            var dal = new Melodies();
            if (category != null && !category.Id.Equals(default(int)))
            {
                results = dal.GetBaseMelodiesByCategoryId(category.Id);
            }
            else
            {
                results = dal.GetBaseMelodies();
            }

            if (results == null || !results.Any())
            {
                throw new DataException("Could not find any Melodies.");
            }

            foreach (var result in results)
            {
                result.FilePath = GetMelodyFilePath(result);
                result.Categories = GetMelodyCategories(result);
            }

            return results;
        }

        /// <summary>
        /// Gets the user melodies.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.Data.DataException">Could not find any Melodies.</exception>
        public IList<Melody> GetUserMelodies(Guid sessionToken, Category category = null)
        {
            IList<Melody> results;

            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || sessionUser == default(User))
            {
                throw new UnauthorizedAccessException(
                    string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            var dal = new Melodies();
            if (category != null && !category.Id.Equals(default(int)))
            {
                results = dal.GetMelodiesByUserIdAndCategoryId(sessionUser.Id, category.Id);
            }
            else
            {
                results = dal.GetMelodiesByUserId(sessionUser.Id);
            }

            if (results == null || !results.Any())
            {
                throw new DataException("Could not find any Melodies for this User.");
            }

            foreach (var result in results)
            {
                result.FilePath = GetMelodyFilePath(result);
                result.Categories = GetMelodyCategories(result);
            }

            return results;
        }

        /// <summary>
        /// Deletes the melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.ArgumentException">
        /// Must provide a valid Melody Id or File Name.
        /// or
        /// Must provide a valid Melody Id or File Name.
        /// or
        /// Requested melody was not created by this User. Cannot delete this record.
        /// </exception>
        /// <exception cref="System.Data.DataException"></exception>
        public void DeleteUserMelody(Melody melody, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || sessionUser == default(User))
            {
                throw new UnauthorizedAccessException(
                    string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (melody == null
                || (melody.Id.Equals(default(int))
                    && string.IsNullOrWhiteSpace(melody.FileName)))
            {
                throw new ArgumentException("Must provide a valid Melody Id or File Name.");
            }

            var result = this.GetMelodyRecord(melody);
            if (result == null)
            {
                throw new ArgumentException("Must provide a valid Melody Id or File Name.");
            }

            if (!result.IsUserMelody || !result.UserId.Equals(sessionUser.Id))
            {
                throw new ArgumentException("Requested melody was not created by this User. Cannot delete this record.");
            }

            // delete melody
            var dal = new Melodies();
            dal.DeleteMelody(result.Id);

            var check = this.GetMelodyRecord(result);
            if (check != null)
            {
                throw new DataException(string.Format("Failed to delete Melody {0}.", result.Id));
            }

            // delete melody categories
            this.DeleteMelodyCategories(result.Id);
        }

        #endregion Public Methods

        #region Private Methods

        private MelodyLoopPart CreateLoopPart(MelodyLoopPart part)
        {
            // TODO:
            throw new NotImplementedException();
        }

        private IList<MelodyLoopPart> GetMelodyLoopParts()
        {
            // TODO:
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the melody record.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Must provide a valid Melody to determine if it is a Base Melody.</exception>
        /// <exception cref="System.Data.DataException">Could not find Melody.</exception>
        private Melody GetMelodyRecord(Melody melody)
        {
            Melody record = null;

            var dal = new Melodies();
            if (!melody.Id.Equals(default(int)))
            {
                record = dal.GetMelodyById(melody.Id);
            }

            if (record == null && !string.IsNullOrWhiteSpace(melody.FileName))
            {
                record = dal.GetMelodyByFileName(melody.FileName);
            }

            return record;
        }

        /// <summary>
        /// Gets the name of the melody by file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private Melody GetMelodyByFileName(string fileName)
        {
            var dal = new Melodies();
            var result = dal.GetMelodyByFileName(fileName);

            return result;
        }

        /// <summary>
        /// Gets the name of the melody by user identifier and.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private Melody GetMelodyByUserIdAndName(Guid userId, string name)
        {
            var dal = new Melodies();
            var result = dal.GetMelodyByUserIdAndFileName(userId, name);

            return result;
        }

        /// <summary>
        /// Gets the base melody.
        /// </summary>
        /// <param name="userMelody">The user melody.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Must provide a valid Melody to determine if it is a Base Melody.</exception>
        /// <exception cref="System.Data.DataException">Could not find the Melody requested.</exception>
        private Melody GetBaseMelody(Melody userMelody)
        {
            if (userMelody == null
                || (userMelody.Id.Equals(default(int))
                    && string.IsNullOrWhiteSpace(userMelody.FileName)
                    && (userMelody.BaseMelodyId == null 
                        || userMelody.BaseMelodyId.Equals(default(int)))))
            {
                throw new ArgumentException("Must provide a valid Melody to determine if it is a Base Melody.");
            }

            var dal = new Melodies();
            Melody record = null;

            if (userMelody.BaseMelodyId != null 
                && !userMelody.BaseMelodyId.Equals(default(int)))
            {
                record = dal.GetMelodyById((int)userMelody.BaseMelodyId);
            }

            if (record == null)
            {
                // search parent record for base id
                var parent = this.GetMelodyRecord(userMelody);
                if (parent != null
                    && parent.BaseMelodyId != null
                    && !parent.BaseMelodyId.Equals(default(int)))
                {
                    record = dal.GetMelodyById((int)parent.BaseMelodyId);
                }
            }

            if (record == null)
            {
                throw new DataException("Could not find the Melody requested.");
            }

            record.Categories = this.GetMelodyCategories(record);
            record.FilePath = this.GetMelodyFilePath(record);

            return record;
        }

        /// <summary>
        /// Gets the melody file path.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">A Melody with a valid File Name must be provided to retrieve a path.</exception>
        private string GetMelodyFilePath(Melody melody)
        {
            if (melody == null || string.IsNullOrWhiteSpace(melody.FilePath))
            {
                throw new ArgumentException("A Melody with a valid File Name must be provided to retrieve a path.");
            }

            return string.Format("{0}/{1}/{2}", 
                Settings.Default.BaseFileUploadFolder, 
                Settings.Default.AudioFileUploadFolder, 
                melody.FilePath);
        }

        /// <summary>
        /// Gets the melody categories.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// A valid Melody must be provided to retrieve its Categories.
        /// or
        /// A valid Melody must be provided to retrieve its Categories.
        /// </exception>
        private IList<Category> GetMelodyCategories(Melody melody)
        {
            if (melody == null
                || (melody.Id.Equals(default(int))
                    && string.IsNullOrWhiteSpace(melody.FileName)))
            {
                throw new ArgumentException("A valid Melody must be provided to retrieve its Categories.");
            }

            var record = this.GetMelodyRecord(melody);
            if (record == null)
            {
                throw new ArgumentException("A valid Melody must be provided to retrieve its Categories.");
            }

            // get melody categories
            var melCatDal = new MelodyCategories();
            var melCats = melCatDal.GetMelodyCategoriesByMelodyId(melody.Id);
            if (melCats == null || !melCats.Any())
            {
                return null;
            }

            var results = new List<Category>();

            // get categories
            var catDal = new Categories();
            foreach (var melodyCategory in melCats)
            {
                var cat = catDal.GetCategoryById(melodyCategory.CategoryId);
                results.Add(cat);
            }

            // return categories
            return results;
        }

        /// <summary>
        /// Saves the melody categories.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        /// <param name="categories">The categories.</param>
        private void SaveMelodyCategories(int melodyId, IEnumerable<Category> categories)
        {
            var catBll = new CategoryBLL();
            var dal = new MelodyCategories();
            foreach (var category in categories)
            {
                var cat = catBll.GetCategory(category);
                if (cat == null) continue;

                dal.AddMelodyCategory(new MelodyCategory
                {
                    CategoryId = cat.Id,
                    MelodyId = melodyId,
                    DateCreated = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Deletes the melody categories.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        private void DeleteMelodyCategories(int melodyId)
        {
            var dal = new MelodyCategories();
            dal.DeleteMelodyCategoriesByMelodyId(melodyId);
        }

        #endregion Private Methods
    }
}

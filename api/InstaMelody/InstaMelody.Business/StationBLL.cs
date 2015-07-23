using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Model.Enums;

namespace InstaMelody.Business
{
    public class StationBLL
    {
        #region Public Methods

        /// <summary>
        /// Creates the station.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="image">The image.</param>
        /// <param name="categories">The categories.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">A Station with the provided information already exists.</exception>
        public object CreateStation(Station station, Guid sessionToken, Image image = null, IList<Category> categories = null)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            // check input for validation errors
            var validationErrors = Model.Utilities.Validate(station).ToList();
            if (validationErrors.Any())
            {
                var sb = new StringBuilder();
                foreach (var error in validationErrors)
                {
                    sb.AppendFormat("{0}; ", error);
                }
                throw new ArgumentException(sb.ToString());
            }

            var clonedStation = station.Clone();

            clonedStation.UserId = sessionUser.Id;

            var foundStation = TryGetStation(clonedStation);
            if (foundStation != null)
            {
                throw new ArgumentException("A Station with the provided information already exists.");
            }

            clonedStation.DateCreated = DateTime.UtcNow;
            clonedStation.DateModified = DateTime.UtcNow;
            clonedStation.IsDeleted = false;

            // create station
            var dal = new Stations();
            var createdStation = dal.CreateStation(clonedStation);

            // add station to categories
            if (categories != null && categories.Any())
            {
                var addedCats = AddStationCategories(createdStation.Id, categories);
                if (addedCats.Any())
                {
                    createdStation.Categories = addedCats;
                }
            }

            if (image == null)
            {
                return createdStation;
            }

            // return station and upload token
            var addedImage = AddUpdateStationImage(createdStation, image);
            createdStation.StationImageId = addedImage.Item1.Id;
            createdStation.Image = addedImage.Item1;

            return new ApiStationFileUpload
            {
                Station = createdStation,
                FileUploadToken = addedImage.Item2
            };
        }

        /// <summary>
        /// Finds the station.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        public Station FindStation(Station station, Guid sessionToken)
        {
            Utilities.GetUserBySession(sessionToken);

            var foundStation = TryGetStation(station);
            if (foundStation == null)
            {
                throw new DataException("Could not find requested Station.");
            }

            return GetStationWithImageAndCategories(foundStation);
        }

        /// <summary>
        /// Gets all stations.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Cannot find requested Stations.</exception>
        public IList<Station> GetAllStations(Guid sessionToken)
        {
            Utilities.GetUserBySession(sessionToken);

            var dal = new Stations();
            var stations = dal.GetAllStations();

            if (stations == null || !stations.Any())
            {
                throw new DataException("Cannot find requested Stations.");
            }

            return stations.Select(GetStationWithImageAndCategories).ToList();
        }

        /// <summary>
        /// Gets the stations by user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Cannot find User.</exception>
        /// <exception cref="System.Data.DataException">Cannot find requested Stations.</exception>
        public IList<Station> GetStationsByUser(User user, Guid sessionToken)
        {
            Utilities.GetUserBySession(sessionToken);

            var userBll = new UserBLL();
            var foundUser = userBll.FindUser(user);

            if (foundUser == null)
            {
                throw new ArgumentException("Cannot find User.");
            }

            var dal = new Stations();
            var stations = dal.GetStationsByUser(foundUser.Id);

            if (stations == null || !stations.Any())
            {
                throw new DataException("Cannot find requested Stations.");
            }

            return stations.Select(GetStationWithImageAndCategories).ToList();
        }

        /// <summary>
        /// Gets the stations by user.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Cannot find requested Stations.</exception>
        public IList<Station> GetStationsByUser(Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var dal = new Stations();
            var stations = dal.GetStationsByUser(sessionUser.Id);

            if (stations == null || !stations.Any())
            {
                throw new DataException("Cannot find requested Stations.");
            }

            return stations.Select(GetStationWithImageAndCategories).ToList();
        }

        /// <summary>
        /// Gets the stations by category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Cannot find requested Category.</exception>
        /// <exception cref="System.Data.DataException">Cannot find requested Stations.</exception>
        public IList<Station> GetStationsByCategory(Category category, Guid sessionToken)
        {
            Utilities.GetUserBySession(sessionToken);

            var catBll = new CategoryBLL();
            var cat = catBll.GetCategory(category);

            if (cat == null)
            {
                throw new ArgumentException("Cannot find requested Category.");
            }

            var dal = new Stations();
            var stations = dal.GetStationsByCategoryId(cat.Id);

            if (stations == null || !stations.Any())
            {
                throw new DataException("Cannot find requested Stations.");
            }

            return stations.Select(GetStationWithImageAndCategories).ToList();
        }

        /// <summary>
        /// Updates the station.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="image">The image.</param>
        /// <param name="categories">The categories.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Station not found.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public object UpdateStation(Station station, Guid sessionToken, Image image = null, IList<Category> categories = null)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            // check input for validation errors
            var validationErrors = Model.Utilities.Validate(station).ToList();
            if (validationErrors.Any())
            {
                var sb = new StringBuilder();
                foreach (var error in validationErrors)
                {
                    sb.AppendFormat("{0}; ", error);
                }
                throw new ArgumentException(sb.ToString());
            }

            var dal = new Stations();

            // find station
            var foundStation = dal.GetStationById(station.Id);
            if (foundStation == null)
            {
                throw new ArgumentException("Station not found.");
            }

            if (!foundStation.UserId.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException(
                    string.Format("User with Token: {0} is not authroized to update Station Id: {1}.", 
                        sessionToken, foundStation.Id));
            }

            var clonedStation = station.Clone();
            if (!station.Name.Equals(foundStation.Name))
            {
                clonedStation.Name = station.Name;
            }
            clonedStation.UserId = foundStation.UserId;
            clonedStation.StationImageId = foundStation.StationImageId;
            clonedStation.DateCreated = foundStation.DateCreated;
            clonedStation.DateModified = DateTime.UtcNow;
            clonedStation.IsDeleted = false;

            var updatedStation = dal.UpdateStation(clonedStation);

            // add station to categories
            if (categories != null && categories.Any())
            {
                AddStationCategories(updatedStation.Id, categories);
            }
            updatedStation.Categories = GetAllStationCategories(updatedStation.Id);

            if (image == null)
            {
                return updatedStation;
            }

            // return station and upload token
            var addedImage = AddUpdateStationImage(updatedStation, image);
            updatedStation.StationImageId = addedImage.Item1.Id;
            updatedStation.Image = addedImage.Item1;

            return new ApiStationFileUpload
            {
                Station = updatedStation,
                FileUploadToken = addedImage.Item2
            };
        }

        /// <summary>
        /// Deletes the station.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <exception cref="System.ArgumentException">Station not found.</exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public void DeleteStation(Station station, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            station.UserId = sessionUser.Id;

            var foundStation = TryGetStation(station);
            if (foundStation == null)
            {
                throw new ArgumentException("Station not found.");
            }

            if (!foundStation.UserId.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException(
                    string.Format("User with Token: {0} is not authorized to delete this Station.",
                        sessionToken));
            }

            var dal = new Stations();
            dal.DeleteStation(foundStation.Id);

            // delete station image
            if (foundStation.StationImageId != null
                && !foundStation.StationImageId.Equals(default(int)))
            {
                DeleteStationImage((int)foundStation.StationImageId);
            }

            // TODO: delete station messages
        }

        /// <summary>
        /// Removes the station from categories.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="categories">The categories.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Could not find a Station with the information provided.</exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public Station RemoveStationFromCategories(Station station, IList<Category> categories, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var foundStation = TryGetStation(station, sessionUser.Id);
            if (foundStation == null)
            {
                throw new ArgumentException("Could not find a Station with the information provided.");
            }

            if (!foundStation.UserId.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException(
                    string.Format("User with Token: {0} is not authroized to update Station Id: {1}.",
                        sessionToken, foundStation.Id));
            }

            var dal = new Stations();

            var catBll = new CategoryBLL();
            foreach (var category in categories)
            {
                try
                {
                    var cat = catBll.GetCategory(category);
                    dal.RemoveStationFromCategory(foundStation.Id, cat.Id);
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            // get all station categories
            foundStation.Categories = GetAllStationCategories(foundStation.Id);

            return GetStationWithImage(foundStation);
        }

        /// <summary>
        /// Follows the station.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Station not found.</exception>
        public Station FollowStation(Station station, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var foundStation = TryGetStation(station);
            if (foundStation == null)
            {
                throw new ArgumentException("Station not found.");
            }

            var dal = new Stations();
            dal.FollowStation(sessionUser.Id, foundStation.Id);

            return GetStationWithImageAndCategories(foundStation);
        }

        /// <summary>
        /// Unfollows the station.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Station not found.</exception>
        public Station UnfollowStation(Station station, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var foundStation = TryGetStation(station);
            if (foundStation == null)
            {
                throw new ArgumentException("Station not found.");
            }

            var dal = new Stations();
            dal.UnfollowStation(sessionUser.Id, foundStation.Id);

            return GetStationWithImageAndCategories(foundStation);
        }

        /// <summary>
        /// Gets the station followers.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Station not found.</exception>
        public IList<User> GetStationFollowers(Station station, Guid sessionToken)
        {
            Utilities.GetUserBySession(sessionToken);

            var foundStation = TryGetStation(station);
            if (foundStation == null)
            {
                throw new ArgumentException("Station not found.");
            }

            var dal = new Stations();

            var followers = dal.GetFollowersByStationId(foundStation.Id);
            foreach (var follower in followers)
            {
                follower.StripSensitiveInfo();
            }

            return followers;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Finds the station.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">No station provided.</exception>
        private Station TryGetStation(Station station, Guid? userId = null)
        {
            if (station == null)
            {
                throw new ArgumentException("No station provided.");
            }

            var dal = new Stations();
            Station foundStation = null;

            if (!station.Id.Equals(default(int)))
            {
                foundStation = dal.GetStationById(station.Id);
            }

            if (foundStation == null
                && !string.IsNullOrEmpty(station.Name))
            {
                if (station.UserId.Equals(default(Guid)) && userId != null)
                {
                    station.UserId = (Guid)userId;
                }
                foundStation = dal.GetStationByNameAndUserId(station.Name, station.UserId);
            }

            return foundStation;
        }

        /// <summary>
        /// Gets the station with image.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <returns></returns>
        private Station GetStationWithImage(Station station)
        {
            if (station.StationImageId != null
                && !station.StationImageId.Equals(default(int)))
            {
                var imageBll = new FileBLL();
                var image = imageBll.GetImage(new Image
                {
                    Id = (int)station.StationImageId
                });

                if (image != null)
                {
                    station.Image = image;
                }
            }
            return station;
        }

        /// <summary>
        /// Gets the station with image and categories.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <returns></returns>
        private Station GetStationWithImageAndCategories(Station station)
        {
            var stationWithImage = GetStationWithImage(station);
            stationWithImage.Categories = GetAllStationCategories(station.Id);

            return stationWithImage;
        }

        /// <summary>
        /// Adds the station categories.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <param name="categories">The categories.</param>
        /// <returns></returns>
        private IList<Category> AddStationCategories(int stationId, IList<Category> categories)
        {
            var catBll = new CategoryBLL();
            var dal = new Stations();
            foreach (var category in categories)
            {
                try
                {
                    var cat = catBll.GetCategory(category);
                    dal.AddStationToCategory(stationId, cat.Id);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return dal.GetCategoriesByStationId(stationId);
        }

        /// <summary>
        /// Gets all station categories.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <returns></returns>
        private IList<Category> GetAllStationCategories(int stationId)
        {
            var dal = new Stations();
            return dal.GetCategoriesByStationId(stationId);
        }

        /// <summary>
        /// Adds the update station image.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        private Tuple<Image,FileUploadToken> AddUpdateStationImage(Station station, Image image)
        {
            if (image == null)
            {
                throw new ArgumentException("Cannot Add or Update a Station Image with a NULL Image.");
            }

            var fileBll = new FileBLL();

            // delete existing image
            if (station.StationImageId != null)
            {
                var foundImage = fileBll.GetImage(new Image{ Id = (int)station.StationImageId });
                if (foundImage != null)
                {
                    fileBll.DeleteImage(foundImage);
                }
            }

            // upload new image
            var addedImage = fileBll.AddImage(image);

            // update station with new image id
            var dal = new Stations();
            dal.UpdateStationImage(station.Id, addedImage.Id);

            // create file upload token
            var uploadBll = new FileBLL();
            var uploadToken = uploadBll.CreateToken(new FileUploadToken
            {
                UserId = station.UserId,
                MediaType = FileUploadTypeEnum.UserImage,
                FileName = addedImage.FileName
            });

            return new Tuple<Image, FileUploadToken>(addedImage, uploadToken);
        }

        /// <summary>
        /// Deletes the station image.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        private void DeleteStationImage(int imageId)
        {
            var fileBll = new FileBLL();
            fileBll.DeleteImage(new Image {Id = imageId});
        }

        #endregion Private Methods
    }
}

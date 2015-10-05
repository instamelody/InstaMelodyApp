using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class Stations : DataAccess
    {
        /// <summary>
        /// Creates the station.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <returns></returns>
        public Station CreateStation(Station station)
        {
            var query = @"INSERT INTO dbo.Stations (UserId, StationImageId, Name, DateCreated, DateModified)
                        VALUES (@UserId, @StationImageId, @Name, @DateCreated, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = station.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "StationImageId",
                    Value = station.StationImageId != null
                        ? (object)station.StationImageId
                        : DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = station.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = station.DateCreated > DateTime.MinValue
                        ? station.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                return GetStationById(Convert.ToInt32(obj));
            }

            throw new DataException("Failed to create a new Station.");
        }

        /// <summary>
        /// Gets all stations.
        /// </summary>
        /// <returns></returns>
        public IList<Station> GetAllStations(bool includeUnpublished = false)
        {
            var query = @"SELECT * FROM dbo.Stations
                        WHERE IsDeleted = 0 AND IsPublished = 1";

            if (includeUnpublished)
            {
                query = @"SELECT * FROM dbo.Stations
                        WHERE IsDeleted = 0";
            }

            return GetRecordSet<Station>(query);
        }

        /// <summary>
        /// Gets the station by identifier.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <returns></returns>
        public Station GetStationById(int stationId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Stations
                        WHERE IsDeleted = 0 AND Id = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<Station>(query, parameters.ToArray());
        }

        /// <summary>
        /// Publishes the station.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <returns></returns>
        public Station PublishStation(int stationId)
        {
            var query = @"UPDATE dbo.Stations
                        SET IsPublished = 1
                        WHERE IsDeleted = 0 AND Id = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return GetStationById(stationId);
        }

        /// <summary>
        /// Unpublishes the station.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <returns></returns>
        public Station UnpublishStation(int stationId)
        {
            var query = @"UPDATE dbo.Stations
                        SET IsPublished = 0
                        WHERE IsDeleted = 0 AND Id = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return GetStationById(stationId);
        }

        /// <summary>
        /// Gets the station by name and user identifier.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Station GetStationByNameAndUserId(string name, Guid userId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Stations
                        WHERE IsDeleted = 0 AND Name = @Name AND UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<Station>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the stations by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<Station> GetStationsByUser(Guid userId)
        {
            var query = @"SELECT * FROM dbo.Stations
                        WHERE IsDeleted = 0 AND UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Station>(query, parameters.ToArray());
        }

        /// <summary>
        /// Updates the station.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <returns></returns>
        public Station UpdateStation(Station station)
        {
            var query = @"UPDATE dbo.Stations
                        SET UserId = @UserId, StationImageId = @StationImageId,
                        Name = @Name, DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = station.Id,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = station.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "StationImageId",
                    Value = station.StationImageId != null
                        ? (object)station.StationImageId
                        : DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = station.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = station.DateModified > DateTime.MinValue
                        ? station.DateModified
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
            return GetStationById(station.Id);
        }

        /// <summary>
        /// Updates the station image.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <param name="stationImageId">The station image identifier.</param>
        /// <returns></returns>
        public Station UpdateStationImage(int stationId, int stationImageId)
        {
            var query = @"UPDATE dbo.Stations
                        SET StationImageId = @StationImageId, DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "StationImageId",
                    Value = stationImageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
            return GetStationById(stationId);
        }

        /// <summary>
        /// Deletes the station.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        public void DeleteStation(int stationId)
        {
            var query = @"UPDATE dbo.Stations
                        SET IsDeleted = 1, DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            DeleteStationCategoriesByStationId(stationId);
            DeleteAllFollowersByStationId(stationId);
        }

        #region Station Categories

        /// <summary>
        /// Adds the station to category.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public void AddStationToCategory(int stationId, int categoryId)
        {
            var query = @"INSERT INTO dbo.StationCategories (StationId, CategoryId, DateCreated)
                        VALUES (@StationId, @CategoryId, @DateCreated)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the stations by category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IList<Station> GetStationsByCategoryId(int categoryId)
        {
            var query = @"SELECT s.* FROM dbo.Stations s
                        JOIN dbo.StationCategories sc
                        ON s.Id = sc.StationId
                        WHERE s.IsDeleted = 0 AND sc.IsDeleted = 0
                        AND sc.CategoryId = @CategoryId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Station>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the categories by station identifier.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IList<Category> GetCategoriesByStationId(int stationId)
        {
            var query = @"SELECT c.* FROM dbo.Categories c
                        JOIN dbo.StationCategories sc
                        ON c.Id = sc.CategoryId
                        WHERE c.IsDeleted = 0 AND sc.IsDeleted = 0
                        AND sc.StationId = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
            };

            return GetRecordSet<Category>(query, parameters.ToArray());
        }

        /// <summary>
        /// Removes the station from category.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <param name="categoryId">The category identifier.</param>
        public void RemoveStationFromCategory(int stationId, int categoryId)
        {
            var query = @"UPDATE dbo.StationCategories SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND StationId = @StationId 
                        AND CategoryId = @CategoryId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the station categories by station identifier.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        public void DeleteStationCategoriesByStationId(int stationId)
        {
            var query = @"UPDATE dbo.StationCategories SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND StationId = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion Station Categories

        #region Station Likes

        /// <summary>
        /// Likes the station.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Station LikeStation(int stationId, Guid userId)
        {
            var query = @"INSERT INTO dbo.StationLikes (StationId, UserId, DateCreated)
                        VALUES (@StationId, @CategoryId, @DateCreated)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return GetStationById(stationId);
        }

        /// <summary>
        /// Unlikes the station.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public Station UnlikeStation(int stationId, Guid userId)
        {
            var query = @"UPDATE dbo.StationLikes
                        SET IsDeleted = 1
                        WHERE StationId = @StationId AND UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return GetStationById(stationId);
        }

        /// <summary>
        /// Determines whether [is station liked by user] [the specified station identifier].
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to retrieve likes for Station and User.</exception>
        public bool IsStationLikedByUser(int stationId, Guid userId)
        {
            var query = @"SELECT COUNT(*) FROM dbo.StationLikes
                        WHERE StationId = @StationId AND UserId = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                return Convert.ToInt32(obj) > 0;
            }
            throw new DataException("Failed to retrieve likes for Station and User.");
        }

        /// <summary>
        /// Gets the likes by station.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <returns></returns>
        public IList<StationLike> GetLikesByStation(int stationId)
        {
            var query = @"SELECT * FROM dbo.StationLikes
                        WHERE StationId = @StationId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<StationLike>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the likes count for station.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public int GetLikesCountForStation(int stationId)
        {
            var query = @"SELECT COUNT(Id) FROM dbo.StationLikes
                        WHERE StationId = @StationId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                return Convert.ToInt32(obj);
            }
            throw new DataException(string.Format("Failed to get a count of likes for Station Id: {0}.", stationId));
        }

        /// <summary>
        /// Gets the liked stations by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<Station> GetLikedStationsByUser(Guid userId)
        {
            var query = @"SELECT s.* FROM Stations s
                        INNER JOIN StationLikes l
                        ON s.Id = l.StationId
                        WHERE l.UserId = @UserId 
                        AND l.IsDeleted = 0 AND s.IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Station>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the most liked stations.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public IList<Station> GetMostLikedStations(int limit = int.MaxValue)
        {
            var query = @"SELECT TOP(@Limit) s.*, COUNT(l.StationId) Likes
                        FROM StationLikes l
                        RIGHT JOIN Stations s
                        ON s.Id = l.StationId
                        WHERE s.IsDeleted = 0 AND s.IsPublished = 1 AND (l.IsDeleted = 0 OR l.IsDeleted IS NULL)
                        GROUP BY l.StationId, s.Id, s.UserId, s.StationImageId, 
                            s.DateCreated, s.DateModified, s.IsPublished, s.IsDeleted, s.Name
                        ORDER BY Likes DESC, s.Name";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Limit",
                    Value = limit,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Station>(query, parameters.ToArray());
        }

        #endregion Station Likes

        #region Station Followers

        /// <summary>
        /// Follows the station.
        /// </summary>
        /// <param name="followerId">The follower identifier.</param>
        /// <param name="stationId">The station identifier.</param>
        public void FollowStation(Guid followerId, int stationId)
        {
            var query = @"INSERT INTO dbo.StationFollowers (StationId, UserId, DateCreated)
                        VALUES (@StationId, @UserId, @DateCreated)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = followerId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Uns the follow station.
        /// </summary>
        /// <param name="followerId">The follower identifier.</param>
        /// <param name="stationId">The station identifier.</param>
        public void UnfollowStation(Guid followerId, int stationId)
        {
            var query = @"UPDATE dbo.StationFollowers
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND StationId = @StationId
                        AND UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = followerId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Doeses the user follow station.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="stationId">The station identifier.</param>
        /// <returns></returns>
        public bool DoesUserFollowStation(Guid userId, int stationId)
        {
            var query = @"SELECT COUNT(*) FROM dbo.StationFollowers
                        WHERE IsDeleted = 0 AND StationId = @StationId
                        AND UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj) && Convert.ToBoolean(obj))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the followers by station identifier.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IList<User> GetFollowersByStationId(int stationId)
        {
            var query = @"SELECT u.* FROM dbo.Users u
                        JOIN dbo.StationFollowers sf
                        ON u.Id = sf.UserId
                        WHERE u.IsDeleted = 0 AND sf.IsDeleted = 0
                        AND sf.StationId = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<User>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the followed stations by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IList<Station> GetFollowedStationsByUserId(Guid userId)
        {
            var query = @"SELECT s.* FROM dbo.Stations s
                        JOIN dbo.StationFollowers sf
                        ON s.Id = sf.StationId
                        WHERE s.IsDeleted = 0 AND sf.IsDeleted = 0
                        AND sf.UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Station>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes all followers by station identifier.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        public void DeleteAllFollowersByStationId(int stationId)
        {
            var query = @"UPDATE dbo.StationFollowers
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND StationId = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion Station Followers
    }
}

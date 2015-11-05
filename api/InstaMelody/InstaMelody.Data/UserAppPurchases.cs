using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class UserAppPurchases : DataAccess
    {
        /// <summary>
        /// Adds the purchase receipt.
        /// </summary>
        /// <param name="receipt">The receipt.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create a new App Purchase Receipt.</exception>
        public UserAppPurchaseReceipt AddPurchaseReceipt(UserAppPurchaseReceipt receipt)
        {
            var query = @"INSERT INTO dbo.UserAppPurchaseReceipts (UserId, ReceiptData, DateCreated, DateModified)
                        VALUES (@UserId, @ReceiptData, @DateCreated, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = receipt.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "ReceiptData",
                    Value = receipt.ReceiptData,
                    SqlDbType = SqlDbType.VarChar,
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

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                return GetAppPurchaseReceiptById(Convert.ToInt32(obj));
            }

            throw new DataException("Failed to create a new App Purchase Receipt.");
        }

        /// <summary>
        /// Gets the application purchase receipt by identifier.
        /// </summary>
        /// <param name="receiptId">The receipt identifier.</param>
        /// <returns></returns>
        public UserAppPurchaseReceipt GetAppPurchaseReceiptById(int receiptId)
        {
            var query = @"SELECT * FROM dbo.UserAppPurchaseReceipts
                        WHERE Id = @ReceiptId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ReceiptId",
                    Value = receiptId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<UserAppPurchaseReceipt>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the application purchase receipt by receipt data.
        /// </summary>
        /// <param name="receiptData">The receipt data.</param>
        /// <returns></returns>
        public UserAppPurchaseReceipt GetAppPurchaseReceiptByReceiptData(string receiptData)
        {
            var query = @"SELECT * FROM dbo.UserAppPurchaseReceipts
                        WHERE ReceiptData = @ReceiptData AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ReceiptData",
                    Value = receiptData,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<UserAppPurchaseReceipt>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the application purchase receipts by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<UserAppPurchaseReceipt> GetAppPurchaseReceiptsByUserId(Guid userId)
        {
            var query = @"SELECT * FROM dbo.UserAppPurchaseReceipts
                        WHERE UserId = @UserId AND IsDeleted = 0";

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

            return GetRecordSet<UserAppPurchaseReceipt>(query, parameters.ToArray());
        }

        /// <summary>
        /// Invalidates the application purchase receipt.
        /// </summary>
        /// <param name="receiptId">The receipt identifier.</param>
        /// <returns></returns>
        public UserAppPurchaseReceipt InvalidateAppPurchaseReceipt(int receiptId)
        {
            var query = @"UPDATE dbo.UserAppPurchaseReceipts
                        SET IsInvalid = 1, DateModified = @DateModified
                        WHERE Id = @ReceiptId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ReceiptId",
                    Value = receiptId,
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

            return GetAppPurchaseReceiptById(receiptId);
        }

        /// <summary>
        /// Deletes the application purchase receipt.
        /// </summary>
        /// <param name="receiptId">The receipt identifier.</param>
        public void DeleteAppPurchaseReceipt(int receiptId)
        {
            var query = @"UPDATE dbo.UserAppPurchaseReceipts
                        SET IsDeleted = 1, DateModified = @DateModified
                        WHERE Id = @ReceiptId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ReceiptId",
                    Value = receiptId,
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
        }
    }
}

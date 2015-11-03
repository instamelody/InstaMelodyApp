CREATE DATABASE InstaMelody
GO

USE InstaMelody
GO

-- START Tables

CREATE TABLE dbo.Images
	(Id int IDENTITY(1,1) PRIMARY KEY,
	FileName varchar(255) NOT NULL,
	DateCreated datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.Users
	(Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    UserImageId int NULL,
    CONSTRAINT FK_Users_UserImageId FOREIGN KEY (UserImageId)
		REFERENCES Images (Id),
    UserCoverImageId int NULL,
    CONSTRAINT FK_Users_UserCoverImageId FOREIGN KEY (UserCoverImageId)
		REFERENCES Images (Id),
    EmailAddress varchar(320) NULL,
    DisplayName varchar(32) NOT NULL,
    FirstName varchar(64) NULL,
    LastName varchar(64) NULL,
    PhoneNumber varchar(28) NULL,
    IsFemale bit NOT NULL DEFAULT 0,
    DateOfBirth datetime NULL,
    HashSalt varchar(64) NULL,
    Password varchar(255) NULL,
    IsSubscribed bit NOT NULL DEFAULT 0,
    TwitterUsername varchar(128) NULL,
    TwitterUserId varchar(128) NULL,
    TwitterToken varchar(255) NULL,
    TwitterSecret varchar(255) NULL,
    FacebookUserId varchar(128) NULL,
    FacebookToken varchar(255) NULL,
    LastLoginSuccess datetime NULL,
    LastLoginFailure datetime NULL,
    NumberLoginFailures int NOT NULL DEFAULT 0,
    IsLocked bit NOT NULL DEFAULT 0,
    DateCreated datetime NOT NULL,
    DateModified datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
CREATE UNIQUE NONCLUSTERED INDEX UIX_Users_DisplayName ON dbo.Users(DisplayName)
GO

CREATE TABLE dbo.UserAppPurchaseReceipts
	(Id int IDENTITY(1,1) PRIMARY KEY,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserAppPurchaseReceipts_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	ReceiptData varchar(max) NOT NULL,
	IsInvalid bit NOT NULL default 0,
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
	IsDeleted bit NOT NULL default 0)
GO

CREATE TABLE dbo.FileUploadTokens
	(Token UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_FileUploadTokens_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	FileName varchar(255) NOT NULL,
	MediaType varchar(32) NOT NULL,
	DateExpires datetime NOT NULL,
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.UserSessions
	(Token UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserSessions_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	DeviceToken varchar(64) NULL,
	LastActivity datetime NOT NULL,
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.UserFriends
	(Id int IDENTITY(1,1) PRIMARY KEY,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserFriends_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	RequestorId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserFriends_RequestorId FOREIGN KEY (RequestorId)
		REFERENCES Users (Id),
	IsPending bit NOT NULL DEFAULT 1,
	IsDenied bit NOT NULL DEFAULT 0,
	DateApproved datetime NULL,
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.Categories
	(Id int IDENTITY(1,1) PRIMARY KEY,
	ParentId int NULL,
	Name varchar(128) NOT NULL,
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

-- START MELODIES

CREATE TABLE dbo.FileGroups
	(Id int IDENTITY(1,1) PRIMARY KEY,
	Name varchar(128) NOT NULL,
	IsLockedContent bit NOT NULL DEFAULT 0,
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.Melodies
	(Id int IDENTITY(1,1) PRIMARY KEY,
	IsUserCreated bit NOT NULL DEFAULT 0,
	Name varchar(128) NOT NULL,
	Description text NULL,
	FileName varchar(255) NOT NULL,
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.MelodyFileGroups
	(Id int IDENTITY(1,1) PRIMARY KEY,
	MelodyId int NOT NULL,
	CONSTRAINT FK_MelodyFileGroups_MelodyId FOREIGN KEY (MelodyId)
		REFERENCES Melodies (Id),
	FileGroupId int NOT NULL,
	CONSTRAINT FK_MelodyFileGroups_FileGroupId FOREIGN KEY (FileGroupId)
		REFERENCES FileGroups (Id),
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.MelodyCategories
	(Id int IDENTITY(1,1) PRIMARY KEY,
	MelodyId int NOT NULL,
	CONSTRAINT FK_MelodyCategories_MelodyId FOREIGN KEY (MelodyId)
		REFERENCES Melodies (Id),
	CategoryId int NOT NULL,
	CONSTRAINT FK_MelodyCategories_CategoryId FOREIGN KEY (CategoryId)
		REFERENCES Categories (Id),
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.UserMelodies
	(Id UNIQUEIDENTIFIER PRIMARY KEY,
	Name varchar(128) NOT NULL,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserMelodies_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	IsExplicit bit NOT NULL DEFAULT 0,
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.UserMelodyParts
	(Id int IDENTITY(1,1) PRIMARY KEY,
	UserMelodyId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserMelodyParts_UserMelodyId FOREIGN KEY (UserMelodyId)
		REFERENCES UserMelodies (Id),
	MelodyId int NOT NULL,
	CONSTRAINT FK_UserMelodyParts_MelodyId FOREIGN KEY (MelodyId)
		REFERENCES Melodies (Id),
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.UserLoops
	(Id UNIQUEIDENTIFIER PRIMARY KEY,
	Name varchar(128) NOT NULL,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserLoops_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	IsExplicit bit NOT NULL DEFAULT 0,
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.UserLoopParts
	(Id int IDENTITY(1,1) PRIMARY KEY,
	UserLoopId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserLoopParts_UserLoopId FOREIGN KEY (UserLoopId)
		REFERENCES UserLoops (Id),
	UserMelodyId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserLoopParts_UserMelodyId FOREIGN KEY (UserMelodyId)
		REFERENCES UserMelodies (Id),
	OrderIndex int NOT NULL DEFAULT 1,
	StartTime bigint NULL,
	StartEffect varchar(56) NULL,
	StartEffectDuration bigint NULL,
	EndTime bigint NULL,
	EndEffect varchar(56) NULL,
	EndEffectDuration bigint NULL,
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

-- END MELODIES

-- START CHATS & MESSAGES

CREATE TABLE dbo.Videos
	(Id int IDENTITY(1,1) PRIMARY KEY,
	FileName varchar(255) NOT NULL,
	DateCreated datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.Messages
	(Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	ParentId UNIQUEIDENTIFIER NULL,
	Description text NULL,
	MediaType varchar(32) NOT NULL,
	IsRead bit NOT NULL DEFAULT 0,
	DateRead datetime NULL,
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.Chats
	(Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	ChatLoopId UNIQUEIDENTIFIER NULL,
	CONSTRAINT FK_Chats_UserLoopId FOREIGN KEY (ChatLoopId)
		REFERENCES UserLoops (Id),
	Name varchar(128) NULL,
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.ChatMessages
	(Id int IDENTITY(1,1) PRIMARY KEY,
	ChatId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_ChatMessages_ChatId FOREIGN KEY (ChatId)
		REFERENCES Chats (Id),
	MessageId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_ChatMessages_MessageId FOREIGN KEY (MessageId)
		REFERENCES Messages (Id),
	SenderId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_ChatMessages_SenderId FOREIGN KEY (SenderId)
		REFERENCES Users (Id),
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.ChatUsers
	(Id int IDENTITY(1,1) PRIMARY KEY,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_ChatMessages_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	ChatId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_ChatUsers_ChatId FOREIGN KEY (ChatId)
		REFERENCES Chats (Id),
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.MessageImages
	(Id int IDENTITY(1,1) PRIMARY KEY,
	MessageId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_MessageImages_MessageId FOREIGN KEY (MessageId)
		REFERENCES Messages (Id),
	ImageId int NOT NULL,
	CONSTRAINT FK_MessageImages_ImageId FOREIGN KEY (ImageId)
		REFERENCES Images (Id),
	DateCreated datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.MessageVideos
	(Id int IDENTITY(1,1) PRIMARY KEY,
	MessageId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_MessageVideos_MessageId FOREIGN KEY (MessageId)
		REFERENCES Messages (Id),
	VideoId int NOT NULL,
	CONSTRAINT FK_MessageVideos_VideoId FOREIGN KEY (VideoId)
		REFERENCES Videos (Id),
	DateCreated datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.MessageMelodies
	(Id int IDENTITY(1,1) PRIMARY KEY,
	MessageId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_MessageMelodies_MessageId FOREIGN KEY (MessageId)
		REFERENCES Messages (Id),
	UserMelodyId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_MessageMelodies_UserMelodyId FOREIGN KEY (UserMelodyId)
		REFERENCES UserMelodies (Id),
	DateCreated datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

-- END CHATS & MESSAGES

-- -- START STATIONS

CREATE TABLE dbo.Stations
	(Id int IDENTITY(1,1) PRIMARY KEY,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_Stations_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
    StationImageId int NULL,
    CONSTRAINT FK_Stations_StationImageId FOREIGN KEY (StationImageId)
		REFERENCES Images (Id),
	Name varchar(128) NOT NULL,
	IsPublished bit NOT NULL DEFAULT 0,
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.StationCategories
	(Id int IDENTITY(1,1) PRIMARY KEY,
	StationId int NOT NULL,
	CONSTRAINT FK_StationCategories_StationId FOREIGN KEY (StationId)
		REFERENCES Stations (Id),
	CategoryId int NOT NULL,
	CONSTRAINT FK_StationCategories_CategoryId FOREIGN KEY (CategoryId)
		REFERENCES Categories (Id),
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.StationLikes
	(Id int IDENTITY(1,1) PRIMARY KEY,
	StationId int NOT NULL,
	CONSTRAINT FK_StationLikes_StationId FOREIGN KEY (StationId)
		REFERENCES Stations (Id),
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_StationLikes_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.StationFollowers
	(Id int IDENTITY(1,1) PRIMARY KEY,
	StationId int NOT NULL,
	CONSTRAINT FK_StationFollowers_StationId FOREIGN KEY (StationId)
		REFERENCES Stations (Id),
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_StationFollowers_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.StationMessages
	(Id int IDENTITY(1,1) PRIMARY KEY,
	StationId int NOT NULL,
	CONSTRAINT FK_StationMessages_StationId FOREIGN KEY (StationId)
		REFERENCES Stations (Id),
	MessageId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_StationMessages_MessageId FOREIGN KEY (MessageId)
		REFERENCES Messages (Id),
	SenderId UNIQUEIDENTIFIER NULL,
	CONSTRAINT FK_StationMessages_SenderId FOREIGN KEY (SenderId)
		REFERENCES Users (Id),
	ParentId int NULL,
	IsPrivate bit NOT NULL DEFAULT 0,
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.StationMessageUserLikes
	(Id int IDENTITY(1,1) PRIMARY KEY,
	StationMessageId int NOT NULL,
	CONSTRAINT FK_StationMessageUserLikes_StationMessageId FOREIGN KEY (StationMessageId)
		REFERENCES StationMessages (Id),
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_StationMessageUserLikes_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	DateCreated datetime NOT NULL,
	IsDeleted bit NOT NULL DEFAULT 0)
GO

-- -- END STATIONS

-- END Tables

-- START SProcs

CREATE PROCEDURE ReindexUserMelodyParts
	@loopId UniqueIdentifier
AS

	DECLARE @currentIndex int = 1
	DECLARE @rowOrderIndex int
	DECLARE @rowId int
	DECLARE @count int

	SELECT * INTO #mytemp 
	FROM dbo.UserLoopParts 
	WHERE IsDeleted = 0 AND UserLoopId = @loopId 
	ORDER BY OrderIndex

	WHILE (1=1)
	BEGIN
		SELECT @count = COUNT(*) FROM #mytemp
		IF (@count = 0)
		BEGIN
			break
		END
		SELECT TOP 1 @rowOrderIndex = OrderIndex, @rowId = Id FROM #mytemp
		IF (@currentIndex < @rowOrderIndex)
		BEGIN
			UPDATE dbo.UserLoopParts SET OrderIndex = @currentIndex WHERE Id = @rowId
		END
		SET @currentIndex = @currentIndex + 1
		DELETE FROM #mytemp WHERE Id = @rowId
	END
	DROP TABLE #mytemp

GO

-- END SProcs
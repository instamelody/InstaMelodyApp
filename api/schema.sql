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
    EmailAddress varchar(320) NULL,
    DisplayName varchar(32) NOT NULL,
    FirstName varchar(64) NULL,
    LastName varchar(64) NULL,
    PhoneNumber varchar(28) NULL,
    HashSalt varchar(64) NULL,
    Password varchar(255) NULL,
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

CREATE TABLE dbo.MessageMelodies
	(Id int IDENTITY(1,1) PRIMARY KEY,
	MessageId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_MessageMelodies_MessageId FOREIGN KEY (MessageId)
		REFERENCES Messages (Id),
	MelodyId int NOT NULL,
	CONSTRAINT FK_MessageMelodies_MelodyId FOREIGN KEY (MelodyId)
		REFERENCES Melodies (Id),
	DateCreated datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

-- CREATE TABLE dbo.Stations
-- 	(Id int IDENTITY(1,1) PRIMARY KEY,
-- 	UserId UNIQUEIDENTIFIER NOT NULL,
-- 	CONSTRAINT FK_Stations_UserId FOREIGN KEY (UserId)
-- 		REFERENCES Users (Id),
--     StationImageId int NULL,
--     CONSTRAINT FK_Stations_StationImageId FOREIGN KEY (StationImageId)
-- 		REFERENCES Images (Id),
-- 	Name varchar(128) NOT NULL,
-- 	DateCreated datetime NOT NULL,
-- 	DateModified datetime NOT NULL,
--     IsDeleted bit NOT NULL DEFAULT 0)
-- GO

-- CREATE TABLE dbo.StationMessages
-- 	(Id int IDENTITY(1,1) PRIMARY KEY,
-- 	StationId int NOT NULL,
-- 	CONSTRAINT FK_StationMessages_StationId FOREIGN KEY (StationId)
-- 		REFERENCES Stations (Id),
-- 	MessageId UNIQUEIDENTIFIER NOT NULL,
-- 	CONSTRAINT FK_StationMessages_MessageId FOREIGN KEY (MessageId)
-- 		REFERENCES Messages (Id),
-- 	IsPrivate bit NOT NULL DEFAULT 0,
-- 	DateCreated datetime NOT NULL,
-- 	DateModified datetime NOT NULL,
--     IsDeleted bit NOT NULL DEFAULT 0)
-- GO

CREATE TABLE dbo.UserMessages
	(Id int IDENTITY(1,1) PRIMARY KEY,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserMessages_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	RecipientId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserMessages_RecipientId FOREIGN KEY (RecipientId)
		REFERENCES Users (Id),
	MessageId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_UserMessages_MessageId FOREIGN KEY (MessageId)
		REFERENCES Messages (Id),
	DateCreated datetime NOT NULL,
	IsDeletedBySender bit NOT NULL DEFAULT 0,
	IsDeletedByRecipient bit NOT NULL DEFAULT 0,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

-- CREATE TABLE dbo.StationMessageUserLikes
-- 	(Id int IDENTITY(1,1) PRIMARY KEY,
-- 	StationMessageId int NOT NULL,
-- 	CONSTRAINT FK_StationMessageUserLikes_StationMessageId FOREIGN KEY (StationMessageId)
-- 		REFERENCES StationMessages (Id),
-- 	UserId UNIQUEIDENTIFIER NOT NULL,
-- 	CONSTRAINT FK_StationMessageUserLikes_UserId FOREIGN KEY (UserId)
-- 		REFERENCES Users (Id),
-- 	DateCreated datetime NOT NULL,
-- 	DateModified datetime NOT NULL,
--     IsDeleted bit NOT NULL DEFAULT 0)
-- GO

-- CREATE TABLE dbo.StationFollowers
-- 	(Id int IDENTITY(1,1) PRIMARY KEY,
-- 	StationId int NOT NULL,
-- 	CONSTRAINT FK_StationFollowers_StationId FOREIGN KEY (StationId)
-- 		REFERENCES Stations (Id),
-- 	UserId UNIQUEIDENTIFIER NOT NULL,
-- 	CONSTRAINT FK_StationFollowers_UserId FOREIGN KEY (UserId)
-- 		REFERENCES Users (Id),
-- 	DateCreated datetime NOT NULL,
--     IsDeleted bit NOT NULL DEFAULT 0)
-- GO

-- CREATE TABLE dbo.StationCategories
-- 	(Id int IDENTITY(1,1) PRIMARY KEY,
-- 	StationId int NOT NULL,
-- 	CONSTRAINT FK_StationCategories_StationId FOREIGN KEY (StationId)
-- 		REFERENCES Stations (Id),
-- 	CategoryId int NOT NULL,
-- 	CONSTRAINT FK_StationCategories_CategoryId FOREIGN KEY (CategoryId)
-- 		REFERENCES Categories (Id),
-- 	DateCreated datetime NOT NULL,
--     IsDeleted bit NOT NULL DEFAULT 0)
-- GO

CREATE TABLE dbo.Melodies
	(Id int IDENTITY(1,1) PRIMARY KEY,
	BaseMelodyId int NULL,
	UserId UNIQUEIDENTIFIER NULL,
	CONSTRAINT FK_Melodies_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	IsUserMelody bit NOT NULL DEFAULT 0,
	Name varchar(128) NOT NULL,
	Description text NULL,
	FileName varchar(255) NOT NULL,
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
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

CREATE TABLE dbo.MelodyLoops
	(Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_MelodyLoops_UserId FOREIGN KEY (UserId)
		REFERENCES Users (Id),
	DateCreated datetime NOT NULL,
	DateModified datetime NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.MelodyLoopParts
	(Id int IDENTITY(1,1) PRIMARY KEY,
	MelodyLoopId UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_MelodyLoopParts_MelodyLoopId FOREIGN KEY (MelodyLoopId)
		REFERENCES MelodyLoops (Id),
	MelodyId int NOT NULL,
	CONSTRAINT FK_MelodyLoopParts_MelodyId FOREIGN KEY (MelodyId)
		REFERENCES Melodies (Id),
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

-- END Tables

USE [InstaMelody]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[Name] [varchar](128) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ChatMessages]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatMessages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ChatId] [uniqueidentifier] NOT NULL,
	[MessageId] [uniqueidentifier] NOT NULL,
	[SenderId] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Chats]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Chats](
	[Id] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChatUsers]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ChatId] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FileGroups]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FileGroups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](128) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FileUploadTokens]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FileUploadTokens](
	[Token] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[FileName] [varchar](255) NOT NULL,
	[MediaType] [varchar](32) NOT NULL,
	[DateExpires] [datetime] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Images]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Images](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [varchar](255) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Melodies]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Melodies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IsUserCreated] [bit] NOT NULL,
	[Name] [varchar](128) NOT NULL,
	[Description] [text] NULL,
	[FileName] [varchar](255) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MelodyCategories]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MelodyCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MelodyId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MelodyFileGroups]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MelodyFileGroups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MelodyId] [int] NOT NULL,
	[FileGroupId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MessageImages]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageImages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [uniqueidentifier] NOT NULL,
	[ImageId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MessageMelodies]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageMelodies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [uniqueidentifier] NOT NULL,
	[UserMelodyId] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Messages]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Messages](
	[Id] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NULL,
	[Description] [text] NULL,
	[MediaType] [varchar](32) NOT NULL,
	[IsRead] [bit] NOT NULL,
	[DateRead] [datetime] NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MessageVideos]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageVideos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [uniqueidentifier] NOT NULL,
	[VideoId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StationCategories]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StationCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StationId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StationFollowers]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StationFollowers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StationId] [int] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Stations]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Stations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[StationImageId] [int] NULL,
	[Name] [varchar](128) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserFriends]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserFriends](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[RequestorId] [uniqueidentifier] NOT NULL,
	[IsPending] [bit] NOT NULL,
	[IsDenied] [bit] NOT NULL,
	[DateApproved] [datetime] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserLoopParts]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserLoopParts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserLoopId] [uniqueidentifier] NOT NULL,
	[UserMelodyId] [uniqueidentifier] NOT NULL,
	[OrderIndex] [int] NOT NULL,
	[StartTime] [bigint] NULL,
	[StartEffect] [varchar](56) NULL,
	[StartEffectDuration] [bigint] NULL,
	[EndTime] [bigint] NULL,
	[EndEffect] [varchar](56) NULL,
	[EndEffectDuration] [bigint] NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserLoops]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserLoops](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](128) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserMelodies]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserMelodies](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](128) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserMelodyParts]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMelodyParts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserMelodyId] [uniqueidentifier] NOT NULL,
	[MelodyId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[UserImageId] [int] NULL,
	[EmailAddress] [varchar](320) NULL,
	[DisplayName] [varchar](32) NOT NULL,
	[FirstName] [varchar](64) NULL,
	[LastName] [varchar](64) NULL,
	[PhoneNumber] [varchar](28) NULL,
	[HashSalt] [varchar](64) NULL,
	[Password] [varchar](255) NULL,
	[IsSubscribed] [bit] NOT NULL,
	[TwitterUsername] [varchar](128) NULL,
	[TwitterUserId] [varchar](128) NULL,
	[TwitterToken] [varchar](255) NULL,
	[TwitterSecret] [varchar](255) NULL,
	[FacebookUserId] [varchar](128) NULL,
	[FacebookToken] [varchar](255) NULL,
	[LastLoginSuccess] [datetime] NULL,
	[LastLoginFailure] [datetime] NULL,
	[NumberLoginFailures] [int] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserSessions]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSessions](
	[Token] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[LastActivity] [datetime] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Videos]    Script Date: 7/20/2015 9:16:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Videos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [varchar](255) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([Id], [ParentId], [Name], [DateCreated], [DateModified], [IsDeleted]) VALUES (1, NULL, N'Test Category', CAST(0x0000A4CD00000000 AS DateTime), CAST(0x0000A4CD00000000 AS DateTime), 0)
INSERT [dbo].[Categories] ([Id], [ParentId], [Name], [DateCreated], [DateModified], [IsDeleted]) VALUES (2, 1, N'test 2', CAST(0x0000A41300000000 AS DateTime), CAST(0x0000A41300000000 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Categories] OFF
SET IDENTITY_INSERT [dbo].[ChatMessages] ON 

INSERT [dbo].[ChatMessages] ([Id], [ChatId], [MessageId], [SenderId], [DateCreated], [IsDeleted]) VALUES (5, N'ebcb1743-7378-4d7d-96c5-5403cc51a07e', N'713d9487-910b-4026-9252-8eb5cca1e50f', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4CE011EA175 AS DateTime), 0)
INSERT [dbo].[ChatMessages] ([Id], [ChatId], [MessageId], [SenderId], [DateCreated], [IsDeleted]) VALUES (6, N'ebcb1743-7378-4d7d-96c5-5403cc51a07e', N'a4a35715-a9e4-488c-ab70-10dda0f57c94', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4CE0122D9D9 AS DateTime), 0)
INSERT [dbo].[ChatMessages] ([Id], [ChatId], [MessageId], [SenderId], [DateCreated], [IsDeleted]) VALUES (7, N'ebcb1743-7378-4d7d-96c5-5403cc51a07e', N'6bc30a9c-ea0a-4e75-ac25-c120f920f63a', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4CE01230FF7 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[ChatMessages] OFF
INSERT [dbo].[Chats] ([Id], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'ebcb1743-7378-4d7d-96c5-5403cc51a07e', CAST(0x0000A4CE011EA14E AS DateTime), CAST(0x0000A4CE01230FFE AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[ChatUsers] ON 

INSERT [dbo].[ChatUsers] ([Id], [UserId], [ChatId], [DateCreated], [IsDeleted]) VALUES (14, N'cbca4d41-42ca-45ab-8aaf-262a0a59d677', N'ebcb1743-7378-4d7d-96c5-5403cc51a07e', CAST(0x0000A4CE011EA15A AS DateTime), 0)
INSERT [dbo].[ChatUsers] ([Id], [UserId], [ChatId], [DateCreated], [IsDeleted]) VALUES (15, N'5651a0ad-b934-4be8-88f2-c380f884adf5', N'ebcb1743-7378-4d7d-96c5-5403cc51a07e', CAST(0x0000A4CE011EA164 AS DateTime), 0)
INSERT [dbo].[ChatUsers] ([Id], [UserId], [ChatId], [DateCreated], [IsDeleted]) VALUES (18, N'cbca4d41-42ca-45ab-8aaf-262a0a59d677', N'ebcb1743-7378-4d7d-96c5-5403cc51a07e', CAST(0x0000A4CE011FA01A AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[ChatUsers] OFF
SET IDENTITY_INSERT [dbo].[FileGroups] ON 

INSERT [dbo].[FileGroups] ([Id], [Name], [DateCreated], [DateModified], [IsDeleted]) VALUES (1, N'Test Group', CAST(0x0000A4CD00000000 AS DateTime), CAST(0x0000A4CD00000000 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[FileGroups] OFF
INSERT [dbo].[FileUploadTokens] ([Token], [UserId], [FileName], [MediaType], [DateExpires], [DateCreated], [IsDeleted]) VALUES (N'd3e0f5ab-9d78-42bd-bc85-1c0ed823ad28', N'5651a0ad-b934-4be8-88f2-c380f884adf5', N'sample.m4a', N'UserMelody', CAST(0x0000A4CF0127F443 AS DateTime), CAST(0x0000A4CF01253523 AS DateTime), 1)
INSERT [dbo].[FileUploadTokens] ([Token], [UserId], [FileName], [MediaType], [DateExpires], [DateCreated], [IsDeleted]) VALUES (N'c23e93ab-2f22-4b31-981d-8f13b5b6131d', N'5651a0ad-b934-4be8-88f2-c380f884adf5', N'testuserapi3.m4a', N'UserMelody', CAST(0x0000A4D8012047BD AS DateTime), CAST(0x0000A4D8011D889D AS DateTime), 0)
INSERT [dbo].[FileUploadTokens] ([Token], [UserId], [FileName], [MediaType], [DateExpires], [DateCreated], [IsDeleted]) VALUES (N'c2eb8573-2d04-43af-8c2e-9eb121f9cb2c', N'5651a0ad-b934-4be8-88f2-c380f884adf5', N'testuserapi.m4a', N'UserMelody', CAST(0x0000A4D80119AFC1 AS DateTime), CAST(0x0000A4D80116F0A1 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Images] ON 

INSERT [dbo].[Images] ([Id], [FileName], [DateCreated], [IsDeleted]) VALUES (1, N'thisismyimagename.jpg', CAST(0x0000A4CE00FFF8AE AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Images] OFF
SET IDENTITY_INSERT [dbo].[Melodies] ON 

INSERT [dbo].[Melodies] ([Id], [IsUserCreated], [Name], [Description], [FileName], [DateCreated], [DateModified], [IsDeleted]) VALUES (1, 0, N'01 Bass', NULL, N'01 Bass.wav', CAST(0x0000A4CD00000000 AS DateTime), CAST(0x0000A4CD00000000 AS DateTime), 0)
INSERT [dbo].[Melodies] ([Id], [IsUserCreated], [Name], [Description], [FileName], [DateCreated], [DateModified], [IsDeleted]) VALUES (2, 0, N'01 Drums', NULL, N'01 Drums.wav', CAST(0x0000A4CD00000000 AS DateTime), CAST(0x0000A4CD00000000 AS DateTime), 0)
INSERT [dbo].[Melodies] ([Id], [IsUserCreated], [Name], [Description], [FileName], [DateCreated], [DateModified], [IsDeleted]) VALUES (3, 0, N'01 Melody', NULL, N'01 Melody.wav', CAST(0x0000A4CD00000000 AS DateTime), CAST(0x0000A4CD00000000 AS DateTime), 0)
INSERT [dbo].[Melodies] ([Id], [IsUserCreated], [Name], [Description], [FileName], [DateCreated], [DateModified], [IsDeleted]) VALUES (16, 1, N'Test API Melody', N'This is a non-required field', N'sample.m4a', CAST(0x0000A4CF012534FD AS DateTime), CAST(0x0000A4CF012534FD AS DateTime), 0)
INSERT [dbo].[Melodies] ([Id], [IsUserCreated], [Name], [Description], [FileName], [DateCreated], [DateModified], [IsDeleted]) VALUES (17, 1, N'Test API Melody 2', N'This is a non-required field', N'testuserapi.m4a', CAST(0x0000A4D80116F06F AS DateTime), CAST(0x0000A4D80116F06F AS DateTime), 0)
INSERT [dbo].[Melodies] ([Id], [IsUserCreated], [Name], [Description], [FileName], [DateCreated], [DateModified], [IsDeleted]) VALUES (18, 1, N'Test API Melody 3', N'This is a non-required field', N'testuserapi3.m4a', CAST(0x0000A4D8011D886B AS DateTime), CAST(0x0000A4D8011D886B AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Melodies] OFF
SET IDENTITY_INSERT [dbo].[MelodyCategories] ON 

INSERT [dbo].[MelodyCategories] ([Id], [MelodyId], [CategoryId], [DateCreated], [IsDeleted]) VALUES (1, 1, 1, CAST(0x0000A4CD00000000 AS DateTime), 0)
INSERT [dbo].[MelodyCategories] ([Id], [MelodyId], [CategoryId], [DateCreated], [IsDeleted]) VALUES (2, 2, 1, CAST(0x0000A4CD00000000 AS DateTime), 0)
INSERT [dbo].[MelodyCategories] ([Id], [MelodyId], [CategoryId], [DateCreated], [IsDeleted]) VALUES (3, 3, 1, CAST(0x0000A4CD00000000 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[MelodyCategories] OFF
SET IDENTITY_INSERT [dbo].[MelodyFileGroups] ON 

INSERT [dbo].[MelodyFileGroups] ([Id], [MelodyId], [FileGroupId], [DateCreated], [IsDeleted]) VALUES (1, 1, 1, CAST(0x0000A4CD00000000 AS DateTime), 0)
INSERT [dbo].[MelodyFileGroups] ([Id], [MelodyId], [FileGroupId], [DateCreated], [IsDeleted]) VALUES (2, 2, 1, CAST(0x0000A4CD00000000 AS DateTime), 0)
INSERT [dbo].[MelodyFileGroups] ([Id], [MelodyId], [FileGroupId], [DateCreated], [IsDeleted]) VALUES (3, 3, 1, CAST(0x0000A4CD00000000 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[MelodyFileGroups] OFF
INSERT [dbo].[Messages] ([Id], [ParentId], [Description], [MediaType], [IsRead], [DateRead], [DateCreated], [IsDeleted]) VALUES (N'b5dec62c-b43c-4c9e-974c-0f7327119732', NULL, N'Hello', N'Unknown', 0, NULL, CAST(0x0000A4CE01150085 AS DateTime), 0)
INSERT [dbo].[Messages] ([Id], [ParentId], [Description], [MediaType], [IsRead], [DateRead], [DateCreated], [IsDeleted]) VALUES (N'a4a35715-a9e4-488c-ab70-10dda0f57c94', NULL, N'Test Message 2', N'Unknown', 0, NULL, CAST(0x0000A4CE0122D9D0 AS DateTime), 0)
INSERT [dbo].[Messages] ([Id], [ParentId], [Description], [MediaType], [IsRead], [DateRead], [DateCreated], [IsDeleted]) VALUES (N'8666267e-8e74-4b7a-b599-2b1230910a31', NULL, N'Hello', N'Unknown', 0, NULL, CAST(0x0000A4CE0114C81C AS DateTime), 0)
INSERT [dbo].[Messages] ([Id], [ParentId], [Description], [MediaType], [IsRead], [DateRead], [DateCreated], [IsDeleted]) VALUES (N'407c524c-8a43-42a5-be13-4887060126e9', NULL, N'Hello', N'Unknown', 0, NULL, CAST(0x0000A4CE0115A66F AS DateTime), 0)
INSERT [dbo].[Messages] ([Id], [ParentId], [Description], [MediaType], [IsRead], [DateRead], [DateCreated], [IsDeleted]) VALUES (N'47da6a72-5265-405f-b9c5-72fa044c6ea4', NULL, N'Hello', N'Unknown', 0, NULL, CAST(0x0000A4CE011C32B9 AS DateTime), 0)
INSERT [dbo].[Messages] ([Id], [ParentId], [Description], [MediaType], [IsRead], [DateRead], [DateCreated], [IsDeleted]) VALUES (N'713d9487-910b-4026-9252-8eb5cca1e50f', NULL, N'Hello', N'Unknown', 0, NULL, CAST(0x0000A4CE011EA16F AS DateTime), 0)
INSERT [dbo].[Messages] ([Id], [ParentId], [Description], [MediaType], [IsRead], [DateRead], [DateCreated], [IsDeleted]) VALUES (N'81a2abce-2396-4ff6-a555-a71ebe73f65d', NULL, N'Hello', N'Unknown', 0, NULL, CAST(0x0000A4CE0115F6F4 AS DateTime), 0)
INSERT [dbo].[Messages] ([Id], [ParentId], [Description], [MediaType], [IsRead], [DateRead], [DateCreated], [IsDeleted]) VALUES (N'6bc30a9c-ea0a-4e75-ac25-c120f920f63a', NULL, N'Test Message 2', N'Unknown', 0, NULL, CAST(0x0000A4CE01230FEE AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Stations] ON 

INSERT [dbo].[Stations] ([Id], [UserId], [StationImageId], [Name], [DateCreated], [DateModified], [IsDeleted]) VALUES (1, N'5651a0ad-b934-4be8-88f2-c380f884adf5', NULL, N'Test Station', CAST(0x0000A4DB00000000 AS DateTime), CAST(0x0000A4DB00000000 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Stations] OFF
SET IDENTITY_INSERT [dbo].[UserFriends] ON 

INSERT [dbo].[UserFriends] ([Id], [UserId], [RequestorId], [IsPending], [IsDenied], [DateApproved], [DateCreated], [DateModified], [IsDeleted]) VALUES (1, N'cbca4d41-42ca-45ab-8aaf-262a0a59d677', N'5651a0ad-b934-4be8-88f2-c380f884adf5', 0, 0, NULL, CAST(0x0000A4CE0106FBA9 AS DateTime), CAST(0x0000A4CE010BCA97 AS DateTime), 0)
INSERT [dbo].[UserFriends] ([Id], [UserId], [RequestorId], [IsPending], [IsDenied], [DateApproved], [DateCreated], [DateModified], [IsDeleted]) VALUES (2, N'cc2042aa-238e-44c1-b0be-dede01476d9d', N'5651a0ad-b934-4be8-88f2-c380f884adf5', 0, 0, NULL, CAST(0x0000A4CE0106FBA9 AS DateTime), CAST(0x0000A4CE0106FBA9 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[UserFriends] OFF
SET IDENTITY_INSERT [dbo].[UserLoopParts] ON 

INSERT [dbo].[UserLoopParts] ([Id], [UserLoopId], [UserMelodyId], [OrderIndex], [StartTime], [StartEffect], [StartEffectDuration], [EndTime], [EndEffect], [EndEffectDuration], [DateCreated], [IsDeleted]) VALUES (4, N'45731948-041c-4417-b96b-a54c0f332cf6', N'79e0b7af-6e39-4c93-b260-666f5de2c3c7', 1, 0, N'Unknown', 0, NULL, N'Unknown', 0, CAST(0x0000A4D80112CB5D AS DateTime), 0)
INSERT [dbo].[UserLoopParts] ([Id], [UserLoopId], [UserMelodyId], [OrderIndex], [StartTime], [StartEffect], [StartEffectDuration], [EndTime], [EndEffect], [EndEffectDuration], [DateCreated], [IsDeleted]) VALUES (5, N'35c4da6e-fd05-40b0-8394-68d7abb6f5fb', N'c7ff0501-eb70-45f3-a8a5-8233f0f60ed7', 1, 0, N'Unknown', 0, NULL, N'Unknown', 0, CAST(0x0000A4D80116F0B4 AS DateTime), 1)
INSERT [dbo].[UserLoopParts] ([Id], [UserLoopId], [UserMelodyId], [OrderIndex], [StartTime], [StartEffect], [StartEffectDuration], [EndTime], [EndEffect], [EndEffectDuration], [DateCreated], [IsDeleted]) VALUES (6, N'35c4da6e-fd05-40b0-8394-68d7abb6f5fb', N'8ce82975-cbb8-4737-9826-33f4409a10f2', 2, 0, N'Unknown', 0, NULL, N'Unknown', 0, CAST(0x0000A4D8011D8F2F AS DateTime), 1)
INSERT [dbo].[UserLoopParts] ([Id], [UserLoopId], [UserMelodyId], [OrderIndex], [StartTime], [StartEffect], [StartEffectDuration], [EndTime], [EndEffect], [EndEffectDuration], [DateCreated], [IsDeleted]) VALUES (7, N'35c4da6e-fd05-40b0-8394-68d7abb6f5fb', N'79e0b7af-6e39-4c93-b260-666f5de2c3c7', 3, 0, N'Unknown', 0, NULL, N'Unknown', 0, CAST(0x0000A4D8011E36D6 AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[UserLoopParts] OFF
INSERT [dbo].[UserLoops] ([Id], [Name], [UserId], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'35c4da6e-fd05-40b0-8394-68d7abb6f5fb', N'Test Loop 2', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4D80116AD41 AS DateTime), CAST(0x0000A4D8011E36D6 AS DateTime), 1)
INSERT [dbo].[UserLoops] ([Id], [Name], [UserId], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'50480a68-7b1b-4752-b362-8e85774190e3', N'Test Loop 2', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4D8011632D1 AS DateTime), CAST(0x0000A4D8011632D1 AS DateTime), 1)
INSERT [dbo].[UserLoops] ([Id], [Name], [UserId], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'45731948-041c-4417-b96b-a54c0f332cf6', N'Test Loop', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4D80112CB42 AS DateTime), CAST(0x0000A4D80112CB42 AS DateTime), 0)
INSERT [dbo].[UserMelodies] ([Id], [Name], [UserId], [DateCreated], [IsDeleted]) VALUES (N'8ce82975-cbb8-4737-9826-33f4409a10f2', N'Test API Melody 3', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4D8011D8874 AS DateTime), 0)
INSERT [dbo].[UserMelodies] ([Id], [Name], [UserId], [DateCreated], [IsDeleted]) VALUES (N'79e0b7af-6e39-4c93-b260-666f5de2c3c7', N'Test API Melody', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4CF01253503 AS DateTime), 0)
INSERT [dbo].[UserMelodies] ([Id], [Name], [UserId], [DateCreated], [IsDeleted]) VALUES (N'c7ff0501-eb70-45f3-a8a5-8233f0f60ed7', N'Test API Melody 2', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4D80116F079 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[UserMelodyParts] ON 

INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (49, N'79e0b7af-6e39-4c93-b260-666f5de2c3c7', 1, CAST(0x0000A4CF01253509 AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (50, N'79e0b7af-6e39-4c93-b260-666f5de2c3c7', 2, CAST(0x0000A4CF0125350F AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (51, N'79e0b7af-6e39-4c93-b260-666f5de2c3c7', 3, CAST(0x0000A4CF01253515 AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (52, N'79e0b7af-6e39-4c93-b260-666f5de2c3c7', 16, CAST(0x0000A4CF0125351A AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (53, N'c7ff0501-eb70-45f3-a8a5-8233f0f60ed7', 1, CAST(0x0000A4D80116F081 AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (54, N'c7ff0501-eb70-45f3-a8a5-8233f0f60ed7', 2, CAST(0x0000A4D80116F089 AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (55, N'c7ff0501-eb70-45f3-a8a5-8233f0f60ed7', 3, CAST(0x0000A4D80116F08F AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (56, N'c7ff0501-eb70-45f3-a8a5-8233f0f60ed7', 17, CAST(0x0000A4D80116F095 AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (57, N'8ce82975-cbb8-4737-9826-33f4409a10f2', 1, CAST(0x0000A4D8011D887E AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (58, N'8ce82975-cbb8-4737-9826-33f4409a10f2', 2, CAST(0x0000A4D8011D8886 AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (59, N'8ce82975-cbb8-4737-9826-33f4409a10f2', 3, CAST(0x0000A4D8011D888C AS DateTime), 0)
INSERT [dbo].[UserMelodyParts] ([Id], [UserMelodyId], [MelodyId], [DateCreated], [IsDeleted]) VALUES (60, N'8ce82975-cbb8-4737-9826-33f4409a10f2', 18, CAST(0x0000A4D8011D8893 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[UserMelodyParts] OFF
INSERT [dbo].[Users] ([Id], [UserImageId], [EmailAddress], [DisplayName], [FirstName], [LastName], [PhoneNumber], [HashSalt], [Password], [IsSubscribed], [TwitterUsername], [TwitterUserId], [TwitterToken], [TwitterSecret], [FacebookUserId], [FacebookToken], [LastLoginSuccess], [LastLoginFailure], [NumberLoginFailures], [IsLocked], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'cbca4d41-42ca-45ab-8aaf-262a0a59d677', NULL, N'test2@test2.com', N'testaccount2', N'test2', N'est2', N'123-555-1234', N'7GOK84DX2I3QSK82Q491J52', N'zSU5vte+WYcg82TgQfv6hLSoqUfZjD13Ulj+F0uAH/4=', 0, NULL, NULL, NULL, NULL, NULL, NULL, CAST(0x0000A4CE0110E059 AS DateTime), CAST(0x0000A4CE01109123 AS DateTime), 0, 0, CAST(0x0000A4CE01054A65 AS DateTime), CAST(0x0000A4CE0110E00D AS DateTime), 0)
INSERT [dbo].[Users] ([Id], [UserImageId], [EmailAddress], [DisplayName], [FirstName], [LastName], [PhoneNumber], [HashSalt], [Password], [IsSubscribed], [TwitterUsername], [TwitterUserId], [TwitterToken], [TwitterSecret], [FacebookUserId], [FacebookToken], [LastLoginSuccess], [LastLoginFailure], [NumberLoginFailures], [IsLocked], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'9697ee03-4179-4178-8118-b22cbfce15ad', NULL, N'dwight@schrutefarms.com', N'AgentSchrute', N'Dwight', N'Schrute', NULL, N'I1RVLLB1FV3X32KM5V03UR1', N'pkfWMl6INYYZ5sdTei0gAxyn9VmumrtH7KQHIlZI80o=', 0, NULL, NULL, NULL, NULL, NULL, NULL, CAST(0x0000A4CD01498D05 AS DateTime), NULL, 0, 0, CAST(0x0000A4CD01498CF2 AS DateTime), CAST(0x0000A4CD01498CF2 AS DateTime), 0)
INSERT [dbo].[Users] ([Id], [UserImageId], [EmailAddress], [DisplayName], [FirstName], [LastName], [PhoneNumber], [HashSalt], [Password], [IsSubscribed], [TwitterUsername], [TwitterUserId], [TwitterToken], [TwitterSecret], [FacebookUserId], [FacebookToken], [LastLoginSuccess], [LastLoginFailure], [NumberLoginFailures], [IsLocked], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'5651a0ad-b934-4be8-88f2-c380f884adf5', 1, N'test@testy.com', N'testaccount123', N'test', N'account', N'123-555-1234', N'JXXOG419G0E5M30G165978U', N'/lGj9XUBeMKzwmCSW4akDsoYXK5RURIyNMyJ5/hr0nI=', 0, NULL, NULL, NULL, NULL, NULL, NULL, CAST(0x0000A4CE00F8C28E AS DateTime), NULL, 0, 0, CAST(0x0000A4CE00F85445 AS DateTime), CAST(0x0000A4CE00FFFC23 AS DateTime), 0)
INSERT [dbo].[Users] ([Id], [UserImageId], [EmailAddress], [DisplayName], [FirstName], [LastName], [PhoneNumber], [HashSalt], [Password], [IsSubscribed], [TwitterUsername], [TwitterUserId], [TwitterToken], [TwitterSecret], [FacebookUserId], [FacebookToken], [LastLoginSuccess], [LastLoginFailure], [NumberLoginFailures], [IsLocked], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'017e9ab4-af68-4a01-9a70-d26e50dd1cc0', NULL, N'jeff@test.com', N'jeffdennis', N'jeff', N'dennnis', NULL, N'5IU61Q73HOD6XM334OE7KU5', N'VNWBn4wP9s1nEYgseZZ7oqLlxpMsAG1dGKif9rSzPz4=', 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, CAST(0x0000A4CF012A8BB0 AS DateTime), CAST(0x0000A4CF012A8BB0 AS DateTime), 0)
INSERT [dbo].[Users] ([Id], [UserImageId], [EmailAddress], [DisplayName], [FirstName], [LastName], [PhoneNumber], [HashSalt], [Password], [IsSubscribed], [TwitterUsername], [TwitterUserId], [TwitterToken], [TwitterSecret], [FacebookUserId], [FacebookToken], [LastLoginSuccess], [LastLoginFailure], [NumberLoginFailures], [IsLocked], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'cc2042aa-238e-44c1-b0be-dede01476d9d', NULL, N'test3@test3.com', N'testaccount3', N'test3', N'test3', N'123-555-1234', N'3NTRP3XW4J1S9XQ4O3EJ290', N'zXUSlFHa7aC0NP8LPP7S+BsYDV3G/Vyisi7v0KosZ78=', 0, NULL, NULL, NULL, NULL, NULL, NULL, CAST(0x0000A4CE010E758E AS DateTime), CAST(0x0000A4CE010FFF55 AS DateTime), 1, 0, CAST(0x0000A4CE010E64EC AS DateTime), CAST(0x0000A4CE010FFF45 AS DateTime), 0)
INSERT [dbo].[UserSessions] ([Token], [UserId], [LastActivity], [DateCreated], [IsDeleted]) VALUES (N'd18ac064-f500-483f-992d-76c137730dea', N'9697ee03-4179-4178-8118-b22cbfce15ad', CAST(0x0000A4CD01498D09 AS DateTime), CAST(0x0000A4CD01498D09 AS DateTime), 0)
INSERT [dbo].[UserSessions] ([Token], [UserId], [LastActivity], [DateCreated], [IsDeleted]) VALUES (N'bf310686-2dc9-4333-bc21-ab38812fc11a', N'cc2042aa-238e-44c1-b0be-dede01476d9d', CAST(0x0000A4CE010FFF4E AS DateTime), CAST(0x0000A4CE010E75B3 AS DateTime), 0)
INSERT [dbo].[UserSessions] ([Token], [UserId], [LastActivity], [DateCreated], [IsDeleted]) VALUES (N'9d0ab021-fcf8-4ec3-b6e3-bb1d0d03b12e', N'5651a0ad-b934-4be8-88f2-c380f884adf5', CAST(0x0000A4CE010351B9 AS DateTime), CAST(0x0000A4CE00F8C392 AS DateTime), 0)
INSERT [dbo].[UserSessions] ([Token], [UserId], [LastActivity], [DateCreated], [IsDeleted]) VALUES (N'd89d3d12-e141-4b7c-8769-cf860538f203', N'cbca4d41-42ca-45ab-8aaf-262a0a59d677', CAST(0x0000A4CE0110E062 AS DateTime), CAST(0x0000A4CE0110E062 AS DateTime), 0)
INSERT [dbo].[UserSessions] ([Token], [UserId], [LastActivity], [DateCreated], [IsDeleted]) VALUES (N'4a51960e-6e46-49ae-921b-ea0d5e247bfb', N'cbca4d41-42ca-45ab-8aaf-262a0a59d677', CAST(0x0000A4CE0110E053 AS DateTime), CAST(0x0000A4CE01055CDB AS DateTime), 1)
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ChatMessages] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Chats] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ChatUsers] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[FileGroups] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[FileUploadTokens] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Images] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Melodies] ADD  DEFAULT ((0)) FOR [IsUserCreated]
GO
ALTER TABLE [dbo].[Melodies] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[MelodyCategories] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[MelodyFileGroups] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[MessageImages] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[MessageMelodies] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Messages] ADD  DEFAULT ((0)) FOR [IsRead]
GO
ALTER TABLE [dbo].[Messages] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[MessageVideos] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[StationCategories] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[StationFollowers] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Stations] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserFriends] ADD  DEFAULT ((1)) FOR [IsPending]
GO
ALTER TABLE [dbo].[UserFriends] ADD  DEFAULT ((0)) FOR [IsDenied]
GO
ALTER TABLE [dbo].[UserFriends] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserLoopParts] ADD  DEFAULT ((1)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[UserLoopParts] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserLoops] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserMelodies] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserMelodyParts] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [IsSubscribed]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [NumberLoginFailures]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserSessions] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Videos] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ChatMessages]  WITH CHECK ADD  CONSTRAINT [FK_ChatMessages_ChatId] FOREIGN KEY([ChatId])
REFERENCES [dbo].[Chats] ([Id])
GO
ALTER TABLE [dbo].[ChatMessages] CHECK CONSTRAINT [FK_ChatMessages_ChatId]
GO
ALTER TABLE [dbo].[ChatMessages]  WITH CHECK ADD  CONSTRAINT [FK_ChatMessages_MessageId] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Messages] ([Id])
GO
ALTER TABLE [dbo].[ChatMessages] CHECK CONSTRAINT [FK_ChatMessages_MessageId]
GO
ALTER TABLE [dbo].[ChatMessages]  WITH CHECK ADD  CONSTRAINT [FK_ChatMessages_SenderId] FOREIGN KEY([SenderId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[ChatMessages] CHECK CONSTRAINT [FK_ChatMessages_SenderId]
GO
ALTER TABLE [dbo].[ChatUsers]  WITH CHECK ADD  CONSTRAINT [FK_ChatMessages_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[ChatUsers] CHECK CONSTRAINT [FK_ChatMessages_UserId]
GO
ALTER TABLE [dbo].[ChatUsers]  WITH CHECK ADD  CONSTRAINT [FK_ChatUsers_ChatId] FOREIGN KEY([ChatId])
REFERENCES [dbo].[Chats] ([Id])
GO
ALTER TABLE [dbo].[ChatUsers] CHECK CONSTRAINT [FK_ChatUsers_ChatId]
GO
ALTER TABLE [dbo].[FileUploadTokens]  WITH CHECK ADD  CONSTRAINT [FK_FileUploadTokens_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[FileUploadTokens] CHECK CONSTRAINT [FK_FileUploadTokens_UserId]
GO
ALTER TABLE [dbo].[MelodyCategories]  WITH CHECK ADD  CONSTRAINT [FK_MelodyCategories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[MelodyCategories] CHECK CONSTRAINT [FK_MelodyCategories_CategoryId]
GO
ALTER TABLE [dbo].[MelodyCategories]  WITH CHECK ADD  CONSTRAINT [FK_MelodyCategories_MelodyId] FOREIGN KEY([MelodyId])
REFERENCES [dbo].[Melodies] ([Id])
GO
ALTER TABLE [dbo].[MelodyCategories] CHECK CONSTRAINT [FK_MelodyCategories_MelodyId]
GO
ALTER TABLE [dbo].[MelodyFileGroups]  WITH CHECK ADD  CONSTRAINT [FK_MelodyFileGroups_FileGroupId] FOREIGN KEY([FileGroupId])
REFERENCES [dbo].[FileGroups] ([Id])
GO
ALTER TABLE [dbo].[MelodyFileGroups] CHECK CONSTRAINT [FK_MelodyFileGroups_FileGroupId]
GO
ALTER TABLE [dbo].[MelodyFileGroups]  WITH CHECK ADD  CONSTRAINT [FK_MelodyFileGroups_MelodyId] FOREIGN KEY([MelodyId])
REFERENCES [dbo].[Melodies] ([Id])
GO
ALTER TABLE [dbo].[MelodyFileGroups] CHECK CONSTRAINT [FK_MelodyFileGroups_MelodyId]
GO
ALTER TABLE [dbo].[MessageImages]  WITH CHECK ADD  CONSTRAINT [FK_MessageImages_ImageId] FOREIGN KEY([ImageId])
REFERENCES [dbo].[Images] ([Id])
GO
ALTER TABLE [dbo].[MessageImages] CHECK CONSTRAINT [FK_MessageImages_ImageId]
GO
ALTER TABLE [dbo].[MessageImages]  WITH CHECK ADD  CONSTRAINT [FK_MessageImages_MessageId] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Messages] ([Id])
GO
ALTER TABLE [dbo].[MessageImages] CHECK CONSTRAINT [FK_MessageImages_MessageId]
GO
ALTER TABLE [dbo].[MessageMelodies]  WITH CHECK ADD  CONSTRAINT [FK_MessageMelodies_MessageId] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Messages] ([Id])
GO
ALTER TABLE [dbo].[MessageMelodies] CHECK CONSTRAINT [FK_MessageMelodies_MessageId]
GO
ALTER TABLE [dbo].[MessageMelodies]  WITH CHECK ADD  CONSTRAINT [FK_MessageMelodies_UserMelodyId] FOREIGN KEY([UserMelodyId])
REFERENCES [dbo].[UserMelodies] ([Id])
GO
ALTER TABLE [dbo].[MessageMelodies] CHECK CONSTRAINT [FK_MessageMelodies_UserMelodyId]
GO
ALTER TABLE [dbo].[MessageVideos]  WITH CHECK ADD  CONSTRAINT [FK_MessageVideos_MessageId] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Messages] ([Id])
GO
ALTER TABLE [dbo].[MessageVideos] CHECK CONSTRAINT [FK_MessageVideos_MessageId]
GO
ALTER TABLE [dbo].[MessageVideos]  WITH CHECK ADD  CONSTRAINT [FK_MessageVideos_VideoId] FOREIGN KEY([VideoId])
REFERENCES [dbo].[Videos] ([Id])
GO
ALTER TABLE [dbo].[MessageVideos] CHECK CONSTRAINT [FK_MessageVideos_VideoId]
GO
ALTER TABLE [dbo].[StationCategories]  WITH CHECK ADD  CONSTRAINT [FK_StationCategories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[StationCategories] CHECK CONSTRAINT [FK_StationCategories_CategoryId]
GO
ALTER TABLE [dbo].[StationCategories]  WITH CHECK ADD  CONSTRAINT [FK_StationCategories_StationId] FOREIGN KEY([StationId])
REFERENCES [dbo].[Stations] ([Id])
GO
ALTER TABLE [dbo].[StationCategories] CHECK CONSTRAINT [FK_StationCategories_StationId]
GO
ALTER TABLE [dbo].[StationFollowers]  WITH CHECK ADD  CONSTRAINT [FK_StationFollowers_StationId] FOREIGN KEY([StationId])
REFERENCES [dbo].[Stations] ([Id])
GO
ALTER TABLE [dbo].[StationFollowers] CHECK CONSTRAINT [FK_StationFollowers_StationId]
GO
ALTER TABLE [dbo].[StationFollowers]  WITH CHECK ADD  CONSTRAINT [FK_StationFollowers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[StationFollowers] CHECK CONSTRAINT [FK_StationFollowers_UserId]
GO
ALTER TABLE [dbo].[Stations]  WITH CHECK ADD  CONSTRAINT [FK_Stations_StationImageId] FOREIGN KEY([StationImageId])
REFERENCES [dbo].[Images] ([Id])
GO
ALTER TABLE [dbo].[Stations] CHECK CONSTRAINT [FK_Stations_StationImageId]
GO
ALTER TABLE [dbo].[Stations]  WITH CHECK ADD  CONSTRAINT [FK_Stations_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Stations] CHECK CONSTRAINT [FK_Stations_UserId]
GO
ALTER TABLE [dbo].[UserFriends]  WITH CHECK ADD  CONSTRAINT [FK_UserFriends_RequestorId] FOREIGN KEY([RequestorId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserFriends] CHECK CONSTRAINT [FK_UserFriends_RequestorId]
GO
ALTER TABLE [dbo].[UserFriends]  WITH CHECK ADD  CONSTRAINT [FK_UserFriends_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserFriends] CHECK CONSTRAINT [FK_UserFriends_UserId]
GO
ALTER TABLE [dbo].[UserLoopParts]  WITH CHECK ADD  CONSTRAINT [FK_UserLoopParts_UserLoopId] FOREIGN KEY([UserLoopId])
REFERENCES [dbo].[UserLoops] ([Id])
GO
ALTER TABLE [dbo].[UserLoopParts] CHECK CONSTRAINT [FK_UserLoopParts_UserLoopId]
GO
ALTER TABLE [dbo].[UserLoopParts]  WITH CHECK ADD  CONSTRAINT [FK_UserLoopParts_UserMelodyId] FOREIGN KEY([UserMelodyId])
REFERENCES [dbo].[UserMelodies] ([Id])
GO
ALTER TABLE [dbo].[UserLoopParts] CHECK CONSTRAINT [FK_UserLoopParts_UserMelodyId]
GO
ALTER TABLE [dbo].[UserLoops]  WITH CHECK ADD  CONSTRAINT [FK_UserLoops_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserLoops] CHECK CONSTRAINT [FK_UserLoops_UserId]
GO
ALTER TABLE [dbo].[UserMelodies]  WITH CHECK ADD  CONSTRAINT [FK_UserMelodies_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserMelodies] CHECK CONSTRAINT [FK_UserMelodies_UserId]
GO
ALTER TABLE [dbo].[UserMelodyParts]  WITH CHECK ADD  CONSTRAINT [FK_UserMelodyParts_MelodyId] FOREIGN KEY([MelodyId])
REFERENCES [dbo].[Melodies] ([Id])
GO
ALTER TABLE [dbo].[UserMelodyParts] CHECK CONSTRAINT [FK_UserMelodyParts_MelodyId]
GO
ALTER TABLE [dbo].[UserMelodyParts]  WITH CHECK ADD  CONSTRAINT [FK_UserMelodyParts_UserMelodyId] FOREIGN KEY([UserMelodyId])
REFERENCES [dbo].[UserMelodies] ([Id])
GO
ALTER TABLE [dbo].[UserMelodyParts] CHECK CONSTRAINT [FK_UserMelodyParts_UserMelodyId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserImageId] FOREIGN KEY([UserImageId])
REFERENCES [dbo].[Images] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserImageId]
GO
ALTER TABLE [dbo].[UserSessions]  WITH CHECK ADD  CONSTRAINT [FK_UserSessions_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserSessions] CHECK CONSTRAINT [FK_UserSessions_UserId]
GO

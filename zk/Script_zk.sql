USE [HeyVote]
GO
/****** Object:  UserDefinedFunction [dbo].[AppendParentPath]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Function [dbo].[AppendParentPath]
(
	@parent hierarchyid = null
)
RETURNS varchar(max) AS
Begin       
    Declare @result varchar(max), @newid uniqueidentifier

    SELECT @newid = new_id FROM dbo.getNewID; 

    SELECT @result = ISNULL(@parent.ToString(), '/') +
                     convert(varchar(20), convert(bigint, substring(convert(binary(16), @newid), 1, 6))) + '.' +
                     convert(varchar(20), convert(bigint, substring(convert(binary(16), @newid), 7, 6))) + '.' +
                     convert(varchar(20), convert(bigint, substring(convert(binary(16), @newid), 13, 4))) + '/'     
    RETURN @result 
End


GO
/****** Object:  UserDefinedFunction [dbo].[SplitString]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitString]
(    
      @Input NVARCHAR(MAX),
      @Character CHAR(1)
)
RETURNS @Output TABLE (
      Item NVARCHAR(1000)
)
AS
BEGIN
      DECLARE @StartIndex INT, @EndIndex INT
 
      SET @StartIndex = 1
      IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
      BEGIN
            SET @Input = @Input + @Character
      END
 
      WHILE CHARINDEX(@Character, @Input) > 0
      BEGIN
            SET @EndIndex = CHARINDEX(@Character, @Input)
           
            INSERT INTO @Output(Item)
            SELECT SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
           
            SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
      END
 
      RETURN
END

GO
/****** Object:  Table [dbo].[DET_Hash_Posts]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_Hash_Posts](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HasTagId] [bigint] NOT NULL,
	[PostId] [bigint] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_DET_Hash_Posts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_Post_Comments]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_Post_Comments](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PostId] [bigint] NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[Comment] [nvarchar](200) NOT NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_DET_Post_Comments_CreatedOn]  DEFAULT (getutcdate()),
	[ModifiedOn] [datetime] NOT NULL CONSTRAINT [DF_DET_Post_Comments_ModifiedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_DET_Post_Comments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_Post_SelectedContacts]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_Post_SelectedContacts](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PostId] [bigint] NOT NULL,
	[ContactUserIdf] [uniqueidentifier] NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_DET_Post_SelectedContacts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_Post_Subscribers]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_Post_Subscribers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PostId] [bigint] NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_DET_Post_Subscribers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_Post_Votes]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_Post_Votes](
	[Id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_DET_Post_Votes_Id]  DEFAULT (newid()),
	[PostId] [bigint] NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[VoteOption] [bit] NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_DET_Post_Votes_CreatedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_DET_Post_Votes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_User_Block]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_User_Block](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[BlockUserIdf] [uniqueidentifier] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_DET_User_Block] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_User_Contact]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DET_User_Contact](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[ContactUserIdf] [uniqueidentifier] NULL,
	[Name] [varchar](150) NOT NULL,
	[Number] [varchar](15) NOT NULL,
	[TerritoryId] [int] NOT NULL,
	[isAllowed] [bit] NULL CONSTRAINT [DF_DET_User_Contact_isAllowed]  DEFAULT ((1)),
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Contact_CreatedOn]  DEFAULT (getutcdate()),
	[ModifiedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Contact_ModifiedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_MST_Contact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DET_User_Follow]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_User_Follow](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[FollowUserIdf] [uniqueidentifier] NOT NULL,
	[TerritoryId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_DET_User_Follow_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_DET_User_Follow] UNIQUE NONCLUSTERED 
(
	[UserIdf] ASC,
	[FollowUserIdf] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_User_Notifications]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_User_Notifications](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PostId] [bigint] NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[ImageIdf] [uniqueidentifier] NULL,
	[FolderPath] [hierarchyid] NOT NULL,
	[NotifyUserIdf] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[PostCreatedOn] [datetime] NULL,
	[isDone] [bit] NULL,
	[isViewed] [bit] NOT NULL CONSTRAINT [DF_DET_User_Notifications_isViewed]  DEFAULT ((0)),
	[hasRead] [bit] NOT NULL CONSTRAINT [DF_DET_User_Notifications_hasRead]  DEFAULT ((0)),
	[Vote1Result] [int] NULL,
	[Vote2Result] [int] NULL,
	[Image1Idf] [uniqueidentifier] NULL,
	[Image2Idf] [uniqueidentifier] NULL,
	[DisplayName] [nvarchar](200) NULL,
	[Caption1] [nvarchar](100) NULL,
	[Caption2] [nvarchar](100) NULL,
	[EndDate] [datetime] NULL,
	[isPost] [bit] NOT NULL,
	[isFollow] [bit] NOT NULL,
	[isContact] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_DET_User_Notifications_CreatedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_DET_User_Notifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_User_Post_Spam]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_User_Post_Spam](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdf] [uniqueidentifier] NULL,
	[PostId] [bigint] NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_DET_User_Post_Spam] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_User_Subscribe]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_User_Subscribe](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[SubscribeUserIdf] [uniqueidentifier] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_DET_User_Subscribe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DET_User_Views]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DET_User_Views](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[ViewingUserIdf] [uniqueidentifier] NOT NULL,
	[TerritoryId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_DET_User_Views_CreatedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_DET_User_Views] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MST_Age_Ranges]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MST_Age_Ranges](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AgeRanges] [varchar](10) NOT NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Age_Ranges_CreatedOn]  DEFAULT (getutcdate()),
	[ModifiedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Age_Ranges_ModifiedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_MST_Age_Ranges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MST_Category]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MST_Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Category_CreatedOn]  DEFAULT (getutcdate()),
	[ModifiedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Category_ModifiedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_MST_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MST_Gender]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MST_Gender](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](15) NOT NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Gender_CreatedOn]  DEFAULT (getutcdate()),
	[ModifiedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Gender_ModifiedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_MST_Gender] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MST_HashTags]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MST_HashTags](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HashTag] [nvarchar](30) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_MST_HashTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MST_Posts_Active]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MST_Posts_Active](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[Title] [varchar](140) NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Caption1] [varchar](100) NOT NULL,
	[Caption2] [varchar](100) NOT NULL,
	[Img1Idf] [uniqueidentifier] NULL,
	[Img2Idf] [uniqueidentifier] NULL,
	[FolderPath] [hierarchyid] NULL,
	[VoteCount1] [bigint] NULL CONSTRAINT [DF_MST_Posts_Active_VoteCount1]  DEFAULT ((0)),
	[VoteCount2] [bigint] NULL CONSTRAINT [DF_MST_Posts_Active_VoteCount2]  DEFAULT ((0)),
	[Vote1Result] [int] NULL,
	[Vote2Result] [int] NULL,
	[Option1] [bit] NULL CONSTRAINT [DF_MST_Posts_Active_Won]  DEFAULT ((0)),
	[isDone] [bit] NOT NULL CONSTRAINT [DF_MST_Posts_Active_isDone]  DEFAULT ((0)),
	[NumberOfSubscribers] [bigint] NULL CONSTRAINT [DF_MST_Posts_Active_NumberOfSubscribers]  DEFAULT ((0)),
	[isPublic] [bit] NOT NULL CONSTRAINT [DF_MST_Posts_Active_isPublic]  DEFAULT ((0)),
	[toContacts] [bit] NULL CONSTRAINT [DF_MST_Posts_Active_toContacts]  DEFAULT ((0)),
	[toSelectedContacts] [bit] NULL CONSTRAINT [DF_MST_Posts_Active_toSelectedContacts]  DEFAULT ((0)),
	[TerritoryId] [int] NULL,
	[CategoryId] [int] NOT NULL,
	[isActive] [bit] NOT NULL CONSTRAINT [DF_MST_Posts_Active_isActive]  DEFAULT ((1)),
	[isGlobal] [bit] NOT NULL CONSTRAINT [DF_MST_Posts_Active_isGlobal]  DEFAULT ((0)),
	[TypeId] [int] NOT NULL CONSTRAINT [DF_MST_Posts_Active_TypeId]  DEFAULT ((1)),
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Posts_CreatedOn]  DEFAULT (getutcdate()),
	[MoifiedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Posts_MoifiedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_MST_Posts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MST_Resources]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ARITHABORT ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MST_Resources] AS FILETABLE ON [PRIMARY] FILESTREAM_ON [HeyVoteFileGroup]
WITH
(
FILETABLE_DIRECTORY = N'MST_Resources', FILETABLE_COLLATE_FILENAME = Latin1_General_CI_AS
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MST_Resources1]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MST_Resources1](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResourceId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[FileName] [varchar](100) NULL,
	[Data] [varbinary](max) FILESTREAM  NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Resources_CreatedOn]  DEFAULT (getutcdate()),
	[ModifiedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_Resources_ModifiedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_MST_Resources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] FILESTREAM_ON [HeyVoteFileGroup],
 CONSTRAINT [UQ__Resource__3214EC064C9B982D] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Resource__DEA9AE6CCA9F6C7F] UNIQUE NONCLUSTERED 
(
	[ResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] FILESTREAM_ON [HeyVoteFileGroup]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MST_Territory]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MST_Territory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Territory] [varchar](100) NOT NULL,
	[CountryCode] [varchar](5) NULL,
	[CreatedOn] [datetime] NULL CONSTRAINT [DF_MST_Territory_CreatedOn]  DEFAULT (getutcdate()),
	[ModifiedOn] [datetime] NULL CONSTRAINT [DF_MST_Territory_ModifiedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_MST_Territory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MST_User]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MST_User](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Idf] [uniqueidentifier] NOT NULL CONSTRAINT [DF_MST_User_Idf]  DEFAULT (newid()),
	[UserName] [nvarchar](100) NOT NULL,
	[DisplayName] [nvarchar](150) NOT NULL,
	[OwnNumber] [varchar](15) NOT NULL,
	[Status] [nvarchar](100) NOT NULL,
	[ImageIdf] [uniqueidentifier] NULL,
	[FolderPath] [hierarchyid] NULL,
	[TerritoryId] [int] NOT NULL,
	[GenderID] [int] NULL,
	[AgeRangeId] [int] NULL,
	[DomainName] [nvarchar](100) NULL,
	[HowOld] [int] NULL,
	[LastLoggedIn] [datetime] NOT NULL CONSTRAINT [DF_MST_User_LastLoggedIn]  DEFAULT (getdate()),
	[isActive] [bit] NOT NULL CONSTRAINT [DF_MST_User_isActive]  DEFAULT ((1)),
	[isSpecial] [bit] NULL CONSTRAINT [DF_MST_User_isSpecial]  DEFAULT ((0)),
	[Rank] [int] NULL,
	[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_User_CreatedOn]  DEFAULT (getutcdate()),
	[ModifiedOn] [datetime] NOT NULL CONSTRAINT [DF_MST_User_ModifiedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_MST_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Posts_Calculated]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Posts_Calculated](
	[Id] [bigint] NOT NULL,
	[UserIdf] [uniqueidentifier] NOT NULL,
	[Title] [varchar](140) NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Caption1] [varchar](100) NOT NULL,
	[Caption2] [varchar](100) NOT NULL,
	[Img1Idf] [uniqueidentifier] NULL,
	[Img2Idf] [uniqueidentifier] NULL,
	[VoteCount1] [bigint] NULL,
	[VoteCount2] [bigint] NULL,
	[Option1] [bit] NULL,
	[isDone] [bit] NOT NULL,
	[NumberOfSubscribers] [bigint] NOT NULL,
	[isPublic] [bit] NOT NULL,
	[TerritoryId] [int] NULL,
	[isGlobal] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[MoifiedOn] [datetime] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [user].[ContactList]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE View [user].[ContactList]
As
(
	Select
		a.Id, a.Name, a.Number, a.TerritoryId, d.Idf, e.isAllowed
		, Case when f.Id is null and e.isAllowed = 1 and a.isAllowed = 1 then a.ContactUserIdf else null end as ContactUserIdf
		, Case when f.Id is null and e.isAllowed = 1 and a.isAllowed = 1 then b.ImageIdf else null end as ImageIdf
		, Case When f.Id is null and e.isAllowed = 1 and a.isAllowed = 1 then b.FolderPath else null end as FolderPath
		, Case when f.Id is null and e.isAllowed = 1 and a.isAllowed = 1 then b.[Status] else null end as [Status]
		, Case when f.Id is null and e.isAllowed = 1 and a.isAllowed = 1 then 1 else 0 end as isAvailable
		, UPPER(substring(a.Name, 1, 1)) as Alias
	From
		MST_User d
	Inner Join
		dbo.DET_User_Contact a
	On
		d.Idf = a.UserIdf and d.isActive = 1
	Left Join
		dbo.DET_User_Contact e
	On
		a.ContactUserIdf = e.UserIdf and e.ContactUserIdf = a.UserIdf
	Left Join
		dbo.MST_User b
	On
		a.ContactUserIdf = b.Idf and b.isActive = 1
	Left Join
		dbo.DET_User_Block f
	On
		f.UserIdf = a.ContactUserIdf and f.BlockUserIdf = a.UserIdf

)


GO
/****** Object:  View [posts].[LivePosts]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO










	
CREATE View [posts].[LivePosts]
As
(
	Select
		a.Id, a.UserIdf
		, d.DisplayName
		, d.ImageIdf, a.EndDate, a.CreatedOn, d.FolderPath, a.FolderPath as PostFolderPath
		, a.Title, a.Caption1, a.Caption2, a.Img1Idf, a.Img2Idf, a.TypeId
		, a.Vote1Result, a.Vote2Result, a.isDone, a.isPublic, a.TerritoryId, a.isGlobal
		, Case When a.toContacts = 1 then b.ContactUserIdf else c.ContactUserIdf end as ContactUserIdf
	From
		dbo.MST_Posts_Active a
	Inner Join
		dbo.MST_User d
	On
		a.UserIdf = d.Idf
	Left Join
		[user].ContactList b
	On
		a.toContacts = 1 and b.isAvailable = 1 and a.UserIdf = b.Idf and (a.isGlobal = 1 or a.TerritoryId = b.TerritoryId)
	Left Join
		dbo.DET_Post_SelectedContacts c
	On
		a.toSelectedContacts = 1 and a.Id = c.PostId
	Left Join
		[user].ContactList e
	On
		a.toSelectedContacts = 1 and e.isAvailable = 1 and c.ContactUserIdf = e.Idf and (a.isGlobal = 1 or a.TerritoryId = e.TerritoryId)
	Where
		((a.toContacts = 1 and b.ContactUserIdf is not null) or (a.toSelectedContacts = 1 and c.ContactUserIdf is not null and e.Id is not null)
		or a.isPublic = 1) and a.isActive = 1 and d.isActive = 1 -- and a.Id= 10184

)












GO
/****** Object:  View [dbo].[getNewID]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create view [dbo].[getNewID] as select newid() as new_id 

GO
/****** Object:  View [user].[UserRank]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create View [user].[UserRank]
As
(
	Select
		a.Id, a.Idf, a.DisplayName, a.[Status], ROW_NUMBER() over (order by COUNT(c.Id) desc) as URank, COUNT(c.Id) as Votes
	From
		MST_User a
	Left Join
		MST_Posts_Active b
	On
		a.Idf = b.UserIdf and b.isActive = 1
	Left Join
		DET_Post_Votes c
	On
		b.Id = c.PostId
	Where
		a.isActive = 1
	Group By
		a.Id, a.Idf, a.DisplayName, a.[Status]
)
GO
ALTER TABLE [dbo].[DET_Hash_Posts] ADD  CONSTRAINT [DF_DET_Hash_Posts_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DET_Post_SelectedContacts] ADD  CONSTRAINT [DF_DET_Post_SelectedContacts_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DET_Post_Subscribers] ADD  CONSTRAINT [DF_DET_Post_Subscribers_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DET_Post_Subscribers] ADD  CONSTRAINT [DF_DET_Post_Subscribers_ModifiedOn]  DEFAULT (getutcdate()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[DET_User_Block] ADD  CONSTRAINT [DF_DET_User_Block_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DET_User_Follow] ADD  CONSTRAINT [DF_DET_User_Follow_TerritoryId]  DEFAULT ((1)) FOR [TerritoryId]
GO
ALTER TABLE [dbo].[DET_User_Follow] ADD  CONSTRAINT [DF_DET_User_Follow_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DET_User_Post_Spam] ADD  CONSTRAINT [DF_DET_User_Post_Spam_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DET_User_Subscribe] ADD  CONSTRAINT [DF_DET_User_Subscribe_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DET_User_Subscribe] ADD  CONSTRAINT [DF_DET_User_Subscribe_ModifiedOn]  DEFAULT (getutcdate()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[MST_HashTags] ADD  CONSTRAINT [DF_MST_HashTags_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Posts_Calculated] ADD  CONSTRAINT [DF_Posts_Calculated_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Posts_Calculated] ADD  CONSTRAINT [DF_Posts_Calculated_MoifiedOn]  DEFAULT (getutcdate()) FOR [MoifiedOn]
GO
ALTER TABLE [dbo].[DET_Post_SelectedContacts]  WITH NOCHECK ADD  CONSTRAINT [FK_DET_Post_SelectedContacts_MST_Posts_Active] FOREIGN KEY([PostId])
REFERENCES [dbo].[MST_Posts_Active] ([Id])
GO
ALTER TABLE [dbo].[DET_Post_SelectedContacts] CHECK CONSTRAINT [FK_DET_Post_SelectedContacts_MST_Posts_Active]
GO
ALTER TABLE [dbo].[DET_User_Contact]  WITH NOCHECK ADD  CONSTRAINT [FK_MST_Contact_MST_Contact] FOREIGN KEY([Id])
REFERENCES [dbo].[DET_User_Contact] ([Id])
GO
ALTER TABLE [dbo].[DET_User_Contact] CHECK CONSTRAINT [FK_MST_Contact_MST_Contact]
GO
ALTER TABLE [dbo].[DET_User_Contact]  WITH NOCHECK ADD  CONSTRAINT [FK_MST_Contact_MST_Territory] FOREIGN KEY([TerritoryId])
REFERENCES [dbo].[MST_Territory] ([Id])
GO
ALTER TABLE [dbo].[DET_User_Contact] CHECK CONSTRAINT [FK_MST_Contact_MST_Territory]
GO
ALTER TABLE [dbo].[DET_User_Follow]  WITH CHECK ADD  CONSTRAINT [FK_DET_User_Follow_MST_User] FOREIGN KEY([FollowUserIdf])
REFERENCES [dbo].[MST_User] ([Idf])
GO
ALTER TABLE [dbo].[DET_User_Follow] CHECK CONSTRAINT [FK_DET_User_Follow_MST_User]
GO
ALTER TABLE [dbo].[DET_User_Post_Spam]  WITH CHECK ADD  CONSTRAINT [FK_DET_User_Post_Spam_MST_Posts_Active] FOREIGN KEY([PostId])
REFERENCES [dbo].[MST_Posts_Active] ([Id])
GO
ALTER TABLE [dbo].[DET_User_Post_Spam] CHECK CONSTRAINT [FK_DET_User_Post_Spam_MST_Posts_Active]
GO
ALTER TABLE [dbo].[DET_User_Post_Spam]  WITH CHECK ADD  CONSTRAINT [FK_DET_User_Post_Spam_MST_User] FOREIGN KEY([UserIdf])
REFERENCES [dbo].[MST_User] ([Idf])
GO
ALTER TABLE [dbo].[DET_User_Post_Spam] CHECK CONSTRAINT [FK_DET_User_Post_Spam_MST_User]
GO
ALTER TABLE [dbo].[MST_Posts_Active]  WITH CHECK ADD  CONSTRAINT [FK_MST_Posts_Active_MST_Territory] FOREIGN KEY([TerritoryId])
REFERENCES [dbo].[MST_Territory] ([Id])
GO
ALTER TABLE [dbo].[MST_Posts_Active] CHECK CONSTRAINT [FK_MST_Posts_Active_MST_Territory]
GO
ALTER TABLE [dbo].[MST_Posts_Active]  WITH CHECK ADD  CONSTRAINT [FK_MST_Posts_Active_MST_User] FOREIGN KEY([UserIdf])
REFERENCES [dbo].[MST_User] ([Idf])
GO
ALTER TABLE [dbo].[MST_Posts_Active] CHECK CONSTRAINT [FK_MST_Posts_Active_MST_User]
GO
ALTER TABLE [dbo].[MST_User]  WITH CHECK ADD  CONSTRAINT [FK_MST_User_MST_Age_Ranges] FOREIGN KEY([AgeRangeId])
REFERENCES [dbo].[MST_Age_Ranges] ([Id])
GO
ALTER TABLE [dbo].[MST_User] CHECK CONSTRAINT [FK_MST_User_MST_Age_Ranges]
GO
ALTER TABLE [dbo].[MST_User]  WITH CHECK ADD  CONSTRAINT [FK_MST_User_MST_Gender] FOREIGN KEY([GenderID])
REFERENCES [dbo].[MST_Gender] ([Id])
GO
ALTER TABLE [dbo].[MST_User] CHECK CONSTRAINT [FK_MST_User_MST_Gender]
GO
ALTER TABLE [dbo].[MST_User]  WITH CHECK ADD  CONSTRAINT [FK_MST_User_MST_Territory] FOREIGN KEY([TerritoryId])
REFERENCES [dbo].[MST_Territory] ([Id])
GO
ALTER TABLE [dbo].[MST_User] CHECK CONSTRAINT [FK_MST_User_MST_Territory]
GO
/****** Object:  StoredProcedure [contact].[AddContact]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [contact].[AddContact]
(
	@UserIdf uniqueidentifier,
	@ContactUserIdf uniqueidentifier = null,
	@Name varchar(150),
	@Number varchar(15),
	@TerritoryId int
)
As
Begin

	Insert into 
		DET_User_Contact(UserIdf, ContactUserIdf, Name, Number, TerritoryId)
	Values
		(@UserIdf, @ContactUserIdf, @Name, @Number, @TerritoryId)

End
GO
/****** Object:  StoredProcedure [contact].[GetBlockedUsers]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [contact].[GetBlockedUsers]
(
	@UserIdf uniqueidentifier = '57DA548E-04BE-4643-ABED-8D9D256B4C88'
)
As
Begin

	Select
		a.Id, b.DisplayName as Name, b.OwnNumber as Number, b.TerritoryId
		, b.Idf as ContactUserIdf
		, b.ImageIdf as ImageIdf
		, b.FolderPath as FolderPath
		, b.[Status] as [Status], 0 as PostId, null as VoteEndDate
		, UPPER(substring(b.DisplayName, 1, 1)) as Alias
	From
		MST_User d
	Inner Join
		dbo.DET_User_Block a
	On
		d.Idf = a.UserIdf and d.isActive = 1
	Inner Join
		dbo.MST_User b
	On
		b.Idf = a.BlockUserIdf and b.isActive = 1
	Where
		a.UserIdf = @UserIdf

	union

	Select
		a.Id, b.DisplayName as Name, b.OwnNumber as Number, b.TerritoryId
		, b.Idf as ContactUserIdf
		, b.ImageIdf as ImageIdf
		, b.FolderPath as FolderPath
		, b.[Status] as [Status], 0 as PostId, null as VoteEndDate
		, UPPER(substring(b.DisplayName, 1, 1)) as Alias
	From
		MST_User d
	Inner Join
		dbo.DET_User_Contact a
	On
		d.Idf = a.UserIdf and d.isActive = 1 and a.isAllowed = 0
	Inner Join
		dbo.MST_User b
	On
		b.Idf = a.ContactUserIdf and b.isActive = 1
	Where
		a.UserIdf = @UserIdf

End
GO
/****** Object:  StoredProcedure [contact].[GetContactList]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [contact].[GetContactList]
(
	@UserIdf uniqueidentifier = '9B08A236-5DB9-4BFE-BBD5-F56E366BC2BC'
)
As
Begin

	Select
		a.Id, a.Name, a.Number, a.TerritoryId
		, a.ContactUserIdf
		, a.ImageIdf
		, a.FolderPath
		, a.[Status]
		, Case when a.isAvailable is null then c.Id else null end as PostId
		, Case when a.isAvailable is null then c.EndDate else null end as VoteEndDate
		, a.Alias
	From
		[user].ContactList a
	Left Join
		dbo.MST_Posts_Active c
	On
		c.isDone = 0 and c.UserIdf = a.ContactUserIdf
	Where
		a.Idf = @UserIdf

End
GO
/****** Object:  StoredProcedure [contact].[GetFollowers]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [contact].[GetFollowers]
(
	@UserIdf uniqueidentifier = '5C993AA9-2FB2-4379-8A63-9D7BA53A02B9'
)
As
Begin

	Select
		a.Id, b.DisplayName as Name, b.OwnNumber as Number, b.TerritoryId
		, a.UserIdf as ContactUserIdf
		, b.ImageIdf as ImageIdf
		, b.FolderPath as FolderPath
		, b.[Status] as [Status]
		, c.Id as PostId
		, c.EndDate as VoteEndDate
		, UPPER(substring(b.DisplayName, 1, 1)) as Alias
	From
		MST_User d
	Inner Join
		dbo.DET_User_Follow a
	On
		d.Idf = a.FollowUserIdf and d.isActive = 1
	Inner Join
		dbo.MST_User b
	On
		a.UserIdf = b.Idf and b.isActive = 1
	Left Join
		dbo.MST_Posts_Active c
	On
		c.isDone = 0 and c.UserIdf = a.FollowUserIdf
	Where
		a.FollowUserIdf = @UserIdf

End
GO
/****** Object:  StoredProcedure [contact].[GetFollowing]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [contact].[GetFollowing]
(
	@UserIdf uniqueidentifier = '5C993AA9-2FB2-4379-8A63-9D7BA53A02B9'
)
As
Begin

	Select
		a.Id, b.DisplayName as Name, b.OwnNumber as Number, b.TerritoryId
		, a.FollowUserIdf as ContactUserIdf
		, b.ImageIdf as ImageIdf
		, b.FolderPath as FolderPath
		, b.[Status] as [Status]
		, c.Id as PostId
		, c.EndDate as VoteEndDate
		, UPPER(substring(b.DisplayName, 1, 1)) as Alias
	From
		MST_User d
	Inner Join
		dbo.DET_User_Follow a
	On
		d.Idf = a.UserIdf and d.isActive = 1
	Inner Join
		dbo.MST_User b
	On
		a.FollowUserIdf = b.Idf and b.isActive = 1
	Left Join
		dbo.MST_Posts_Active c
	On
		c.isDone = 0 and c.UserIdf = a.UserIdf
	Where
		d.Idf = @UserIdf

End
GO
/****** Object:  StoredProcedure [contact].[RefreshContacts]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [contact].[RefreshContacts]
(
	@UserIdf uniqueidentifier = '9181942E-A867-4759-9709-244D8ECDC33B'
)
As
Begin

	Update a
	Set 
		a.ContactUserIdf = b.Idf
	From
		DET_User_Contact a
	Left Join
		MST_User b
	On
		a.Number = b.OwnNumber
	Where
		a.UserIdf = @UserIdf and a.ContactUserIdf is null

End




GO
/****** Object:  StoredProcedure [contact].[SavePostContacts]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [contact].[SavePostContacts]
(
	@UserIdf uniqueidentifier = '57da548e-04be-4643-abed-8d9d256b4c88',
	@TerritoryId int,
	@isGlobal bit,
	@postContacts dbo.ContactType readonly,
	@PostId bigint
)
As
Begin

	Insert into 
		DET_Post_SelectedContacts (PostId, ContactUserIdf)
	Select
		@PostId, a.ContactUserIdf
	From
		DET_User_Contact a
	Inner Join
		@postContacts b
	On
		a.ContactUserIdf = b.ContactUserIdf and (@isGlobal = 1 or a.TerritoryId = @TerritoryId)
	Where
		a.UserIdf = @UserIdf


End
GO
/****** Object:  StoredProcedure [contact].[SyncContacts]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [contact].[SyncContacts]
(
	@UserIdf uniqueidentifier = '57da548e-04be-4643-abed-8d9d256b4c88',
	@syncContacts dbo.ContactType readonly
)
As
Begin

	Begin Try

		Begin Tran
		-- Newly added contacts
		Insert into
			dbo.DET_User_Contact (Number, TerritoryId, Name, UserIdf, ContactUserIdf)
		Select
			a.Number, a.TerritoryId, b.Name, @UserIdf, c.Idf
		From
		(
			Select
				a.Number, a.TerritoryId
			From
				@syncContacts a

			except

			Select
				a.Number, a.TerritoryId
			From
				dbo.DET_User_Contact a
			Where
				a.UserIdf = @UserIdf
		) a
		Inner Join
			@syncContacts b
		On
			a.Number = b.Number and a.TerritoryId = b.TerritoryId
		Left Join
			dbo.MST_User c
		On
			a.Number = c.OwnNumber and a.TerritoryId = c.TerritoryId

		-- to delete contacts
		Delete a
		From
			dbo.DET_User_Contact a
		Inner Join
		(
			Select
				a.Number, a.TerritoryId, b.Name
			From
			(

				Select
					a.Number, a.TerritoryId
				From
					dbo.DET_User_Contact a
				Where
					a.UserIdf = @UserIdf

				except

				Select
					a.Number, a.TerritoryId
				From
					@syncContacts a

			) a
			Inner Join
				dbo.DET_User_Contact b
			On
				a.Number = b.Number and a.TerritoryId = b.TerritoryId and b.UserIdf = @UserIdf
		) b
		On
			a.UserIdf = @UserIdf and a.Number = b.Number and a.TerritoryId = b.TerritoryId

		Commit tran

	End Try
	Begin Catch

		Rollback tran

		Raiserror('409', 16, 1)

	End Catch



End





GO
/****** Object:  StoredProcedure [dbo].[GetData]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[GetData]
(
	@ResouceId uniqueidentifier
)
As
Begin

	Select
		a.Data.PathName() as FilePathName, GET_FILESTREAM_TRANSACTION_CONTEXT() as DataContext 
	From
		MST_Resources a
	Where
		a.ResourceId = @ResouceId


End

GO
/****** Object:  StoredProcedure [dbo].[SchedulePost]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[SchedulePost]
	@parameter1 [nvarchar](max)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MySQLCLRProject].[StoredProcedures].[SchedulePost]
GO
/****** Object:  StoredProcedure [dbo].[spWcfCall]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[spWcfCall]
	@parameter1 [nvarchar](max)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MySQLCLRProject].[StoredProcedures].[WcfSp]
GO
/****** Object:  StoredProcedure [posts].[AddComment]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[AddComment]
(
	@UserIdf uniqueidentifier,
	@TerritoryId int,
	@PostId bigint,
	@Comment nvarchar(200)
)
As
Begin

	if exists (
					select
						1
					from
						posts.LivePosts a
					Left Join
						dbo.DET_User_Block b
					On
						a.UserIdf = b.UserIdf and b.BlockUserIdf = @UserIdf 
					where
						a.Id = @PostId and b.Id is null and ((a.isPublic = 1 and (a.isGlobal = 1 or a.TerritoryId = @TerritoryId)) or (a.UserIdf = @UserIdf or a.ContactUserIdf = @UserIdf))
			   )
	Begin

		Insert into 
			DET_Post_Comments (PostId, UserIdf, Comment)
		output
			inserted.Id, inserted.PostId, inserted.UserIdf, inserted.Comment, inserted.CreatedOn
		Values
			(@PostId, @UserIdf, @Comment)

	End
	else	
	Begin
		raiserror('801', 16, 1)
	End

End
GO
/****** Object:  StoredProcedure [posts].[AddPost]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[AddPost]
(
	@UserIdf uniqueidentifier,
	@Title varchar(140),
	@EndDate datetime,
	@Caption1 varchar(100),
	@Caption2 varchar(100),
	@Img1Idf uniqueidentifier,
	@Img2Idf uniqueidentifier = null,
	@NumberOfSubscribers bigint = null,
	@isPublic bit,
	@toContacts bit,
	@toSelectedContacts bit,
	@TerritoryId int = null,
	@isGlobal bit,
	@TypeId int,
	@CategoryId int,
	@FolderPath hierarchyId
)
As
Begin

	Insert into 
		MST_Posts_Active(UserIdf, Title, EndDate, Caption1, Caption2, Img1Idf, Img2Idf, CategoryId
		, NumberOfSubscribers, isPublic, toContacts, toSelectedContacts, TerritoryId, isGlobal, TypeId, FolderPath)
	Values
		(@UserIdf, @Title, @EndDate, @Caption1, @Caption2, @Img1Idf, @Img2Idf, @CategoryId
		, @NumberOfSubscribers, @isPublic, @toContacts, @toSelectedContacts, @TerritoryId, @isGlobal, @TypeId, @FolderPath)

	Select
		SCOPE_IDENTITY() as PostId

End
GO
/****** Object:  StoredProcedure [posts].[AddPostSpam]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[AddPostSpam]
(
	@UserIdf uniqueidentifier,
	@PostId bigint
)
As
Begin

	Insert into 
		DET_User_Post_Spam(UserIdf, PostId)
	Values
		(@UserIdf, @PostId)

End
GO
/****** Object:  StoredProcedure [posts].[CalculatePostResults]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[CalculatePostResults]
As
Begin

	Declare @PostIds nvarchar(max) = ''

	Update a
	Set
		@PostIds = @PostIds + Cast(a.Id as nvarchar(max)) + ',',
		a.Vote1Result = Case When (a.VoteCount1 + a.VoteCount2) > 0 then Round((Cast(a.VoteCount1 as float) / (a.VoteCount1 + a.VoteCount2) * 100), 0) else 50 end,
		a.Vote2Result = Case When (a.VoteCount1 + a.VoteCount2) > 0 then Round((Cast(a.VoteCount2 as float) / (a.VoteCount1 + a.VoteCount2) * 100), 0) else 50 end,
		a.isDone = 1,
		a.MoifiedOn = GETUTCDATE()
	--Select
	--	a.Id, a.UserIdf, a.Title, a.VoteCount1, a.VoteCount2
	--	, Case When (a.VoteCount1 + a.VoteCount2) > 0 then Round((Cast(a.VoteCount1 as float) / (a.VoteCount1 + a.VoteCount2) * 100), 0) else 50 end
	--	, Case When (a.VoteCount1 + a.VoteCount2) > 0 then Round((Cast(a.VoteCount2 as float) / (a.VoteCount1 + a.VoteCount2) * 100), 0) else 50 end
	From
		dbo.MST_Posts_Active a
	Where
		a.isDone = 0 and a.EndDate <= GETUTCDATE()

	if (@PostIds <> '')
		exec spWcfCall @PostIds

End
GO
/****** Object:  StoredProcedure [posts].[ChangeNotificationStatus]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[ChangeNotificationStatus]
(
	@UserIdf uniqueidentifier,
	@NotificationId bigint
)
As
Begin

	Update a
	Set
		a.hasRead = 1
	From
		DET_User_Notifications a
	Where
		a.Id = @NotificationId and a.NotifyUserIdf = @UserIdf

End
GO
/****** Object:  StoredProcedure [posts].[DeleteComment]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[DeleteComment]
(
	@Id bigint,
	@UserIdf uniqueidentifier,
	@PostId bigint
)
As
Begin

	Declare @isOwner bit = 0

	Select
		@isOwner = Case When a.UserIdf = @UserIdf then 1 else 0 end 
	From
		MST_Posts_Active a
	Where
		a.isActive = 1 and a.Id = @PostId

	Delete a
	From
		DET_Post_Comments a
	Where
		a.Id = @Id and (a.UserIdf = @UserIdf or @isOwner = 1) and a.PostId = @PostId

End
GO
/****** Object:  StoredProcedure [posts].[DeletePost]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [posts].[DeletePost]
(
	@UserIdf uniqueidentifier,
	@PostId bigint
)
As
Begin

	Update a
	Set
		a.isActive = 0		
	From
		MST_Posts_Active a
	Where
		a.UserIdf = @UserIdf and a.Id = @PostId and a.isActive = 1

	if (@@ROWCOUNT = 0)
	Begin

		raiserror('510', 16, 1)

	End

End
GO
/****** Object:  StoredProcedure [posts].[EditComment]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[EditComment]
(
	@Id bigint,
	@UserIdf uniqueidentifier,
	@PostId bigint,
	@Comment nvarchar(200)
)
As
Begin

	if exists (select 1 from posts.LivePosts a where a.Id = @PostId and (a.isPublic = 1 or (a.UserIdf = @UserIdf or a.ContactUserIdf = @UserIdf)))
	Begin

		Update a
		Set
			a.Comment = @Comment
		From
			DET_Post_Comments a
		Where
			a.Id = @Id

	End
	else	
	Begin
		raiserror('803', 16, 1)
	End

End
GO
/****** Object:  StoredProcedure [posts].[GetCategoryList]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [posts].[GetCategoryList]
As
Begin

	Select
		a.Id, a.Description as Category, a.CreatedOn
	From
		MST_Category a

End
GO
/****** Object:  StoredProcedure [posts].[GetComments]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[GetComments]
(
	@UserIdf uniqueidentifier,
	@PostId bigint,
	@PageId int = 1,
	@PageSize int = 7
)
As
Begin

	Declare @Offset int = @PageSize * @PageId

	if exists (select 1 from posts.LivePosts a where a.Id = @PostId and (a.isPublic = 1 or (a.UserIdf = @UserIdf or a.ContactUserIdf = @UserIdf)) and a.isDone = 1)
	Begin

		Select
			a.Id, b.DisplayName, a.Comment, a.UserIdf, b.ImageIdf, b.FolderPath, a.CreatedOn, a.ModifiedOn
		From
			DET_Post_Comments a
		Inner Join
			MST_User b
		On
			a.UserIdf = b.Idf
		Where
			a.PostId = @PostId and b.isActive = 1
		Order By
			a.CreatedOn
		Offset @Offset rows
		Fetch next @PageSize rows Only

	End
	else	
	Begin
		raiserror('803', 16, 1)
	End

End
GO
/****** Object:  StoredProcedure [posts].[GetContactPostsList]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [posts].[GetContactPostsList]
(
	@UserIdf uniqueidentifier = 'cf8ca6f6-f7ff-494b-b3b4-5b6bd622256d',
	@ContactUserIdf uniqueidentifier,
	@TerritoryId int,
	@PageId int = 1,
	@PageSize int = 7
)
As
Begin

	Declare @Offset int = @PageSize * @PageId

	Select
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.DisplayName, a.Title, a.Img1Idf, a.Img2Idf, a.Caption1
		, a.Caption2, a.EndDate, a.CreatedOn, d.VoteOption, a.TypeId, a.PostFolderPath
		, COUNT(c.Id) as VoteCount, COUNT(e.Id) as CommentCount, Case When d.Id is null then 1 else 0 end as hasVoted
	From
		posts.LivePosts a
	Left Join	
		dbo.DET_User_Contact f
	On
		a.isPublic = 1 and (f.UserIdf = a.UserIdf and f.ContactUserIdf = @UserIdf)
	Left Join
		dbo.DET_Post_Votes d
	On
		a.Id = d.PostId and d.UserIdf = @UserIdf
	Left Join
		dbo.DET_Post_Votes c
	On
		a.Id = c.PostId
	Left Join
		dbo.DET_Post_Comments e
	On
		a.Id = e.PostId
	Left Join
		dbo.DET_User_Block g
	On
		a.UserIdf = g.UserIdf and g.BlockUserIdf = @UserIdf
	Where
		(a.isDone = 0 and g.Id is null and a.UserIdf = @ContactUserIdf and (a.ContactUserIdf = @UserIdf or((a.isPublic = 1 and isnull(f.isAllowed, 1) <> 0) and (a.isGlobal = 1 or a.TerritoryId = @TerritoryId)))) 
	Group By
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.DisplayName, a.Title, a.Img1Idf, a.Img2Idf, a.Caption1
		, a.Caption2, a.EndDate, a.CreatedOn, d.Id, d.VoteOption, a.TypeId, a.PostFolderPath
	Order By
		a.CreatedOn desc
	Offset @Offset rows
	Fetch next @PageSize rows Only


End

GO
/****** Object:  StoredProcedure [posts].[GetMyPostsList]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [posts].[GetMyPostsList]
(
	@UserIdf uniqueidentifier = '57DA548E-04BE-4643-ABED-8D9D256B4C88',
	@PageId int = 0,
	@PageSize int = 7
)
As
Begin

	Declare @Offset int = @PageSize * @PageId

	Select
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.DisplayName, a.Title, a.Img1Idf, a.Img2Idf, a.Caption1
		, a.Caption2, a.EndDate, a.CreatedOn, a.isDone, a.Vote1Result, a.Vote2Result, e.VoteOption, a.TypeId
		, COUNT(c.Id) as VoteCount, COUNT(d.Id) as CommentCount, CAST(Case When e.Id is null then 0 else 1 end as bit) as hasVoted
	From
		posts.LivePosts a
	Left Join
		dbo.DET_Post_Votes c
	On
		a.Id = c.PostId
	Left Join
		dbo.DET_Post_Comments d
	On
		a.Id = d.PostId
	Left Join
		dbo.DET_Post_Votes e
	On
		a.Id = e.PostId and e.UserIdf = @UserIdf
	Where
		a.UserIdf = @UserIdf
	Group By
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.DisplayName, a.Title, a.Img1Idf, a.Img2Idf, a.Caption1
		, a.Caption2, a.EndDate, a.CreatedOn, a.isDone, a.Vote1Result, a.Vote2Result, e.Id, e.VoteOption, a.TypeId
	Order By
		a.CreatedOn desc
	Offset @Offset rows
	Fetch next @PageSize rows Only

End

GO
/****** Object:  StoredProcedure [posts].[GetMyVotesList]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [posts].[GetMyVotesList]
(
	@UserIdf uniqueidentifier = '9b08a236-5db9-4bfe-bbd5-f56e366bc2bc',
	@PageId int = 0,
	@PageSize int = 7
)
As
Begin

	Declare @Offset int = @PageSize * @PageId

	Select
		b.Id, b.UserIdf, c.ImageIdf, c.FolderPath, c.DisplayName, b.Title, b.Img1Idf, b.Img2Idf, b.Caption1
		, b.Caption2, b.EndDate, b.CreatedOn, b.isDone, b.Vote1Result, b.Vote2Result, COUNT(d.Id) as VoteCount
	From
		dbo.DET_Post_Votes a
	Inner Join
		dbo.MST_Posts_Active b
	On
		a.PostId = b.Id
	Inner Join
		dbo.MST_User c
	On
		b.UserIdf = c.Idf
	Left Join
		dbo.DET_Post_Votes d
	On
		a.PostId = d.PostId
	Where
		a.UserIdf = @UserIdf and c.isActive = 1
	Group By
		b.Id, b.UserIdf, c.ImageIdf, c.FolderPath, c.DisplayName, b.Title, b.Img1Idf, b.Img2Idf, b.Caption1
		, b.Caption2, b.EndDate, b.CreatedOn, b.isDone, b.Vote1Result, b.Vote2Result
	Order By
		b.CreatedOn desc
	Offset @Offset rows
	Fetch next @PageSize rows Only

End

GO
/****** Object:  StoredProcedure [posts].[GetOngoingHistoryPostsList]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[GetOngoingHistoryPostsList]
(
	@UserIdf uniqueidentifier = 'cf8ca6f6-f7ff-494b-b3b4-5b6bd622256d',
	@TerritoryId int,
	@PageId int = 1,
	@PageSize int = 7
)
As
Begin

	Declare @Offset int = @PageSize * @PageId

	Select
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.DisplayName, a.PostFolderPath
		, a.Title, a.Img1Idf, a.Img2Idf, a.Caption1, a.Caption2, a.Vote1Result, a.Vote2Result, a.TypeId
		, a.EndDate, a.CreatedOn, Case When d.Id is null then 1 else 0 end as VoteStatus, d.VoteOption
	From
		posts.LivePosts a
	Left Join	
		dbo.DET_User_Contact f
	On
		a.isPublic = 1 and (f.UserIdf = a.UserIdf and f.ContactUserIdf = @UserIdf)
	Left Join
		dbo.DET_Post_Votes d
	On
		a.Id = d.PostId and d.UserIdf = @UserIdf
	Left Join
		dbo.DET_User_Block g
	On
		a.UserIdf = g.UserIdf and g.BlockUserIdf = @UserIdf
	Where
		(a.isDone = 1 and g.Id is null and a.UserIdf <> @UserIdf and (a.ContactUserIdf = @UserIdf or ((a.isPublic = 1 and isnull(f.isAllowed, 1) <> 0) and (a.isGlobal = 1 or a.TerritoryId = @TerritoryId))))
	Order By
		a.CreatedOn desc
	Offset @Offset rows
	Fetch next @PageSize rows Only
		
End
GO
/****** Object:  StoredProcedure [posts].[GetOwnComments]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[GetOwnComments]
(
	@UserIdf uniqueidentifier,
	@PostId bigint,
	@PageId int = 1,
	@PageSize int = 7
)
As
Begin

	Declare @Offset int = @PageSize * @PageId

	if exists (select 1 from posts.LivePosts a where a.Id = @PostId and (a.isPublic = 1 or (a.UserIdf = @UserIdf or a.ContactUserIdf = @UserIdf)))
	Begin

		Select
			a.Id, b.DisplayName, a.Comment, a.UserIdf, b.ImageIdf, b.FolderPath, a.CreatedOn, a.ModifiedOn
		From
			DET_Post_Comments a
		Inner Join
			MST_User b
		On
			a.UserIdf = b.Idf
		Where
			a.PostId = @PostId and a.UserIdf = @UserIdf and b.isActive = 1
		Order By
			a.CreatedOn
		Offset @Offset rows
		Fetch next @PageSize rows Only

	End
	else	
	Begin
		raiserror('803', 16, 1)
	End

End
GO
/****** Object:  StoredProcedure [posts].[GetPostById]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[GetPostById]
(
	@UserIdf uniqueidentifier = '57da548e-04be-4643-abed-8d9d256b4c88',
	@TerritoryId int, -- Ziad '57da548e-04be-4643-abed-8d9d256b4c88'
	@PostId bigint = 10043 -- 3 -- 14
)
As
Begin

	Select
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.PostFolderPath, a.EndDate, a.CreatedOn, a.DisplayName
		, a.Title, a.Caption1, a.Caption2, a.Img1Idf, a.Img2Idf
		, a.Vote1Result, a.Vote2Result, a.isDone, b.VoteOption, a.TypeId
		, COUNT(c.Id) as VoteCount, COUNT(d.Id) as CommentCount, CAST(Case When b.Id is null then 0 else 1 end as bit) as hasVoted
	From
		posts.LivePosts a
	Left Join	
		dbo.DET_User_Contact f
	On
		a.isPublic = 1 and (f.UserIdf = a.UserIdf and f.ContactUserIdf = @UserIdf)
	Left Join
		DET_Post_Votes b
	On
		a.Id = b.PostId and b.UserIdf = @UserIdf
	Left Join
		dbo.DET_Post_Votes c
	On
		a.Id = c.PostId
	Left Join
		dbo.DET_Post_Comments d
	On
		a.Id = d.PostId
	Left Join
		dbo.DET_User_Block g
	On
		a.UserIdf = g.UserIdf and g.BlockUserIdf = @UserIdf
	Where
		a.Id = @PostId and g.id is null and ( (a.UserIdf = @UserIdf or a.ContactUserIdf = @UserIdf) or ((a.isPublic = 1 and isnull(f.isAllowed, 1) <> 0) and (a.isGlobal = 1 or a.TerritoryId = @TerritoryId)))  
	Group By
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.PostFolderPath, a.EndDate, a.CreatedOn, a.DisplayName
		, a.Title, a.Caption1, a.Caption2, a.Img1Idf, a.Img2Idf, a.TypeId
		, a.Vote1Result, a.Vote2Result, a.isDone, b.Id, b.VoteOption

End
GO
/****** Object:  StoredProcedure [posts].[GetPostsList]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [posts].[GetPostsList]
(
	@UserIdf uniqueidentifier = '57DA548E-04BE-4643-ABED-8D9D256B4C88',
	@TerritoryId int = 1,
	@PageId int = 0,
	@PageSize int = 7
)
As
Begin

	Declare @Offset int = @PageSize * @PageId

	Select
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.PostFolderPath
		, Case When h.Id is null then a.DisplayName else h.Name end as DisplayName, a.Title, a.Img1Idf, a.Img2Idf, a.Caption1
		, a.Caption2, a.EndDate, a.CreatedOn, a.isDone, d.VoteOption, a.TypeId
		, COUNT(c.Id) as VoteCount, COUNT(e.Id) as CommentCount, Cast(Case When d.Id is null then 0 else 1 end as bit) as hasVoted
	From
		posts.LivePosts a
	Left Join	
		dbo.DET_User_Contact f
	On
		a.isPublic = 1 and (f.UserIdf = a.UserIdf and f.ContactUserIdf = @UserIdf)
	Left Join
		dbo.DET_Post_Votes d
	On
		a.Id = d.PostId and d.UserIdf = @UserIdf
	Left Join
		dbo.DET_Post_Votes c
	On
		a.Id = c.PostId
	Left Join
		dbo.DET_Post_Comments e
	On
		a.Id = e.PostId
	Left Join
		dbo.DET_User_Block g
	On
		a.UserIdf = g.UserIdf and g.BlockUserIdf = @UserIdf
	Left Join
		dbo.DET_User_Contact h
	On
		h.UserIdf = @UserIdf and a.UserIdf = h.ContactUserIdf
	Where
		(a.UserIdf <> @UserIdf and g.Id is null and (a.ContactUserIdf = @UserIdf or (((a.isPublic = 1 and isnull(f.isAllowed, 1) <> 0) and (a.isGlobal = 1 or a.TerritoryId = @TerritoryId))))) 
	Group By
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.PostFolderPath, a.DisplayName, a.Title, a.Img1Idf, a.Img2Idf, a.Caption1
		, a.Caption2, a.EndDate, a.CreatedOn, d.Id, a.isDone, d.VoteOption, a.TypeId, h.Id, h.Name
	Order By
		a.CreatedOn desc
	Offset @Offset rows
	Fetch next @PageSize rows Only

End

GO
/****** Object:  StoredProcedure [posts].[GetPostsListByTerritoryId]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [posts].[GetPostsListByTerritoryId]
(
	@UserIdf uniqueidentifier = 'cf8ca6f6-f7ff-494b-b3b4-5b6bd622256d',
	@TerritoryId int,
	@PageId int = 1,
	@PageSize int = 7
)
As
Begin

	Declare @Offset int = @PageSize * @PageId

	Select
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.DisplayName, a.Title, a.Img1Idf, a.Img2Idf, a.Caption1
		, a.Caption2, a.EndDate, a.CreatedOn, d.VoteOption, a.TypeId, a.PostFolderPath
		, COUNT(c.Id) as VoteCount, COUNT(e.Id) as CommentCount, Case When d.Id is null then 1 else 0 end as hasVoted
	From
		posts.LivePosts a
	Left Join	
		dbo.DET_User_Contact f
	On
		a.isPublic = 1 and (f.UserIdf = a.UserIdf and f.ContactUserIdf = @UserIdf)
	Left Join
		dbo.DET_Post_Votes d
	On
		a.Id = d.PostId and d.UserIdf = @UserIdf
	Left Join
		dbo.DET_Post_Votes c
	On
		a.Id = c.PostId
	Left Join
		dbo.DET_Post_Comments e
	On
		a.Id = e.PostId
	Left Join
		dbo.DET_User_Block g
	On
		a.UserIdf = g.UserIdf and g.BlockUserIdf = @UserIdf
	Where
		(a.isDone = 0 and g.Id is null and a.TerritoryId = @TerritoryId and a.UserIdf <> @UserIdf and ((a.isPublic = 1 and isnull(f.isAllowed, 1) <> 0) or a.ContactUserIdf = @UserIdf) and d.Id is null) 
	Group By
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.DisplayName, a.Title, a.Img1Idf, a.Img2Idf, a.Caption1
		, a.Caption2, a.EndDate, a.CreatedOn, d.Id, d.VoteOption, a.TypeId, a.PostFolderPath
	Order By
		a.CreatedOn desc
	Offset @Offset rows
	Fetch next @PageSize rows Only

End

GO
/****** Object:  StoredProcedure [posts].[PurgePostResults]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [posts].[PurgePostResults]
As
Begin

	Delete From Posts_Calculated


End
GO
/****** Object:  StoredProcedure [posts].[ReHeyVotePost]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[ReHeyVotePost]
(
	@UserIdf uniqueidentifier = '57DA548E-04BE-4643-ABED-8D9D256B4C88',
	@TerritoryId int = 1,
	@PostId bigint = 10337 -- 10338
)
As
Begin
	--Declare @CreatedOn datetime = getdate()

	--Declare @EndDate datetime = dateadd(MI, 55, getdate())

	--Select @CreatedOn, @EndDate

	--Select Dateadd(MI, DATEDIFF(MI, @CreatedOn, @EndDate), getdate()) 

	Insert into 
		MST_Posts_Active(UserIdf, Title, EndDate, Caption1, Caption2, Img1Idf, Img2Idf, FolderPath
		, NumberOfSubscribers, isPublic, toContacts, toSelectedContacts, TerritoryId, isGlobal, TypeId)
	Select
		@UserIdf, a.Title, Dateadd(MI, DATEDIFF(MI, a.CreatedOn, a.EndDate), getdate()), a.Caption1, a.Caption2, a.Img1Idf, a.Img2Idf, c.FolderPath
		, a.NumberOfSubscribers, a.isPublic, a.toContacts, a.toSelectedContacts, a.TerritoryId, a.isGlobal, a.TypeId
	From
		MST_Posts_Active a
	Inner Join
		MST_User c
	On
		a.UserIdf = c.Idf and c.isActive = 1
	Left Join
		[user].ContactList b
	On
		a.UserIdf = b.Idf and b.ContactUserIdf = @UserIdf and b.isAllowed = 1
	Where
		a.isActive = 1 and a.Id = @PostId and ((a.isPublic = 1 or b.isAllowed is not null) and (a.isGlobal = 1 or a.TerritoryId = @TerritoryId))


	if (@@ROWCOUNT > 0)
	Begin

		Declare @MyPostId nvarchar(max) = Cast(scope_identity() as varchar(max))

		exec dbo.SchedulePost @MyPostId
		
	End
	else
	Begin
		raiserror('909', 16, 1)
	End
End
GO
/****** Object:  StoredProcedure [posts].[ScheduleResultForPost]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [posts].[ScheduleResultForPost]
(
	@PostId bigint,
	@ScheduledDate datetime = null
)
As
Begin

	--Set @ScheduledDate = DATEADD(Minute, 1, GETDATE())

	if (@ScheduledDate is not null)
	Begin
		Set @ScheduledDate = CONVERT(datetime, SWITCHOFFSET(CONVERT(datetimeoffset, @ScheduledDate), DATENAME(TzOffset, SYSDATETIMEOFFSET()))) 
	End
	else
	Begin

		Select
			@ScheduledDate = a.EndDate
		From
			MST_Posts_Active a
		Where
			a.Id = @PostId

	End

	Declare @ActiveStartDate int, @ActiveStartTime int

	Select
		@ActiveStartDate = Cast(DATEPART(YEAR, @ScheduledDate) as varchar(4))
			+ right('0' + Cast(DATEPART(MM, @ScheduledDate) as varchar(2)),2)
			+ right('0' + Cast(DATEPART(DAY, @ScheduledDate) as varchar(2)),2)

	Select
		@ActiveStartTime = right('0' + Cast(DATEPART(hh, @ScheduledDate) as varchar(2)),2)
			+ right('0' + Cast(DATEPART(MINUTE, @ScheduledDate) as varchar(2)),2)
			+ right('0' + Cast(DATEPART(SECOND, @ScheduledDate) as varchar(2)),2)

	--Select 
	--	@ActiveStartDate, @ActiveStartTime

	EXEC msdb.dbo.sp_add_jobschedule
	@job_name = 'CalculatePostResults',
	@name = @PostId,
	@enabled = 1,
	@freq_type = 1,
	@active_start_date = @ActiveStartDate,
	@active_end_date = 99991231,
	@active_start_time = @ActiveStartTime,
	@active_end_time = 235959

End

GO
/****** Object:  StoredProcedure [posts].[Vote]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [posts].[Vote]
(
	@PostId bigint,
	@UserIdf uniqueidentifier,
	@TerritoryId int,
	@voteOption	bit
)
As
Begin

	if exists (		
					select
						1
					from
						posts.LivePosts a
					Left Join
						dbo.DET_User_Block b
					On
						a.UserIdf = b.UserIdf and b.BlockUserIdf = @UserIdf 
					Where a.Id = @PostId and b.Id is null and (a.ContactUserIdf = @UserIdf or (a.isPublic = 1 and (a.isGlobal = 1 or a.TerritoryId = @TerritoryId))) and a.isDone = 0
				)
		and not exists (select 1 from dbo.DET_Post_Votes a Where a.PostId = @PostId and a.UserIdf = @UserIdf)
	Begin

		Insert into 
			dbo.DET_Post_Votes (PostId, UserIdf, VoteOption)
		Values
			(@PostId, @UserIdf, @voteOption)

		if (@voteOption = 0)
		Begin
			Update a
			Set
				a.VoteCount1 = a.VoteCount1 + 1
			From
				dbo.MST_Posts_Active a
			Where
				a.Id = @PostId
		End
		Else
		Begin
			Update a
			Set
				a.VoteCount2 = a.VoteCount2 + 1
			From
				dbo.MST_Posts_Active a
			Where
				a.Id = @PostId
		End

	End
	else
	Begin

		Raiserror('504', 16, 1)

	End

End
GO
/****** Object:  StoredProcedure [resource].[AddFileByParentId]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [resource].[AddFileByParentId]
(
	@ResourceId uniqueidentifier,
	@UserHierarchyId hierarchyid,
	@FileExtension varchar(5),
	@FileData varbinary(max)
)
As
Begin

	Insert into dbo.MST_Resources
		(stream_id, name, path_locator, file_stream)
	Select
		@ResourceId, CONCAT(@ResourceId, @FileExtension), dbo.AppendParentPath(@UserHierarchyId), @FileData

End


GO
/****** Object:  StoredProcedure [resource].[DeleteResource]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [resource].[DeleteResource]
(
	@ResourceId uniqueidentifier
)
As
Begin
	
	Delete a
	From
		MST_Resources a
	Where
		a.stream_id = @ResourceId

End
GO
/****** Object:  StoredProcedure [territory].[GetTerritorByUserIdf]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [territory].[GetTerritorByUserIdf]
(
	@UserIdf uniqueidentifier
)
As
Begin

	Select
		top 1 a.TerritoryId, b.CountryCode
	From
		MST_User a
	Inner Join
		MST_Territory b
	On
		a.TerritoryId = b.Id
	Where
		a.Idf = @UserIdf and a.isActive = 1


End
GO
/****** Object:  StoredProcedure [territory].[GetTerritoryList]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [territory].[GetTerritoryList]
As
Begin

	Select
		a.Id, a.Territory, a.CountryCode, a.CreatedOn, a.ModifiedOn
	From
		MST_Territory a


End
GO
/****** Object:  StoredProcedure [test].[PurgeUsersAndFiles]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [test].[PurgeUsersAndFiles]
As
Begin

	Delete a
	From
		DET_Post_SelectedContacts a

	Delete a
	From
		DET_User_Contact a

	Delete a
	From
		MST_Posts_Active a

	Delete a
	From
		MST_Resources a
	Where
		a.parent_path_locator is not null

	Delete a
	From
		MST_Resources a
	Where
		a.is_directory = 1
	
	Delete a
	From
		MST_User a


End
GO
/****** Object:  StoredProcedure [user].[AddResource]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [user].[AddResource]
(
	@ResourceId uniqueidentifier,
	@FileName varchar(100),
	@Data varbinary(max)
)
As
Begin

	Insert into 
		MST_Resources (ResourceId, [FileName], Data) 
	Values
		(@ResourceId, @FileName, @Data)

End
GO
/****** Object:  StoredProcedure [user].[AddUserInfo]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[AddUserInfo]
(
	@UserName varchar(100),
	@DisplayName varchar(150),
	@OwnNumber varchar(15),
	@GenderId int,
	@AgeRangeId int,
	@Status varchar(100),
	@ImageIdf uniqueidentifier = null,
	@FolderPath hierarchyid,
	@TerritoryId int
)
As
Begin

	Insert into 
		MST_User (UserName, DisplayName, OwnNumber, [Status], ImageIdf, FolderPath, TerritoryId, GenderID, AgeRangeId)
	Output inserted.Idf, inserted.FolderPath
	Values
		(@UserName, @DisplayName, @OwnNumber, @Status, @ImageIdf, @FolderPath, @TerritoryId, @GenderId, @AgeRangeId)

End
GO
/****** Object:  StoredProcedure [user].[BlockUser]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[BlockUser]
(
	@UserIdf uniqueidentifier,
	@BlockUserIdf uniqueidentifier
)
As
Begin

	if not exists (Select 1 from DET_User_Block a where a.UserIdf = @BlockUserIdf and a.BlockUserIdf = @UserIdf )
	Begin

		Delete a
		From
			DET_User_Follow a
		Where
			(a.UserIdf = @UserIdf and a.FollowUserIdf = @BlockUserIdf) or (a.UserIdf = @BlockUserIdf and a.FollowUserIdf = @UserIdf)

		Update a
		Set
			a.isAllowed = 0
		From
			DET_User_Contact a
		Where
			a.UserIdf = @UserIdf and a.ContactUserIdf = @BlockUserIdf

		if @@ROWCOUNT = 0
		Begin

			Insert into 
				DET_User_Block (UserIdf, BlockUserIdf)
			Values
				(@UserIdf, @BlockUserIdf)

		End 

	End
	else
	Begin
		
		Raiserror('106', 16, 1)

	End

	





End
GO
/****** Object:  StoredProcedure [user].[ChangePicture]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[ChangePicture]
(
	@UserIdf uniqueidentifier,
	@ResoureId uniqueidentifier = null
)
As
Begin

	Update a
	Set
		a.ImageIdf = @ResoureId,
		a.LastLoggedIn = GETUTCDATE(),
		a.ModifiedOn = GETUTCDATE()
	From
		MST_User a
	Where
		a.Idf = @UserIdf and a.isActive = 1

End
GO
/****** Object:  StoredProcedure [user].[ChangeViewedNotifications]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[ChangeViewedNotifications]
(
	@UserIdf uniqueidentifier,
	@NotificationIds nvarchar(max)
)
As
Begin

	Declare @tmp table (Id bigint)
	Insert into @tmp
	Select
		*
	From
		dbo.SplitString(@NotificationIds, ',')

	Update a
	Set
		a.isViewed = 1
	From
		DET_User_Notifications a	
	Where
		a.Id in (Select Id from @tmp) and a.NotifyUserIdf = @UserIdf and a.isViewed = 0
End
GO
/****** Object:  StoredProcedure [user].[CheckUserByNumber]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[CheckUserByNumber]
(
	@OwnNumber varchar(10)
)
As
Begin

	if exists(select 1 from MST_User a Where a.OwnNumber = @OwnNumber)
	Begin
		Select Cast(1 as bit) as Result
		return
	End

	Select Cast(0 as bit) as Result

End
GO
/****** Object:  StoredProcedure [user].[CreateDirectoryForUser]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [user].[CreateDirectoryForUser]
As
Begin

	DECLARE @FirstSeqNum sql_variant,
            @LastSeqNum  sql_variant
 
     EXEC sys.sp_sequence_get_range
     @sequence_name = N'dbo.NewID'
   , @range_size = 1
   , @range_first_value = @FirstSeqNum OUTPUT
   , @range_last_value = @LastSeqNum OUTPUT

	Declare @MainFolderId HIERARCHYID
	Declare @SubDirectoryPath varchar(max)

	SELECT @SubDirectoryPath = CONCAT('/', CONVERT(VARCHAR(20),@FirstSeqNum) ,'.',
    CONVERT(VARCHAR(20),Convert(BIGINT,@FirstSeqNum)) ,'.',
    CONVERT(VARCHAR(20),@LastSeqNum) ,'/')
	
	INSERT INTO dbo.MST_Resources(name,path_locator,is_directory,is_archive)
	output inserted.path_locator
	VALUES (Cast(@FirstSeqNum as nvarchar(max)), @SubDirectoryPath, 1, 0)

End
GO
/****** Object:  StoredProcedure [user].[DeleteAccount]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[DeleteAccount]
(
	@UserIdf uniqueidentifier
)
As
Begin

	Update a
	Set
		a.isActive = 0
	From
		MST_User a
	Where
		a.Idf = @UserIdf and a.isActive = 1

End
GO
/****** Object:  StoredProcedure [user].[DeleteDirectoryForUser]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  StoredProcedure [user].[CreateDirectoryForUser]    Script Date: 28/09/2015 21:46:56 ******/

CREATE Procedure [user].[DeleteDirectoryForUser]
(
	@PathLocator hierarchyId
)
As
Begin

	Delete a 
	From
		MST_Resources a
	Where
		a.path_locator = @PathLocator


End
GO
/****** Object:  StoredProcedure [user].[FollowUser]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[FollowUser]
(
	@UserIdf uniqueidentifier,
	@FollowUserIdf uniqueidentifier,
	@TerritoryId int
)
As
Begin


	if not exists (select 1 from [user].[ContactList] a where (a.Idf = @FollowUserIdf and a.ContactUserIdf = @UserIdf and a.isAvailable = 1)
									 or (a.Idf = @UserIdf and a.ContactUserIdf = @FollowUserIdf and a.isAvailable = 1))
		and not exists (select 1 from DET_User_Block a where a.UserIdf = @FollowUserIdf and a.BlockUserIdf = @UserIdf)
		and not exists (select 1 from DET_User_Follow a where a.UserIdf = @UserIdf and a.FollowUserIdf = @FollowUserIdf)
	Begin
		Insert into 
			DET_User_Follow(UserIdf, FollowUserIdf, TerritoryId)
		Values
			(@UserIdf, @FollowUserIdf, @TerritoryId)

		Declare @ImageIdf uniqueidentifier, @FolderPath hierarchyid, @DisplayName nvarchar(100) 

		Select
			@ImageIdf = a.ImageIdf, @FolderPath = a.FolderPath, @DisplayName = a.DisplayName
		From
			MST_User a
		Where
			a.Idf = @UserIdf

		Insert into
			DET_User_Notifications (UserIdf, ImageIdf, FolderPath, NotifyUserIdf, Title, DisplayName, isPost, isFollow, isContact)
		output 
			inserted.Id, inserted.UserIdf, inserted.ImageIdf, inserted.FolderPath, inserted.NotifyUserIdf, inserted.Title, inserted.DisplayName, inserted.CreatedOn
		Values
			(@UserIdf, @ImageIdf, @FolderPath, @FollowUserIdf, 'is following you on HeyVote', @DisplayName, 0, 1, 0)
			
	End
	else
	Begin

		Raiserror('108', 16, 1)

	End

End
GO
/****** Object:  StoredProcedure [user].[GetBasicProfileData]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[GetBasicProfileData]
(
	@UserIdf uniqueidentifier
)
As
Begin

	Select
		a.ImageIdf, a.DisplayName, a.UserName, a.LastLoggedIn, a.FolderPath, a.[Status], a.AgeRangeId, c.AgeRanges, a.GenderID, b.Value as Gender
	From
		dbo.MST_User a
	Left Join
		dbo.MST_Gender b
	On
		a.GenderID = b.Id
	Left Join
		dbo.MST_Age_Ranges c
	On
		a.AgeRangeId = c.Id
	Where
		a.Idf = @UserIdf and a.isActive = 1

	
End
GO
/****** Object:  StoredProcedure [user].[GetNotifications]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[GetNotifications]
(
	@UserIdf uniqueidentifier = '252747ae-012e-4130-827b-e2b9d8fd7520',
	@PageId int = 0,
	@PageSize int = 7
)
As
Begin

	Declare @LastLoggedIn datetime

	Declare @Offset int = @PageSize * @PageId

	Select
		a.Id, a.PostId, a.UserIdf, a.ImageIdf, a.FolderPath, a.Title, a.Image1Idf, a.Image2Idf, a.Caption1, a.Caption2, a.EndDate, a.isDone
		, a.Vote1Result, a.Vote2Result, a.PostCreatedOn, a.DisplayName, a.CreatedOn
		, a.hasRead, a.isViewed, a.isPost, a.isFollow, a.isContact
	From
		DET_User_Notifications a
	Left Join
		DET_User_Contact b
	On
		a.UserIdf = b.UserIdf and a.NotifyUserIdf = b.ContactUserIdf
	Where
		a.NotifyUserIdf = @UserIdf and (b.isAllowed is null or b.isAllowed = 1)
	Order By
		a.CreatedOn desc
	Offset @Offset rows
	Fetch next @PageSize rows Only

End
GO
/****** Object:  StoredProcedure [user].[GetPicture]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[GetPicture]
(
	@UserIdf uniqueidentifier
)
As
Begin

	Select
		a.ImageIdf
	From
		MST_User a
	Where
		a.Idf = @UserIdf and a.isActive = 1

End
GO
/****** Object:  StoredProcedure [user].[GetUsersToNotifyByPost]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[GetUsersToNotifyByPost]
(
	@PostIds nvarchar(max) = '10184'
)
As
Begin

	Declare @tmp table (Id bigint)
	Insert into @tmp
	Select
		*
	From
		dbo.SplitString(@PostIds, ',')

	Insert into
		DET_User_Notifications (PostId, UserIdf, ImageIdf, FolderPath, NotifyUserIdf
		, Title, PostCreatedOn, isDone, Vote1Result, Vote2Result, Image1Idf, Image2Idf, DisplayName
		, Caption1, Caption2, EndDate, isPost, isFollow, isContact)
	Output
		inserted.Id, inserted.PostId, inserted.UserIdf, inserted.ImageIdf, inserted.FolderPath, inserted.NotifyUserIdf, inserted.Title, inserted.CreatedOn, inserted.isDone
		, inserted.Vote1Result, inserted.Vote2Result, inserted.Image1Idf, inserted.Image2Idf, inserted.DisplayName, inserted.Caption1, inserted.Caption2, inserted.EndDate
		, inserted.isPost, inserted.isFollow, inserted.isContact
	Select
		a.Id, a.UserIdf, a.ImageIdf, a.FolderPath, a.NotifyUserIdf, a.Title, a.CreatedOn,
		a.isDone, a.Vote1Result, a.Vote2Result, a.Img1Idf, a.Img2Idf, a.DisplayName, a.Caption1, a.Caption2, a.EndDate
		, 1, 0, 0
	From
	(
		Select
			a.Id, a.UserIdf, d.ImageIdf, d.FolderPath, case when  (a.toContacts = 1 or a.isPublic = 1) then b.ContactUserIdf else c.ContactUserIdf end as NotifyUserIdf
			, a.Title, a.CreatedOn, a.isDone, a.Vote1Result, a.Vote2Result, a.Img1Idf, a.Img2Idf, d.DisplayName, a.Caption1, a.Caption2, a.EndDate
		From
			MST_Posts_Active a
		Inner Join
			dbo.MST_User d
		On
			a.UserIdf = d.Idf
		Left Join
			[user].ContactList b
		On
			(a.toContacts = 1 or a.isPublic = 1) and b.isAvailable = 1 and a.UserIdf = b.Idf and (a.isGlobal = 1 or a.TerritoryId = b.TerritoryId)
		Left Join
			dbo.DET_Post_SelectedContacts c
		On
			a.toSelectedContacts = 1 and a.Id = c.PostId
		Left Join
			[user].ContactList e
		On
			a.toSelectedContacts = 1 and e.isAvailable = 1 and c.ContactUserIdf = e.Idf and (a.isGlobal = 1 or e.TerritoryId = a.TerritoryId)
		Where
			d.isActive = 1 and a.Id in (Select Id from @tmp) and (((a.toContacts = 1 or a.isPublic = 1) and b.ContactUserIdf is not null) 
			or (a.toSelectedContacts = 1 and c.ContactUserIdf is not null)) -- and a.isDone = 1

		union

		Select
			a.Id, a.UserIdf, d.ImageIdf, d.FolderPath, b.UserIdf as NotifyUserIdf
			, a.Title, a.CreatedOn, a.isDone, a.Vote1Result, a.Vote2Result, a.Img1Idf, a.Img2Idf, d.DisplayName, a.Caption1, a.Caption2, a.EndDate
		From
			MST_Posts_Active a
		Inner Join
			dbo.MST_User d
		On
			a.UserIdf = d.Idf
		Inner Join
			dbo.DET_User_Follow b
		On
			a.isPublic = 1 and (a.isGlobal = 1 or a.TerritoryId = b.TerritoryId) and b.FollowUserIdf = a.UserIdf
	) a
	Left Join
		dbo.DET_User_Block b
	On
		a.UserIdf = b.UserIdf and a.NotifyUserIdf = b.BlockUserIdf
	Where
		b.Id is null

End
GO
/****** Object:  StoredProcedure [user].[SubscribePost]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[SubscribePost]
(
	@UserIdf uniqueidentifier,
	@PostId bigint
)
As
Begin

	Insert into 
		DET_Post_Subscribers (UserIdf, PostId)
	Values
		(@UserIdf, @PostId)

End
GO
/****** Object:  StoredProcedure [user].[UnBlockUser]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[UnBlockUser]
(
	@UserIdf uniqueidentifier,
	@BlockUserIdf uniqueidentifier
)
As
Begin

	Update a
	Set
		a.isAllowed = 1
	From
		DET_User_Contact a
	Where
		a.UserIdf = @UserIdf and a.ContactUserIdf = @BlockUserIdf

	if @@ROWCOUNT <> 0
	Begin

		Delete a
		From
			DET_User_Block a
		Where
			a.UserIdf = @UserIdf and a.BlockUserIdf = @BlockUserIdf

	End


End
GO
/****** Object:  StoredProcedure [user].[UnFollowUser]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[UnFollowUser]
(
	@UserIdf uniqueidentifier,
	@FollowUserIdf uniqueidentifier
)
As
Begin

	Delete a
	From
		DET_User_Follow a
	Where
		a.UserIdf = @UserIdf and a.FollowUserIdf = @FollowUserIdf

End
GO
/****** Object:  StoredProcedure [user].[UnSubscribePost]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[UnSubscribePost]
(
	@UserIdf uniqueidentifier,
	@PostId bigint
)
As
Begin

	Delete a
	From
		DET_Post_Subscribers a
	Where
		a.UserIdf = @UserIdf and a.PostId = @PostId

End
GO
/****** Object:  StoredProcedure [user].[UpdateLastLoggedIntime]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[UpdateLastLoggedIntime]
(
	@UserIdf uniqueidentifier
)
As
Begin

	Update a
	Set
		a.LastLoggedIn = GETUTCDATE()
	From
		MST_User a
	Where
		a.Idf = @UserIdf and a.isActive = 1

End
GO
/****** Object:  StoredProcedure [user].[UpdateName]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[UpdateName]
(
	@UserIdf uniqueidentifier,
	@Name varchar(100)
)
As
Begin

	Update a
	Set
		a.DisplayName = @Name,
		a.ModifiedOn = GETUTCDATE() 
	From
		MST_User a
	Where
		a.Idf = @UserIdf and a.isActive = 1

End
GO
/****** Object:  StoredProcedure [user].[UpdateStatus]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [user].[UpdateStatus]
(
	@UserIdf uniqueidentifier,
	@Status varchar(100)
)
As
Begin

	Update a
	Set
		a.Status = @Status,
		a.ModifiedOn = GETUTCDATE() 
	From
		MST_User a
	Where
		a.Idf = @UserIdf and a.isActive = 1

End
GO
/****** Object:  StoredProcedure [user].[ViewProfile]    Script Date: 29/11/2015 22:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [user].[ViewProfile]
(
	@UserIdf uniqueidentifier = '57da548e-04be-4643-abed-8d9d256b4c88',
	@ContactUserIdf uniqueidentifier = '252747ae-012e-4130-827b-e2b9d8fd7520',
	@TerritoryId int = 1

)
As
Begin

	Insert into
		DET_User_Views(UserIdf, ViewingUserIdf, TerritoryId) 
	Values
		(@ContactUserIdf, @UserIdf, @TerritoryId)

	Select
		b.Idf, b.ImageIdf, b.FolderPath, b.DisplayName, b.Status
		, isnull(c.FollowerCount, 0) as FollowersCount
		, isnull(d.FollowingCount, 0) as FollowingCount
		, isnull(e.TerritoryViewCount, 0) as TerritoryViewCount
		, isnull(f.GlobalViewCount, 0) as GlobalViewCount
		, isnull(i.HeyVotesCount, 0) as HeyVotesCount
		, g.isAllowed as isAllowed, j.URank
		, Cast(Case when h.Id is null then 0 else 1 end as bit) as isFollowing 
	From
		DET_User_Contact a
	Inner Join
		MST_User b
	On
		a.UserIdf = b.Idf
	Inner Join
		DET_User_Contact g
	On
		a.UserIdf = g.ContactUserIdf and g.UserIdf = @UserIdf
	Left Join
	(
		Select
			c.FollowUserIdf, COUNT(*) as FollowerCount
		From
			DET_User_Follow c
		Where
			c.FollowUserIdf = @ContactUserIdf
		Group By	
			c.FollowUserIdf
	) c
	On
		a.UserIdf = c.FollowUserIdf
	Left Join
	(
		Select
			c.UserIdf, COUNT(*) as FollowingCount
		From
			DET_User_Follow c
		Where
			c.UserIdf = @ContactUserIdf
		Group By	
			c.UserIdf
	) d
	On
		a.UserIdf = d.UserIdf
	Left Join
	(
		Select
			e.UserIdf, e.TerritoryId, COUNT(*) as TerritoryViewCount
		From
			DET_User_Views e
		Where
			e.UserIdf = @ContactUserIdf and e.TerritoryId = @TerritoryId
		Group By
			e.UserIdf, e.TerritoryId
	) e
	On
		a.UserIdf = e.UserIdf and b.TerritoryId = e.TerritoryId
	Left Join
	(
		Select
			e.UserIdf, COUNT(*) as GlobalViewCount
		From
			DET_User_Views e
		Where
			e.UserIdf = @ContactUserIdf
		Group By
			e.UserIdf
	) f
	On
		a.UserIdf = f.UserIdf
	Left Join
	(
		Select	
			a.UserIdf, COUNT(a.Id) as HeyVotesCount
		From
			MST_Posts_Active a
		Where
			a.UserIdf = @ContactUserIdf
		Group By
			a.UserIdf
	) i
	On
		a.UserIdf = i.UserIdf
	Left Join
		DET_User_Follow h
	On
		a.UserIdf = h.FollowUserIdf and h.UserIdf = @UserIdf
	Left Join
		UserRank j
	On
		a.UserIdf = j.Idf
	Where
		a.UserIdf = @ContactUserIdf and a.ContactUserIdf = @UserIdf and a.isAllowed = 1

End
GO

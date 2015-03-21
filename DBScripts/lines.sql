USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[lines]    Script Date: 03/20/2015 17:58:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[lines](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[id] [int] NOT NULL,
	[description] [nvarchar](max) NULL,
	[stations] [int] NULL
) ON [PRIMARY]

GO



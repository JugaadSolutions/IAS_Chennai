USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[class]    Script Date: 03/20/2015 17:56:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[class](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[id] [int] NOT NULL,
	[line] [int] NOT NULL,
	[station] [int] NOT NULL,
	[department] [int] NOT NULL,
	[description] [nvarchar](max) NULL
) ON [PRIMARY]

GO



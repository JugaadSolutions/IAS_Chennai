USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[issues]    Script Date: 03/20/2015 17:58:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[issues](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[line] [int] NULL,
	[station] [int] NOT NULL,
	[department] [int] NOT NULL,
	[data] [nvarchar](max) NULL,
	[status] [nvarchar](50) NULL,
	[timestamp] [datetime] NULL,
	[message] [nvarchar](max) NULL
) ON [PRIMARY]

GO



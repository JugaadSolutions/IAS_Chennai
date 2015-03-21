USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[issue_tracker]    Script Date: 03/20/2015 17:58:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[issue_tracker](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[issue] [int] NOT NULL,
	[status] [nvarchar](50) NULL,
	[timestamp] [datetime] NULL
) ON [PRIMARY]

GO



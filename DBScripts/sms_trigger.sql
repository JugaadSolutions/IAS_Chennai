USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[sms_trigger]    Script Date: 03/20/2015 18:07:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[sms_trigger](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[message] [nvarchar](max) NULL,
	[receiver] [nvarchar](50) NOT NULL,
	[status] [int] NULL,
	[timestamp] [datetime] NULL,
	[priority] [int] NULL
) ON [PRIMARY]

GO



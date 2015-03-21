USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[sms_receiver]    Script Date: 03/20/2015 18:07:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[sms_receiver](
	[slNo] [int] IDENTITY(1,1) NOT NULL,
	[receiver] [nvarchar](50) NOT NULL
) ON [PRIMARY]

GO



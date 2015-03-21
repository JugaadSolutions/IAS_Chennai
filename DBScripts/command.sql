USE [IAS_Schneider_Chennai]
GO

/****** Object:  Table [dbo].[Command]    Script Date: 03/20/2015 17:56:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Command](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[line_id] [int] NULL,
	[command] [int] NULL,
	[command_data] [varchar](max) NULL,
	[response] [int] NULL,
	[response_data] [varchar](max) NULL,
	[status] [int] NULL,
	[request_date] [date] NULL,
	[response_date] [date] NULL,
 CONSTRAINT [PK_DeviceTransaction] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



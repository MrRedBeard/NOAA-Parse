SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WeatherForecasts](
	[WeatherForecastID] [int] IDENTITY(1,1) NOT NULL,
	[ForecastWhen] [varchar](50) NULL,
	[TemperatureLabel] [varchar](50) NULL,
	[Temperature] [int] NULL,
	[PercPrecip] [int] NULL,
	[WeatherTitle] [varchar](50) NULL,
	[WeatherText] [varchar](500) NULL,
	[Icon] [varchar](50) NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_WeatherForecasts] PRIMARY KEY CLUSTERED 
(
	[WeatherForecastID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WeatherForecasts] ADD  CONSTRAINT [DF_WeatherForecasts_Active]  DEFAULT ((1)) FOR [Active]
GO

CREATE TABLE [dbo].[WeatherObservations](
	[WeatherObservationID] [int] IDENTITY(1,1) NOT NULL,
	[Elevation] [float] NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[Date] [datetime] NULL,
	[Temperature] [int] NULL,
	[TemperatureLabel] [varchar](50) NULL,
	[DewPoint] [int] NULL,
	[RelativeHumidity] [int] NULL,
	[WindSpeed] [int] NULL,
	[WindDirection] [int] NULL,
	[WindGust] [int] NULL,
	[WeatherText] [varchar](500) NULL,
	[Icon] [varchar](50) NULL,
	[Visibility] [float] NULL,
	[Altimeter] [float] NULL,
	[Barometer] [float] NULL,
	[WindChill] [int] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_WeatherObservations] PRIMARY KEY CLUSTERED 
(
	[WeatherObservationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WeatherObservations] ADD  CONSTRAINT [DF_WeatherObservations_Active]  DEFAULT ((1)) FOR [Active]
GO

CREATE TABLE [dbo].[WeatherAlerts](
	[WeatherAlertID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](150) NULL,
	[ExpireDateTime] [datetime] NULL,
	[AlertText] [varchar](8000) NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_WeatherAlerts] PRIMARY KEY CLUSTERED 
(
	[WeatherAlertID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WeatherAlerts] ADD  CONSTRAINT [DF_WeatherAlerts_Active]  DEFAULT ((1)) FOR [Active]
GO


USE [master]
GO
/****** Object:  Database [Weather]    Script Date: 4/15/2022 9:24:42 AM ******/
CREATE DATABASE [Weather]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Weather', FILENAME = N'E:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Weather.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Weather_log', FILENAME = N'E:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Weather_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Weather] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Weather].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Weather] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Weather] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Weather] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Weather] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Weather] SET ARITHABORT OFF 
GO
ALTER DATABASE [Weather] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Weather] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Weather] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Weather] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Weather] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Weather] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Weather] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Weather] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Weather] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Weather] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Weather] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Weather] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Weather] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Weather] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Weather] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Weather] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Weather] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Weather] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Weather] SET  MULTI_USER 
GO
ALTER DATABASE [Weather] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Weather] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Weather] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Weather] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Weather] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Weather] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Weather] SET QUERY_STORE = OFF
GO
USE [Weather]
GO
/****** Object:  Table [dbo].[WeatherAlerts]    Script Date: 4/15/2022 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Table [dbo].[WeatherForecasts]    Script Date: 4/15/2022 9:24:45 AM ******/
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
/****** Object:  Table [dbo].[WeatherObservations]    Script Date: 4/15/2022 9:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
ALTER TABLE [dbo].[WeatherAlerts] ADD  CONSTRAINT [DF_WeatherAlerts_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[WeatherForecasts] ADD  CONSTRAINT [DF_WeatherForecasts_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[WeatherObservations] ADD  CONSTRAINT [DF_WeatherObservations_Active]  DEFAULT ((1)) FOR [Active]
GO
USE [master]
GO
ALTER DATABASE [Weather] SET  READ_WRITE 
GO

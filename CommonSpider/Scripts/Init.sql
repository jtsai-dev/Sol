---- SQLite
--CREATE TABLE "main"."Notice" (
--  "Id" varchar(16) NOT NULL,
--  "Title" varchar(32),
--  "PublishDate" datetime,
--  "Content" text,
--  "Url" varchar(256),
--  PRIMARY KEY ("Id")
--);

-- MySQL
-- SZWater
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS `SZNotice`;
CREATE TABLE `SZNotice`  (
  `Id` varchar(16) NOT NULL,
  `Title` varchar(32),
  `PublishDate` datetime(0),
  `Content` varchar(512),
  `Url` varchar(256),
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;
SET FOREIGN_KEY_CHECKS = 1;

-- DoubanRentSummary
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS `DoubanRentSummary`;
CREATE TABLE `DoubanRentSummary`  (
  `Id` int(8) NOT NULL,
  `Title` varchar(32),
  `Author` varchar(32),
  `Url` varchar(256),
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;
SET FOREIGN_KEY_CHECKS = 1;
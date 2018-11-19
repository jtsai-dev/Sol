-- SQLite
CREATE TABLE "main"."Notice" (
  "Id" varchar(16) NOT NULL,
  "Title" varchar(32),
  "PublishDate" datetime,
  "Content" text,
  "Url" varchar(256),
  PRIMARY KEY ("Id")
);

-- MySQL
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS `notice`;
CREATE TABLE `notice`  (
  `Id` varchar(16) NOT NULL,
  `Title` varchar(32),
  `PublishDate` datetime(0),
  `Content` varchar(512),
  `Url` varchar(256),
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
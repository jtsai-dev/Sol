CREATE TABLE "main"."Notice" (
  "Id" varchar(16) NOT NULL,
  "Title" varchar(32),
  "PublishDate" datetime,
  "Content" text,
  "Url" varchar(256),
  PRIMARY KEY ("Id")
);
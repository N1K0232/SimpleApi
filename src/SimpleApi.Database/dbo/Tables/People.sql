CREATE TABLE [dbo].[People] (
    [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
    [FirstName] NVARCHAR (100)   NOT NULL,
    [LastName]  NVARCHAR (100)   NOT NULL,

    PRIMARY KEY (Id)
);


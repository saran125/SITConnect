CREATE TABLE [dbo].[Users] (
    [Id]                     CHAR (100)    NOT NULL,
    [Email]                  CHAR (100)     NULL,
    [Fname]                  CHAR (100)     NULL,
    [Lname]                  CHAR (100)     NULL,
    [Password]               CHAR (100)     NULL,
    [Card_number]            CHAR (100)     NULL,
    [CVV]                    CHAR (100)     NULL,
    [Expiry_date]            DATETIME2 (7) NULL,
    [Key]                    CHAR (100)     NULL,
    [IV]                     CHAR (100)     NULL,
    [Role]                   CHAR (100)     NULL,
    [BirthDate]              DATETIME2 (7) NULL,
    [Photo]                  CHAR (100)     NULL,
    [Verify]                 CHAR (100)     NULL,
    [LockoutEnd]             DATETIME2 (7) NULL,
    [AccessFailedCount]      INT           NULL,
    [LockedoutEnabled]       CHAR (100)     NULL,
    [Password_Changed_Time ] DATETIME2 (7) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



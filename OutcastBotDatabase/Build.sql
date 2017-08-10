/*
The database must have a MEMORY_OPTIMIZED_DATA filegroup
before the memory optimized object can be created.

The bucket count should be set to about two times the 
maximum expected number of distinct values in the 
index key, rounded up to the nearest power of two.
*/

CREATE TABLE [dbo].[Build]
(
	[Id] INT NOT NULL PRIMARY KEY NONCLUSTERED HASH WITH (BUCKET_COUNT = 131072), 
    [AuthorId] BIGINT NOT NULL, 
    [UpVotes] INT NOT NULL, 
    [DownVotes] INT NOT NULL, 
    [MessageId] BIGINT NOT NULL, 
    [BuildUrl] NCHAR(50) NOT NULL, 
    [PatchVersion] NCHAR(7) NOT NULL, 
    [TItle] NVARCHAR(100) NOT NULL, 
    [Description] NVARCHAR(1000) NOT NULL, 
    [HeaderImageUrl] NVARCHAR(100) NULL, 
    [ForumUrl] NVARCHAR(100) NULL, 
    [VideoUrl] NVARCHAR(100) NULL, 
    [Tags] NVARCHAR(100) NULL
) WITH (MEMORY_OPTIMIZED = ON)
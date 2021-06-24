CREATE TRIGGER dbo.Habits_ADD
ON dbo.Habits
AFTER INSERT
AS 
BEGIN
    DECLARE @id as int;
    SET @id = (SELECT Id from INSERTED);
    INSERT INTO Datelog (HabitId) VALUES (@id);
END
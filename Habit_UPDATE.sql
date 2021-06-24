CREATE TRIGGER dbo.Habits_UPDATE
ON dbo.Habits
AFTER UPDATE
AS 
BEGIN
    DECLARE @percentage as float;
    SET @percentage = (SELECT Percentage from INSERTED);
    UPDATE Datelog SET Percentage = @percentage, IfChecked = 1 where HabitId = (SELECT ID from INSERTED);
END
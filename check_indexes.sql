-- Query to check all indexes in the database
SELECT 
    type as "Type",
    name as "Index Name",
    tbl_name as "Table Name"
FROM 
    sqlite_master
WHERE 
    type = 'index';

-- To see how the indexes are used in a specific query, we can use the EXPLAIN QUERY PLAN command
-- This will show the execution plan for the query that gets the latest games

EXPLAIN QUERY PLAN 
SELECT *
FROM GameStats
ORDER BY GameDate DESC
LIMIT 10;

-- Check execution plan for a query that uses the Team/Opponent index
EXPLAIN QUERY PLAN 
SELECT *
FROM GameStats
WHERE Team = 'Lakers' AND Opponent = 'Celtics';

-- Check execution plan for a query that uses the Player index
EXPLAIN QUERY PLAN 
SELECT *
FROM GameStats
WHERE Player = 'LeBron James';

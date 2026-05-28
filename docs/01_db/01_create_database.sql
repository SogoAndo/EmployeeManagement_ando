SELECT 'CREATE DATABASE employee_management'
WHERE NOT EXISTS (
    SELECT 1
    FROM pg_database
    WHERE datname = 'employee_management'
)\gexec

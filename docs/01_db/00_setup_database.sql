\set ON_ERROR_STOP on

\echo 'Create database if needed.'
\ir 01_create_database.sql

\connect employee_management

\echo 'Create tables.'
\ir 02_create_tables.sql

\echo 'Insert initial data.'
\ir 03_insert_initial_data.sql

\echo 'Database setup completed.'

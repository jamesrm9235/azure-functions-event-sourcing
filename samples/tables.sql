CREATE TABLE IF NOT EXISTS todos (
    id uuid,
    description varchar,
    progress integer,
    created_at timestamp,
    completed_at timestamp
);
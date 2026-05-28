BEGIN;

SET CONSTRAINTS ALL DEFERRED;

TRUNCATE TABLE users, employees, departments RESTART IDENTITY CASCADE;

INSERT INTO departments (
    id,
    name,
    created_by_user_id,
    created_at,
    updated_at,
    updated_by_user_id
) VALUES
    (1, '人事部', 1, '2026-05-22 16:53:16', NULL, NULL),
    (2, '経営管理部', 1, '2026-05-22 16:53:16', NULL, NULL),
    (3, '開発部', 1, '2026-05-22 16:53:16', NULL, NULL),
    (4, '総務部', 1, '2026-05-25 11:45:09', NULL, NULL),
    (5, '品質管理部', 1, '2026-05-25 15:05:57', NULL, NULL),
    (6, '営業部', 1, '2026-05-25 16:26:48', NULL, NULL);

INSERT INTO employees (
    id,
    employee_no,
    name,
    email,
    hire_date,
    department_id,
    created_by_user_id,
    created_at,
    updated_at,
    updated_by_user_id
) VALUES
    (1, '1001', '山田太郎', 'yamada.taro@example.com', '2020-04-01', 1, 1, '2026-05-22 16:53:16', NULL, NULL),
    (2, '1002', '鈴木花子', 'suzuki.hanako@example.com', '2021-04-01', 2, 1, '2026-05-22 16:53:16', NULL, NULL),
    (3, '1003', '高橋一郎', 'takahashi.ichiro@example.com', '2022-04-01', 3, 1, '2026-05-22 16:53:16', NULL, NULL),
    (4, '1004', '田中美咲', 'tanaka.misaki@example.com', '2023-04-01', 1, 1, '2026-05-22 16:53:16', NULL, NULL),
    (5, '1005', '伊藤健太', 'ito.kenta@example.com', '2026-04-01', 1, 1, '2026-05-25 16:28:04', NULL, NULL),
    (6, '1006', '渡辺直子', 'watanabe.naoko@example.com', '2026-05-27', 1, 1, '2026-05-27 16:05:46', NULL, NULL);

INSERT INTO users (
    id,
    login_id,
    password,
    employee_id
) VALUES
    (1, '1001', 'AQAAAAIAAYagAAAAEDwB7blxA1SH4R8/r30hZzpVGhugrTJD9FQ4yKaPHimR1cbg7rTlpgrkN+qngwoMtA==', 1),
    (2, '1004', 'AQAAAAIAAYagAAAAEFjh0UXDnNPY8dNF9qqQ2oyeIwRcU5Cz+TFV9fYz+lKtmJsQvemvJDIqMesKzm763Q==', 4),
    (3, '1005', 'AQAAAAIAAYagAAAAEGLgEZIWNoyn8XZ8viIb33rWNywglMJc4KBMHrId+/lXweqW97w7g+rhS81NMbdjog==', 5),
    (4, '1006', 'AQAAAAIAAYagAAAAEJgJMapZdH7UUFOWv0d+CVs+opbvoB1Sruh3yfGUTb5GOQb62HOfZ0SxPCRbgn0dAQ==', 6);

SELECT setval(pg_get_serial_sequence('departments', 'id'), (SELECT max(id) FROM departments));
SELECT setval(pg_get_serial_sequence('employees', 'id'), (SELECT max(id) FROM employees));
SELECT setval(pg_get_serial_sequence('users', 'id'), (SELECT max(id) FROM users));

COMMIT;

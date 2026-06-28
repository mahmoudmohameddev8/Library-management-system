-- ============================================================
--  Library Management System — Sample Data (MySQL)
--  Run this AFTER the application has created the schema.
--  The application seeds this data automatically on first run.
--  This script is provided for reference and manual reseeding.
-- ============================================================

USE LibraryManagementDb;

-- ─── Roles ───────────────────────────────────────────────────
INSERT INTO `Roles` (`Id`, `Name`, `Description`, `CreatedAt`, `CreatedBy`) VALUES
('00000000-0000-0000-0000-000000000001', 'Admin',     'Full system access with all administrative privileges',  NOW(), 'Seed'),
('00000000-0000-0000-0000-000000000002', 'Librarian', 'Manages books, members, and borrowing operations',       NOW(), 'Seed'),
('00000000-0000-0000-0000-000000000003', 'Staff',     'Handles day-to-day borrowing and returning operations',  NOW(), 'Seed');

-- ─── Users ───────────────────────────────────────────────────
-- Passwords: Admin@123! / Librarian@123! / Staff@123!  (BCrypt hashed)
INSERT INTO `Users` (`Id`, `Username`, `Email`, `PasswordHash`, `FirstName`, `LastName`, `IsActive`, `RoleId`, `CreatedAt`, `CreatedBy`) VALUES
('60000000-0000-0000-0000-000000000001', 'admin',     'admin@library.com',     '$2a$12$examplehashforadmin000000000000000000000000000000000', 'System', 'Administrator', 1, '00000000-0000-0000-0000-000000000001', NOW(), 'Seed'),
('60000000-0000-0000-0000-000000000002', 'librarian', 'librarian@library.com', '$2a$12$examplehashforlibrarian00000000000000000000000000000', 'Jane',   'Smith',         1, '00000000-0000-0000-0000-000000000002', NOW(), 'Seed'),
('60000000-0000-0000-0000-000000000003', 'staff',     'staff@library.com',     '$2a$12$examplehashforstaff000000000000000000000000000000000', 'John',   'Doe',           1, '00000000-0000-0000-0000-000000000003', NOW(), 'Seed');

-- NOTE: The hashes above are placeholders.
-- The application generates real BCrypt hashes at runtime.
-- To get real hashes, run the app first then query: SELECT Username, PasswordHash FROM Users;

-- ─── Languages ───────────────────────────────────────────────
INSERT INTO `Languages` (`Id`, `Name`, `Code`, `CreatedAt`, `CreatedBy`) VALUES
('10000000-0000-0000-0000-000000000001', 'English', 'EN', NOW(), 'Seed'),
('10000000-0000-0000-0000-000000000002', 'Arabic',  'AR', NOW(), 'Seed'),
('10000000-0000-0000-0000-000000000003', 'French',  'FR', NOW(), 'Seed'),
('10000000-0000-0000-0000-000000000004', 'Spanish', 'ES', NOW(), 'Seed'),
('10000000-0000-0000-0000-000000000005', 'German',  'DE', NOW(), 'Seed');

-- ─── Publishers ──────────────────────────────────────────────
INSERT INTO `Publishers` (`Id`, `Name`, `Website`, `CreatedAt`, `CreatedBy`) VALUES
('20000000-0000-0000-0000-000000000001', 'Pearson Education',    'https://www.pearson.com',        NOW(), 'Seed'),
('20000000-0000-0000-0000-000000000002', 'O''Reilly Media',      'https://www.oreilly.com',        NOW(), 'Seed'),
('20000000-0000-0000-0000-000000000003', 'Microsoft Press',      'https://www.microsoftpress.com', NOW(), 'Seed'),
('20000000-0000-0000-0000-000000000004', 'Manning Publications', 'https://www.manning.com',        NOW(), 'Seed'),
('20000000-0000-0000-0000-000000000005', 'Addison-Wesley',       'https://www.pearson.com',        NOW(), 'Seed');

-- ─── Categories (hierarchical) ───────────────────────────────
-- Root categories
INSERT INTO `Categories` (`Id`, `Name`, `Description`, `ParentCategoryId`, `CreatedAt`, `CreatedBy`) VALUES
('30000000-0000-0000-0000-000000000001', 'Fiction',     'Fictional literature and stories',      NULL, NOW(), 'Seed'),
('30000000-0000-0000-0000-000000000005', 'Non-Fiction', 'Factual and informational books',       NULL, NOW(), 'Seed'),
('30000000-0000-0000-0000-000000000009', 'Science',     'Scientific books across disciplines',   NULL, NOW(), 'Seed');

-- Fiction children
INSERT INTO `Categories` (`Id`, `Name`, `Description`, `ParentCategoryId`, `CreatedAt`, `CreatedBy`) VALUES
('30000000-0000-0000-0000-000000000002', 'Mystery',          'Detective and mystery novels',         '30000000-0000-0000-0000-000000000001', NOW(), 'Seed'),
('30000000-0000-0000-0000-000000000003', 'Thriller',         'Suspense and thriller novels',         '30000000-0000-0000-0000-000000000001', NOW(), 'Seed'),
('30000000-0000-0000-0000-000000000004', 'Science Fiction',  'Futuristic and speculative fiction',   '30000000-0000-0000-0000-000000000001', NOW(), 'Seed');

-- Non-Fiction children
INSERT INTO `Categories` (`Id`, `Name`, `Description`, `ParentCategoryId`, `CreatedAt`, `CreatedBy`) VALUES
('30000000-0000-0000-0000-000000000006', 'Technology', 'Technology and computing books',            '30000000-0000-0000-0000-000000000005', NOW(), 'Seed');

-- Technology grandchildren
INSERT INTO `Categories` (`Id`, `Name`, `Description`, `ParentCategoryId`, `CreatedAt`, `CreatedBy`) VALUES
('30000000-0000-0000-0000-000000000007', 'Programming',     'Programming languages and paradigms',  '30000000-0000-0000-0000-000000000006', NOW(), 'Seed'),
('30000000-0000-0000-0000-000000000008', 'Web Development', 'Web technologies and frameworks',      '30000000-0000-0000-0000-000000000006', NOW(), 'Seed');

-- Science children
INSERT INTO `Categories` (`Id`, `Name`, `Description`, `ParentCategoryId`, `CreatedAt`, `CreatedBy`) VALUES
('30000000-0000-0000-0000-000000000010', 'Biology', 'Life sciences',                               '30000000-0000-0000-0000-000000000009', NOW(), 'Seed'),
('30000000-0000-0000-0000-000000000011', 'Physics', 'Physical sciences and mechanics',             '30000000-0000-0000-0000-000000000009', NOW(), 'Seed');

-- ─── Authors ─────────────────────────────────────────────────
INSERT INTO `Authors` (`Id`, `FirstName`, `LastName`, `Biography`, `Nationality`, `CreatedAt`, `CreatedBy`) VALUES
('40000000-0000-0000-0000-000000000001', 'Robert', 'Martin',   'The Clean Code author and Agile manifesto contributor.',                                        'Uncle Bob',      NOW(), 'Seed'),
('40000000-0000-0000-0000-000000000002', 'Martin', 'Fowler',   'Author of Refactoring and Patterns of Enterprise Application Architecture.',                    'Martin Fowler',  NOW(), 'Seed'),
('40000000-0000-0000-0000-000000000003', 'Eric',   'Evans',    'Creator of Domain-Driven Design methodology.',                                                  'Eric Evans',     NOW(), 'Seed'),
('40000000-0000-0000-0000-000000000004', 'Andrew', 'Hunt',     'The Pragmatic Programmer co-author.',                                                           'Andy Hunt',      NOW(), 'Seed'),
('40000000-0000-0000-0000-000000000005', 'Agatha', 'Christie', 'Queen of Crime and bestselling mystery novelist.',                                              'Agatha Christie', NOW(), 'Seed');

-- ─── Books ───────────────────────────────────────────────────
INSERT INTO `Books` (`Id`, `ISBN`, `Title`, `Description`, `PublishedDate`, `TotalCopies`, `AvailableCopies`, `Status`, `PageCount`, `PublisherId`, `LanguageId`, `CategoryId`, `CreatedAt`, `CreatedBy`) VALUES
('50000000-0000-0000-0000-000000000001', '9780132350884', 'Clean Code: A Handbook of Agile Software Craftsmanship',
    'A guide to writing clean, readable, and maintainable code.',
    '2008-08-01', 5, 5, 'Available', 431,
    '20000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000007', NOW(), 'Seed'),

('50000000-0000-0000-0000-000000000002', '9780201485677', 'Refactoring: Improving the Design of Existing Code',
    'Systematic techniques for improving the structure of existing code.',
    '1999-06-28', 3, 3, 'Available', 448,
    '20000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000007', NOW(), 'Seed'),

('50000000-0000-0000-0000-000000000003', '9780321125217', 'Domain-Driven Design: Tackling Complexity in the Heart of Software',
    'A methodology for developing complex software by connecting the implementation to an evolving model.',
    '2003-08-20', 4, 4, 'Available', 560,
    '20000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000007', NOW(), 'Seed'),

('50000000-0000-0000-0000-000000000004', '9780135957059', 'The Pragmatic Programmer: Your Journey to Mastery',
    'Practical advice for software developers from code to career.',
    '2019-09-13', 6, 6, 'Available', 352,
    '20000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000007', NOW(), 'Seed'),

('50000000-0000-0000-0000-000000000005', '9780007527526', 'And Then There Were None',
    'Ten people lured to a remote island by a mysterious host, each accused of past crimes.',
    '1939-11-06', 8, 8, 'Available', 272,
    '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000002', NOW(), 'Seed');

-- ─── Book-Author relationships ────────────────────────────────
INSERT INTO `BookAuthors` (`BookId`, `AuthorId`, `IsPrimaryAuthor`) VALUES
('50000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001', 1),
('50000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000002', 1),
('50000000-0000-0000-0000-000000000003', '40000000-0000-0000-0000-000000000003', 1),
('50000000-0000-0000-0000-000000000004', '40000000-0000-0000-0000-000000000004', 1),
('50000000-0000-0000-0000-000000000005', '40000000-0000-0000-0000-000000000005', 1);

-- ─── Members ─────────────────────────────────────────────────
INSERT INTO `Members` (`Id`, `MembershipNumber`, `FirstName`, `LastName`, `Email`, `PhoneNumber`, `Address`, `Status`, `MembershipStartDate`, `MembershipExpiryDate`, `MaxBorrowLimit`, `CreatedAt`, `CreatedBy`) VALUES
(UUID(), 'LIB-000001', 'Alice', 'Johnson', 'alice.johnson@email.com', '+1-555-0101', '123 Main St, Springfield',       'Active', NOW(), DATE_ADD(NOW(), INTERVAL 12 MONTH), 5, NOW(), 'Seed'),
(UUID(), 'LIB-000002', 'Bob',   'Williams','bob.williams@email.com',  '+1-555-0102', '456 Oak Ave, Shelbyville',        'Active', NOW(), DATE_ADD(NOW(), INTERVAL 12 MONTH), 5, NOW(), 'Seed'),
(UUID(), 'LIB-000003', 'Carol', 'Davis',   'carol.davis@email.com',   '+1-555-0103', '789 Pine Rd, Ogdenville',         'Active', NOW(), DATE_ADD(NOW(), INTERVAL 12 MONTH), 5, NOW(), 'Seed'),
(UUID(), 'LIB-000004', 'David', 'Brown',   'david.brown@email.com',   '+1-555-0104', '101 Elm St, North Haverbrook',    'Active', NOW(), DATE_ADD(NOW(), INTERVAL 12 MONTH), 5, NOW(), 'Seed'),
(UUID(), 'LIB-000005', 'Emily', 'Wilson',  'emily.wilson@email.com',  '+1-555-0105', '202 Birch Ln, Capital City',      'Active', NOW(), DATE_ADD(NOW(), INTERVAL 12 MONTH), 5, NOW(), 'Seed');

-- ─── Sample Borrowing Transactions ───────────────────────────
-- Borrow Clean Code by Alice Johnson (replace UUIDs with real ones from your DB)
-- INSERT INTO `BorrowingTransactions` (`Id`, `BookId`, `MemberId`, `BorrowedByUserId`, `Status`, `BorrowedDate`, `DueDate`, `CreatedAt`, `CreatedBy`) VALUES
-- (UUID(), '50000000-0000-0000-0000-000000000001', '<alice-member-id>', '60000000-0000-0000-0000-000000000002', 'Active', NOW(), DATE_ADD(NOW(), INTERVAL 14 DAY), NOW(), 'Seed');

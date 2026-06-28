# Entity Relationship Diagram — Library Management System

## ERD (Mermaid)

```mermaid
erDiagram

    ROLES {
        CHAR(36)    Id          PK
        VARCHAR(50) Name        "Admin | Librarian | Staff"
        VARCHAR(500) Description
        DATETIME    CreatedAt
        VARCHAR(256) CreatedBy
    }

    USERS {
        CHAR(36)    Id          PK
        VARCHAR(100) Username   UK
        VARCHAR(256) Email      UK
        VARCHAR(500) PasswordHash
        VARCHAR(100) FirstName
        VARCHAR(100) LastName
        TINYINT(1)  IsActive
        DATETIME    LastLoginAt
        CHAR(36)    RoleId      FK
        DATETIME    CreatedAt
        VARCHAR(256) CreatedBy
        DATETIME    UpdatedAt
        VARCHAR(256) UpdatedBy
    }

    LANGUAGES {
        CHAR(36)    Id          PK
        VARCHAR(100) Name
        VARCHAR(10) Code
        DATETIME    CreatedAt
        VARCHAR(256) CreatedBy
    }

    PUBLISHERS {
        CHAR(36)    Id          PK
        VARCHAR(200) Name
        VARCHAR(500) Description
        VARCHAR(500) Website
        DATETIME    CreatedAt
        VARCHAR(256) CreatedBy
    }

    CATEGORIES {
        CHAR(36)    Id              PK
        VARCHAR(200) Name
        VARCHAR(500) Description
        CHAR(36)    ParentCategoryId FK "self-ref nullable"
        DATETIME    CreatedAt
        VARCHAR(256) CreatedBy
    }

    AUTHORS {
        CHAR(36)    Id          PK
        VARCHAR(100) FirstName
        VARCHAR(100) LastName
        TEXT        Biography
        VARCHAR(200) Nationality
        DATETIME    CreatedAt
        VARCHAR(256) CreatedBy
    }

    BOOKS {
        CHAR(36)    Id              PK
        VARCHAR(20) ISBN            UK
        VARCHAR(500) Title
        VARCHAR(3000) Description
        DATE        PublishedDate
        INT         TotalCopies
        INT         AvailableCopies
        VARCHAR(50) Status          "Available|FullyBorrowed|Maintenance|Discontinued"
        VARCHAR(1000) CoverImageUrl
        INT         PageCount
        CHAR(36)    PublisherId     FK
        CHAR(36)    LanguageId      FK
        CHAR(36)    CategoryId      FK
        DATETIME    CreatedAt
        VARCHAR(256) CreatedBy
        DATETIME    UpdatedAt
        VARCHAR(256) UpdatedBy
    }

    BOOKAUTHORS {
        CHAR(36)    BookId          PK,FK
        CHAR(36)    AuthorId        PK,FK
        TINYINT(1)  IsPrimaryAuthor
    }

    MEMBERS {
        CHAR(36)    Id                  PK
        VARCHAR(20) MembershipNumber    UK
        VARCHAR(100) FirstName
        VARCHAR(100) LastName
        VARCHAR(256) Email              UK
        VARCHAR(50) PhoneNumber
        VARCHAR(500) Address
        VARCHAR(50) Status              "Active|Suspended|Expired|Pending"
        DATETIME    MembershipStartDate
        DATETIME    MembershipExpiryDate
        INT         MaxBorrowLimit
        DATETIME    CreatedAt
        VARCHAR(256) CreatedBy
        DATETIME    UpdatedAt
        VARCHAR(256) UpdatedBy
    }

    BORROWINGTRANSACTIONS {
        CHAR(36)    Id                      PK
        CHAR(36)    BookId                  FK
        CHAR(36)    MemberId                FK
        CHAR(36)    BorrowedByUserId        FK
        CHAR(36)    ReturnProcessedByUserId FK "nullable"
        DATETIME    BorrowedDate
        DATETIME    DueDate
        DATETIME    ReturnedDate            "nullable"
        VARCHAR(50) Status                  "Active|Returned|Overdue|Lost"
        VARCHAR(500) Notes
        VARCHAR(500) ReturnNotes
        DATETIME    CreatedAt
        VARCHAR(256) CreatedBy
        DATETIME    UpdatedAt
        VARCHAR(256) UpdatedBy
    }

    USERACTIVITYLOGS {
        CHAR(36)    Id              PK
        CHAR(36)    UserId
        VARCHAR(100) Username
        VARCHAR(200) Action
        VARCHAR(100) EntityType
        VARCHAR(100) EntityId
        LONGTEXT    OldValues
        LONGTEXT    NewValues
        VARCHAR(50) IpAddress
        VARCHAR(500) UserAgent
        DATETIME    Timestamp
        TINYINT(1)  IsSuccess
        VARCHAR(2000) ErrorMessage
        INT         HttpStatusCode
    }

    %% Relationships
    ROLES           ||--o{ USERS                   : "has many"
    USERS           ||--o{ BORROWINGTRANSACTIONS    : "processes (BorrowedBy)"
    USERS           ||--o{ BORROWINGTRANSACTIONS    : "processes (ReturnedBy)"
    LANGUAGES       ||--o{ BOOKS                   : "written in"
    PUBLISHERS      ||--o{ BOOKS                   : "publishes"
    CATEGORIES      ||--o{ BOOKS                   : "classifies"
    CATEGORIES      ||--o{ CATEGORIES              : "parent of (self-ref)"
    BOOKS           ||--o{ BOOKAUTHORS             : "written by"
    AUTHORS         ||--o{ BOOKAUTHORS             : "authors"
    BOOKS           ||--o{ BORROWINGTRANSACTIONS   : "borrowed in"
    MEMBERS         ||--o{ BORROWINGTRANSACTIONS   : "borrows"
```

---

## Relationships Summary

| Relationship | Type | Description |
|---|---|---|
| Role → Users | One-to-Many | A role can have many users |
| User → BorrowingTransactions | One-to-Many | A user (staff) processes borrowings |
| Language → Books | One-to-Many | A language is used by many books |
| Publisher → Books | One-to-Many | A publisher publishes many books |
| Category → Books | One-to-Many | A category classifies many books |
| Category → Category | Self One-to-Many | Hierarchical tree (Fiction → Mystery) |
| Book → BookAuthors | One-to-Many | A book can have multiple authors |
| Author → BookAuthors | One-to-Many | An author can write multiple books |
| Book ↔ Author | Many-to-Many | Via BookAuthors join table with IsPrimaryAuthor flag |
| Member → BorrowingTransactions | One-to-Many | A member can borrow many books |
| Book → BorrowingTransactions | One-to-Many | A book can be borrowed many times |

---

## Category Hierarchy (Tree)

```
Fiction
├── Mystery
├── Thriller
└── Science Fiction

Non-Fiction
└── Technology
    ├── Programming
    └── Web Development

Science
├── Biology
└── Physics
```

---

## Key Indexes

| Table | Index | Purpose |
|---|---|---|
| Books | ISBN (UNIQUE) | Fast lookup + duplicate prevention |
| Books | Title | Search queries |
| Books | Status | Filter by availability |
| Books | CategoryId | Filter by category |
| Members | MembershipNumber (UNIQUE) | Fast member lookup |
| Members | Email (UNIQUE) | Duplicate email prevention |
| BorrowingTransactions | (BookId, MemberId, Status) | Prevent double-borrowing |
| BorrowingTransactions | MemberId | Member history queries |
| BorrowingTransactions | DueDate | Overdue detection |
| UserActivityLogs | UserId | User audit trail |
| UserActivityLogs | Timestamp | Time-based log queries |
| Users | Username (UNIQUE) | Auth lookup |
| Users | Email (UNIQUE) | Auth lookup |

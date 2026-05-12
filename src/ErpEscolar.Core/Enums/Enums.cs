namespace ErpEscolar.Core.Enums;

public enum UserRole
{
    Admin = 0,
    Teacher = 1,
    Student = 2,
    Guardian = 3,
}

public enum ContractPeriod
{
    Morning = 0,
    Afternoon = 1,
    Evening = 2,
    FullTime = 3,
}

public enum InvoiceStatus
{
    Pending = 0,
    Paid = 1,
    Overdue = 2,
    Cancelled = 3,
}

public enum Bimester
{
    First = 1,
    Second = 2,
    Third = 3,
    Fourth = 4,
}

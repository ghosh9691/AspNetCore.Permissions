namespace PermissionTest.API.Auth;

public enum Permissions : ushort
{
    None = 0,
    
    TestRead = 63000,
    TestWrite = 63001,
    TestDelete = 63002,
    UsersRead = 64001,
    UsersWrite = 64002,
    UsersDelete = 64003,
    AllAccess = ushort.MaxValue
}
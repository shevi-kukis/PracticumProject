﻿public class UserRegisterDto
{
    public string FullName { get; set; }
    public string Password { get; set; }  // 🔹 שינוי שם השדה
    public string Email { get; set; }

    public int RoleId { get; set; } = 2;

}

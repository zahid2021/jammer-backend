from pydantic import BaseModel
from typing import Optional

class SignupRequest(BaseModel):
    FullName: str
    Email: str
    Password: str
    PhoneNumber: Optional[str] = None
    RoleId: int = 2

class LoginRequest(BaseModel):
    Email: str
    Password: str

class UpdateUserRequest(BaseModel):
    FullName: str
    PhoneNumber: Optional[str] = None
    Password: str
    NewPassword: str

class AddRoleRequest(BaseModel):
    Name: str

class UpdateRoleRequest(BaseModel):
    Id: int
    Name: str

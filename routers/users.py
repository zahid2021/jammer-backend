from fastapi import APIRouter, Request, Form, UploadFile, File, HTTPException
from typing import Optional
from utils.auth import create_token, get_current_user, get_current_role, decode_token
from utils.encryption import encrypt, match
from utils.file_manager import upload_file
from schemas.user import LoginRequest, AddRoleRequest, UpdateRoleRequest
import repositories.user as repo

router = APIRouter(prefix="/api/Users", tags=["Users"])

def response(data=None, message=""):
    return {"Data": data, "Message": message}

@router.post("/Signup")
async def signup(
    FullName: str = Form(...),
    Email: str = Form(...),
    Password: str = Form(...),
    PhoneNumber: Optional[str] = Form(None),
    Image: Optional[UploadFile] = File(None)
):
    image_path = await upload_file(Image) if Image else None
    data = {
        "FullName": FullName, "Email": Email,
        "PasswordHash": encrypt(Password),
        "PhoneNumber": PhoneNumber,
        "Image": image_path, "RoleId": 2
    }
    try:
        user_id = await repo.signup(data)
        token = create_token(str(user_id), "Customer")
        return response(token, "Customer registered successfully.")
    except Exception as e:
        if "Duplicate entry" in str(e) and "email" in str(e):
            raise HTTPException(400, "Email already exists")
        raise HTTPException(400, "Error occurred")

@router.post("/SignupByAdmin")
async def signup_by_admin(
    request: Request,
    FullName: str = Form(...),
    Email: str = Form(...),
    Password: str = Form(...),
    RoleId: int = Form(...),
    PhoneNumber: Optional[str] = Form(None),
    Image: Optional[UploadFile] = File(None)
):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    image_path = await upload_file(Image) if Image else None
    roles_map = {1: "Admin", 2: "Customer", 3: "Manager", 4: "DeliveryBoy"}
    data = {
        "FullName": FullName, "Email": Email,
        "PasswordHash": encrypt(Password),
        "PhoneNumber": PhoneNumber,
        "Image": image_path, "RoleId": RoleId
    }
    try:
        user_id = await repo.signup(data)
        role_name = roles_map.get(RoleId, "Customer")
        token = create_token(str(user_id), role_name)
        return response(token, f"{role_name} registered successfully.")
    except Exception as e:
        raise HTTPException(400, "Error occurred")

@router.post("/Login")
async def login(req: LoginRequest):
    user = await repo.login(req.Email)
    if not user or not match(req.Password, user["passwordhash"]):
        raise HTTPException(400, "Invalid email or password")
    roles_map = {1: "Admin", 2: "Customer", 3: "Manager", 4: "DeliveryBoy"}
    role_name = roles_map.get(user["roleid"], "Customer")
    token = create_token(str(user["id"]), role_name)
    return response(token, "Login successful")

@router.get("/GetAllUsers")
async def get_all_users(request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    users = await repo.get_all_users()
    return response([dict(u) for u in users], "Get Successfully")

@router.get("/GetUserById")
async def get_user_by_id(request: Request):
    user_id = get_current_user(request)
    user = await repo.get_user_by_id(user_id)
    if not user:
        raise HTTPException(404, "User not found")
    return dict(user)

@router.put("/UpdateUser")
async def update_user(
    request: Request,
    FullName: str = Form(...),
    Password: str = Form(...),
    NewPassword: str = Form(...),
    PhoneNumber: Optional[str] = Form(None),
    Image: Optional[UploadFile] = File(None)
):
    user_id = get_current_user(request)
    existing = await repo.get_user_by_id(user_id)
    if not existing or not match(Password, existing["passwordhash"]):
        raise HTTPException(400, "Invalid password")
    image_path = await upload_file(Image) if Image else existing["image"]
    data = {
        "FullName": FullName, "Passwordhash": encrypt(NewPassword),
        "PhoneNumber": PhoneNumber, "Image": image_path, "Id": user_id
    }
    if await repo.update_user(data):
        return response(message="User updated successfully.")
    raise HTTPException(400, "Update failed")

@router.delete("/DeleteAccount")
async def delete_account(request: Request):
    user_id = get_current_user(request)
    if await repo.delete_user(user_id):
        return response(message="User account deleted successfully.")
    raise HTTPException(400, "Delete failed")

@router.delete("/DeleteUserByAdmin/{id}")
async def delete_user_by_admin(id: str, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    if await repo.delete_user(id):
        return response(message="User deleted successfully.")
    raise HTTPException(400, "Delete failed")

@router.post("/AddRole")
async def add_role(req: AddRoleRequest, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    result = await repo.add_role(req.Name)
    return response(result, "Role added successfully")

@router.get("/GetAllRoles")
async def get_all_roles(request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    roles = await repo.get_all_roles()
    return response([dict(r) for r in roles], "Get Successfully")

@router.put("/UpdateRole")
async def update_role(req: UpdateRoleRequest, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    if await repo.update_role({"Name": req.Name, "Id": req.Id}):
        return response(message="Role updated successfully")
    raise HTTPException(404, "Role not found")

@router.delete("/DeleteRole/{id}")
async def delete_role(id: int, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    if await repo.delete_role(id):
        return response(message="Role deleted successfully")
    raise HTTPException(404, "Role not found")

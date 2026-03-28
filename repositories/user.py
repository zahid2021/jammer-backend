from db.database import database

async def signup(data: dict) -> int:
    query = """INSERT INTO user (fullname, passwordhash, email, image, roleid, phonenumber) 
               VALUES (:FullName, :PasswordHash, :Email, :Image, :RoleId, :PhoneNumber)"""
    result = await database.execute(query=query, values=data)
    return result

async def login(email: str):
    query = "SELECT * FROM user WHERE Email = :email"
    return await database.fetch_one(query=query, values={"email": email})

async def get_user_by_id(user_id: str):
    query = "SELECT * FROM user WHERE id = :id"
    return await database.fetch_one(query=query, values={"id": user_id})

async def get_all_users():
    query = """SELECT u.id, u.fullname, u.email, u.phonenumber, u.image,
               r.RoleID, r.Name as RoleName 
               FROM user u LEFT JOIN userrole r ON u.RoleId = r.RoleID"""
    return await database.fetch_all(query=query)

async def update_user(data: dict) -> bool:
    query = """UPDATE user SET fullname=:FullName, passwordhash=:Passwordhash,
               phonenumber=:PhoneNumber, image=:Image WHERE id=:Id"""
    result = await database.execute(query=query, values=data)
    return result > 0

async def update_user_by_admin(data: dict) -> bool:
    query = """UPDATE user SET fullname=:FullName, passwordhash=:Passwordhash,
               phonenumber=:PhoneNumber, image=:Image, roleId=:RoleId WHERE id=:Id"""
    result = await database.execute(query=query, values=data)
    return result > 0

async def delete_user(user_id: str) -> bool:
    query = "DELETE FROM user WHERE id = :id"
    result = await database.execute(query=query, values={"id": user_id})
    return result > 0

async def add_role(name: str) -> int:
    query = "INSERT INTO userrole (Name) VALUES (:Name)"
    return await database.execute(query=query, values={"Name": name})

async def get_all_roles():
    return await database.fetch_all("SELECT * FROM userrole")

async def get_role_by_id(role_id: int):
    return await database.fetch_one("SELECT Name FROM userrole WHERE roleid=:id", values={"id": role_id})

async def update_role(data: dict) -> bool:
    query = "UPDATE userrole SET Name=:Name WHERE roleid=:Id"
    result = await database.execute(query=query, values=data)
    return result > 0

async def delete_role(role_id: int) -> bool:
    result = await database.execute("DELETE FROM userrole WHERE roleid=:id", values={"id": role_id})
    return result > 0

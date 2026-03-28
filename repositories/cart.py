from db.database import database

async def add_to_cart(product_id: int, quantity: int, user_id: str) -> bool:
    query = """INSERT INTO Cart (UserId, ProductId, Quantity) VALUES (:UserId, :ProductId, :Quantity)
               ON DUPLICATE KEY UPDATE Quantity = Quantity + :Quantity"""
    await database.execute(query=query, values={"UserId": user_id, "ProductId": product_id, "Quantity": quantity})
    return True

async def get_user_cart(user_id: str):
    query = """SELECT c.Id, c.ProductId, c.Quantity, p.Name, p.Price, p.ProductURL,
               pi.ImagePath FROM Cart c
               JOIN Product p ON c.ProductId = p.Id
               LEFT JOIN ProductImage pi ON pi.ProductId = p.Id
               WHERE c.UserId = :uid GROUP BY c.Id"""
    return await database.fetch_all(query=query, values={"uid": user_id})

async def get_cart_by_id(user_id: str, cart_id: int):
    query = "SELECT * FROM Cart WHERE Id=:cart_id AND UserId=:uid"
    return await database.fetch_one(query=query, values={"cart_id": cart_id, "uid": user_id})

async def update_cart(cart_id: int, quantity: int) -> bool:
    result = await database.execute("UPDATE Cart SET Quantity=:q WHERE Id=:id", values={"q": quantity, "id": cart_id})
    return result > 0

async def delete_cart_item(cart_id: int) -> bool:
    result = await database.execute("DELETE FROM Cart WHERE Id=:id", values={"id": cart_id})
    return result > 0

async def delete_all_user_cart(user_id: str) -> bool:
    result = await database.execute("DELETE FROM Cart WHERE UserId=:uid", values={"uid": user_id})
    return result > 0

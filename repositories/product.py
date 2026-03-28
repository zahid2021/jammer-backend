from db.database import database
from utils.file_manager import upload_file
from typing import List, Optional

async def create_product(data: dict, images: list, upload_fn) -> bool:
    query = """INSERT INTO Product (Name, Description, Price, StockQuantity, CategoryId, ProductURL)
               VALUES (:Name, :Description, :Price, :StockQuantity, :CategoryId, :ProductURL)"""
    product_id = await database.execute(query=query, values=data)
    if images:
        for image_path in images:
            img_query = """INSERT INTO ProductImage (ProductId, ImagePath, CreatedAt)
                           VALUES (:ProductId, :ImagePath, NOW())"""
            await database.execute(query=img_query, values={"ProductId": product_id, "ImagePath": image_path})
    return True

async def update_product(data: dict, images: list, delete_ids: list) -> bool:
    query = """UPDATE Product SET Name=:Name, Description=:Description, Price=:Price,
               StockQuantity=:StockQuantity, CategoryId=:CategoryId, ProductURL=:ProductURL
               WHERE Id=:Id"""
    rows = await database.execute(query=query, values=data)
    if delete_ids:
        del_query = "DELETE FROM ProductImage WHERE Id IN :ids"
        await database.execute(query=del_query, values={"ids": tuple(delete_ids)})
    if images:
        for image_path in images:
            img_query = """INSERT INTO ProductImage (ProductId, ImagePath, CreatedAt)
                           VALUES (:ProductId, :ImagePath, NOW())"""
            await database.execute(query=img_query, values={"ProductId": data["Id"], "ImagePath": image_path})
    return rows > 0

async def get_products_paged(page: int, size: int):
    offset = (page - 1) * size
    query = """SELECT p.Id, p.Name, p.Description, p.Price, p.StockQuantity,
               p.CreatedAt, p.ProductURL, p.UpdatedAt, p.CategoryId,
               c.Name AS CategoryName
               FROM Product p LEFT JOIN categories c ON p.CategoryId = c.Id
               ORDER BY p.CreatedAt DESC LIMIT :size OFFSET :offset"""
    products = await database.fetch_all(query=query, values={"size": size, "offset": offset})
    return await _attach_images(products)

async def filter_by_category(category_id: int):
    query = """SELECT p.Id, p.Name, p.Description, p.Price, p.StockQuantity,
               p.CreatedAt, p.ProductURL, p.UpdatedAt, p.CategoryId,
               c.Name AS CategoryName
               FROM Product p LEFT JOIN Categories c ON p.CategoryId = c.Id
               WHERE p.CategoryId = :CategoryId"""
    products = await database.fetch_all(query=query, values={"CategoryId": category_id})
    return await _attach_images(products)

async def search_by_name(q: str):
    query = """SELECT p.Id, p.Name, p.Description, p.Price, p.StockQuantity,
               p.CreatedAt, p.ProductURL, p.UpdatedAt, p.CategoryId,
               c.Name AS CategoryName
               FROM Product p LEFT JOIN Categories c ON p.CategoryId = c.Id
               WHERE p.Name LIKE :q"""
    products = await database.fetch_all(query=query, values={"q": f"%{q}%"})
    return await _attach_images(products)

async def get_by_id(product_id: int):
    query = """SELECT p.Id, p.Name, p.Description, p.Price, p.StockQuantity,
               p.CreatedAt, p.ProductURL, p.UpdatedAt, p.CategoryId,
               c.Name AS CategoryName
               FROM Product p LEFT JOIN Categories c ON p.CategoryId = c.Id
               WHERE p.Id = :id"""
    product = await database.fetch_one(query=query, values={"id": product_id})
    if not product:
        return None
    images = await database.fetch_all("SELECT Id, ImagePath FROM ProductImage WHERE ProductId=:id", values={"id": product_id})
    return {**dict(product), "ImagePath": [dict(i) for i in images]}

async def get_by_url(url: str):
    query = """SELECT p.*, c.Name AS CategoryName FROM Product p
               LEFT JOIN Categories c ON p.CategoryId = c.Id WHERE p.ProductURL=:url"""
    product = await database.fetch_one(query=query, values={"url": url})
    if not product:
        return None
    images = await database.fetch_all("SELECT ImagePath FROM ProductImage WHERE ProductId=:id", values={"id": product["Id"]})
    return {**dict(product), "ImagePath": [i["ImagePath"] for i in images]}

async def update_stock(product_id: int, quantity: int) -> bool:
    result = await database.execute("UPDATE Product SET StockQuantity=:q WHERE Id=:id", values={"q": quantity, "id": product_id})
    return result > 0

async def delete_product(product_id: int) -> bool:
    await database.execute("DELETE FROM ProductImage WHERE ProductId=:id", values={"id": product_id})
    result = await database.execute("DELETE FROM Product WHERE Id=:id", values={"id": product_id})
    return result > 0

async def get_all_id_names():
    return await database.fetch_all("SELECT Id, Name FROM Product")

async def _attach_images(products):
    result = []
    for p in products:
        p = dict(p)
        images = await database.fetch_all("SELECT ImagePath FROM ProductImage WHERE ProductId=:id", values={"id": p["Id"]})
        p["ImagePath"] = [i["ImagePath"] for i in images]
        result.append(p)
    return result

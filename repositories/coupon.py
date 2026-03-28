from db.database import database

async def create_coupon(data: dict) -> bool:
    query = """INSERT INTO Coupon (Code, DiscountPercentage, ExpiryDate)
               VALUES (:Code, :DiscountPercentage, :ExpiryDate)"""
    await database.execute(query=query, values=data)
    return True

async def get_all_coupons():
    return await database.fetch_all("SELECT * FROM Coupon")

async def get_coupon_by_id(coupon_id: int):
    return await database.fetch_one("SELECT * FROM Coupon WHERE Id=:id", values={"id": coupon_id})

async def update_coupon(data: dict) -> bool:
    query = """UPDATE Coupon SET Code=:Code, DiscountPercentage=:DiscountPercentage,
               ExpiryDate=:ExpiryDate WHERE Id=:Id"""
    result = await database.execute(query=query, values=data)
    return result > 0

async def delete_coupon(coupon_id: int) -> bool:
    result = await database.execute("DELETE FROM Coupon WHERE Id=:id", values={"id": coupon_id})
    return result > 0

async def get_random_coupon_products():
    query = """SELECT cp.CouponId, cp.ProductId, p.Name, p.Price, p.ProductURL,
               c.Code, c.DiscountPercentage FROM CouponProduct cp
               JOIN Product p ON cp.ProductId = p.Id
               JOIN Coupon c ON cp.CouponId = c.Id
               ORDER BY RAND() LIMIT 10"""
    return await database.fetch_all(query=query)

async def get_products_by_coupon(coupon_id: str):
    query = """SELECT cp.ProductId, p.Name, p.Price FROM CouponProduct cp
               JOIN Product p ON cp.ProductId = p.Id WHERE cp.CouponId=:cid"""
    return await database.fetch_all(query=query, values={"cid": coupon_id})

async def get_product_by_coupon_and_product(coupon_id: str, product_id: str):
    query = """SELECT * FROM CouponProduct WHERE CouponId=:cid AND ProductId=:pid"""
    return await database.fetch_one(query=query, values={"cid": coupon_id, "pid": product_id})

async def get_by_discount(discount: int):
    return await database.fetch_all("SELECT * FROM Coupon WHERE DiscountPercentage=:d", values={"d": discount})

async def get_by_discount_range(min_d: float, max_d: float):
    return await database.fetch_all("SELECT * FROM Coupon WHERE DiscountPercentage BETWEEN :min AND :max", values={"min": min_d, "max": max_d})

async def get_coupons_with_products():
    query = """SELECT c.Id, c.Code, c.DiscountPercentage, p.Id as ProductId, p.Name
               FROM Coupon c JOIN CouponProduct cp ON c.Id=cp.CouponId
               JOIN Product p ON cp.ProductId=p.Id"""
    return await database.fetch_all(query=query)

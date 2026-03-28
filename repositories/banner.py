from db.database import database

async def create_banner(link_id: int, link: str, image_path: str, coupon_id: str = None) -> int:
    query = """INSERT INTO Banner (LinkId, Link, ImagePath, CouponId, CreatedAt)
               VALUES (:LinkId, :Link, :ImagePath, :CouponId, NOW())"""
    return await database.execute(query=query, values={"LinkId": link_id, "Link": link, "ImagePath": image_path, "CouponId": coupon_id})

async def get_all_banners():
    return await database.fetch_all("SELECT * FROM Banner")

async def delete_banner(banner_id: int) -> bool:
    result = await database.execute("DELETE FROM Banner WHERE Id=:id", values={"id": banner_id})
    return result > 0

from fastapi import APIRouter, Request, HTTPException
from utils.auth import get_current_role
from schemas.coupon import CouponRequest, ApplyCouponRequest
import repositories.coupon as repo

router = APIRouter(prefix="/api/Coupon", tags=["Coupons"])

def response(data=None, message=""):
    return {"Data": data, "Message": message}

@router.post("/CreateCoupon")
async def create_coupon(req: CouponRequest, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    await repo.create_coupon({"Code": req.Code, "DiscountPercentage": req.DiscountPercentage, "ExpiryDate": req.ExpiryDate})
    return response(message="Coupon created successfully")

@router.get("/GetAllCoupons")
async def get_all():
    data = await repo.get_all_coupons()
    return response([dict(d) for d in data], "Coupons fetched")

@router.get("/GetCouponById/{id}")
async def get_by_id(id: int):
    data = await repo.get_coupon_by_id(id)
    return response(dict(data) if data else None, "Coupon fetched")

@router.put("/UpdateCoupon")
async def update_coupon(req: CouponRequest, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    await repo.update_coupon({"Id": req.Id, "Code": req.Code, "DiscountPercentage": req.DiscountPercentage, "ExpiryDate": req.ExpiryDate})
    return response(message="Coupon updated")

@router.delete("/DeleteCoupon/{id}")
async def delete_coupon(id: int, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    if await repo.delete_coupon(id):
        return response(message="Coupon deleted")
    raise HTTPException(400, "Delete failed")

@router.get("/GetRandomCouponProducts")
async def random_products():
    data = await repo.get_random_coupon_products()
    return response([dict(d) for d in data], "Fetched")

@router.get("/GetProductsByCoupon/{couponId}")
async def by_coupon(couponId: str):
    data = await repo.get_products_by_coupon(couponId)
    return response([dict(d) for d in data], "Fetched")

@router.get("/GetProductByProductIdCouponIdAsync/{couponId}/{productId}")
async def by_coupon_product(couponId: str, productId: str):
    data = await repo.get_product_by_coupon_and_product(couponId, productId)
    return response(dict(data) if data else None, "Fetched")

@router.get("/GetProductsByDiscountPercentage")
async def by_discount(discountPercentage: int):
    data = await repo.get_by_discount(discountPercentage)
    return response([dict(d) for d in data], "Fetched")

@router.get("/GetProductsByDiscountRange")
async def by_range(minDiscount: float, maxDiscount: float):
    data = await repo.get_by_discount_range(minDiscount, maxDiscount)
    return response([dict(d) for d in data], "Fetched")

@router.get("/GetCouponsWithProductsAsync")
async def with_products():
    data = await repo.get_coupons_with_products()
    return response([dict(d) for d in data], "Fetched")

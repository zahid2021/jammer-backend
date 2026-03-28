from fastapi import APIRouter, Request, HTTPException
from utils.auth import get_current_user, get_current_role
from schemas.cart import AddToCartRequest, UpdateCartRequest
import repositories.cart as repo

router = APIRouter(prefix="/api/Cart", tags=["Cart"])

def response(data=None, message=""):
    return {"Data": data, "Message": message}

@router.post("/AddToCart")
async def add_to_cart(req: AddToCartRequest, request: Request):
    user_id = get_current_user(request)
    await repo.add_to_cart(req.ProductId, req.Quantity, user_id)
    return response(message="Item added to cart")

@router.get("/GetUserCart")
async def get_cart(request: Request):
    user_id = get_current_user(request)
    data = await repo.get_user_cart(user_id)
    return response([dict(d) for d in data], "Cart fetched" if data else "Cart is empty")

@router.put("/UpdateCartItems")
async def update_cart(req: UpdateCartRequest, request: Request):
    if await repo.update_cart(req.CartId, req.Quantity):
        return response(message="Cart updated")
    raise HTTPException(400, "Update failed")

@router.delete("/DeleteCartItem/{cartId}")
async def delete_item(cartId: int, request: Request):
    if await repo.delete_cart_item(cartId):
        return response(message="Item removed from cart")
    raise HTTPException(400, "Delete failed")

@router.delete("/DeleteAllUserCart")
async def delete_all(request: Request):
    user_id = get_current_user(request)
    if await repo.delete_all_user_cart(user_id):
        return response(message="Cart cleared")
    raise HTTPException(400, "Delete failed")

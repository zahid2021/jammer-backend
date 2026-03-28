from fastapi import APIRouter, Request, HTTPException
from utils.auth import get_current_user, get_current_role
from schemas.order import CreateOrderRequest
import repositories.order as repo

router = APIRouter(prefix="/api/Order", tags=["Orders"])

def response(data=None, message=""):
    return {"Data": data, "Message": message}

@router.post("/CreateOrder")
async def create_order(req: CreateOrderRequest, request: Request):
    user_id = get_current_user(request)
    data = await repo.create_order(user_id, req.AddressId, req.CouponId)
    return response(data, "Order created successfully")

@router.get("/GetOrdersByUserId")
async def get_my_orders(request: Request):
    user_id = get_current_user(request)
    data = await repo.get_orders_by_user(user_id)
    return response([dict(d) for d in data] if data else [], "Orders fetched")

@router.get("/GetOrderById/{orderId}")
async def get_order(orderId: int, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    data = await repo.get_order_by_id(orderId)
    return response(dict(data) if data else None, "Order fetched")

@router.put("/UpdateOrderStatus/{orderId}")
async def update_status(orderId: int, status: str, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    if await repo.update_order_status(orderId, status):
        return response(message="Order status updated")
    raise HTTPException(400, "Update failed")

@router.put("/CancelOrder/{orderId}")
async def cancel_order(orderId: int, request: Request):
    if await repo.cancel_order(orderId):
        return response(message="Order cancelled")
    raise HTTPException(400, "Cancel failed")

@router.delete("/DeleteOrder/{orderId}")
async def delete_order(orderId: int, request: Request):
    if await repo.delete_order(orderId):
        return response(message="Order deleted")
    raise HTTPException(400, "Delete failed")

@router.get("/GetOrdersByStatus/{status}")
async def get_by_status(status: str, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    data = await repo.get_orders_by_status(status)
    return response([dict(d) for d in data] if data else [], "Orders fetched")

@router.get("/GetAllOrders/all")
async def get_all(request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    data = await repo.get_all_orders()
    return response([dict(d) for d in data] if data else [], "Orders fetched")

@router.get("/GetOrderSummary/summary")
async def get_summary(request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    data = await repo.get_order_summary()
    return response(dict(data) if data else None, "Summary fetched")

@router.get("/GetMonthlyOrderReport/monthly-order-report")
async def monthly_report(request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    data = await repo.get_monthly_report()
    return response([dict(d) for d in data] if data else [], "Report fetched")

@router.get("/GetOverallOrderReport/overall-order-report")
async def overall_report(request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    data = await repo.get_overall_report()
    return response(dict(data) if data else None, "Report fetched")

@router.get("/GetOrderStatusCounts")
async def status_counts(request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    data = await repo.get_status_counts()
    return response([dict(d) for d in data] if data else [], "Counts fetched")

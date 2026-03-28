from pydantic import BaseModel
from typing import Optional

class CreateOrderRequest(BaseModel):
    AddressId: int
    CouponId: Optional[str] = None

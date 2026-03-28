from pydantic import BaseModel
from typing import Optional

class CouponRequest(BaseModel):
    Id: Optional[int] = None
    Code: str
    DiscountPercentage: float
    ExpiryDate: Optional[str] = None

class ApplyCouponRequest(BaseModel):
    CouponId: str
    ProductId: str

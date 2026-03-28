from pydantic import BaseModel
from typing import Optional

class AddToCartRequest(BaseModel):
    ProductId: int
    Quantity: int

class UpdateCartRequest(BaseModel):
    CartId: int
    Quantity: int

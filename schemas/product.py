from pydantic import BaseModel
from typing import Optional, List

class UpdateProductStockRequest(BaseModel):
    ProductId: int
    NewStockQuantity: int

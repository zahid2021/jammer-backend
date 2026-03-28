from pydantic import BaseModel
from typing import Optional

class AddReviewRequest(BaseModel):
    ProductId: int
    Rating: int
    Comment: Optional[str] = None

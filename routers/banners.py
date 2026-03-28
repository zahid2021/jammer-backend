from fastapi import APIRouter, Request, Form, UploadFile, File, HTTPException
from typing import Optional
from utils.auth import get_current_role
from utils.file_manager import upload_file
import repositories.banner as repo

router = APIRouter(prefix="/api/Banner", tags=["Banners"])

def response(data=None, message=""):
    return {"Data": data, "Message": message}

@router.post("/AddBanner")
async def add_banner(
    request: Request,
    LinkId: int = Form(...),
    Link: str = Form(...),
    CouponId: Optional[str] = Form(None),
    Image: UploadFile = File(...)
):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    image_path = await upload_file(Image)
    banner_id = await repo.create_banner(LinkId, Link, image_path, CouponId)
    if banner_id:
        return response(banner_id, "Banner added successfully")
    raise HTTPException(400, "Failed to add banner")

@router.get("/GetAllBanners")
async def get_all():
    data = await repo.get_all_banners()
    return response([dict(d) for d in data] if data else None, "Banners fetched")

@router.delete("/DeleteBanner/{Id}")
async def delete_banner(Id: int):
    if await repo.delete_banner(Id):
        return response(message="Banner deleted")
    raise HTTPException(400, "Delete failed")

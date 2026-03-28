from fastapi import APIRouter, Request, Form, UploadFile, File, HTTPException
from typing import Optional, List
from utils.auth import get_current_role
from utils.file_manager import upload_file
from schemas.product import UpdateProductStockRequest
import repositories.product as repo

router = APIRouter(prefix="/api/Product", tags=["Products"])

def response(data=None, message=""):
    return {"Data": data, "Message": message}

@router.post("/AddProductWithImages")
async def add_product(
    request: Request,
    Name: str = Form(...),
    Description: Optional[str] = Form(None),
    Price: float = Form(...),
    StockQuantity: int = Form(...),
    CategoryId: int = Form(...),
    ProductURL: str = Form(...),
    Images: Optional[List[UploadFile]] = File(None)
):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    image_paths = []
    if Images:
        for img in Images:
            path = await upload_file(img)
            image_paths.append(path)
    data = {"Name": Name, "Description": Description, "Price": Price,
            "StockQuantity": StockQuantity, "CategoryId": CategoryId, "ProductURL": ProductURL}
    await repo.create_product(data, image_paths, upload_file)
    return response(message="Product added successfully")

@router.put("/UpdateProductAsync")
async def update_product(
    request: Request,
    Id: int = Form(...),
    Name: str = Form(...),
    Description: Optional[str] = Form(None),
    Price: float = Form(...),
    StockQuantity: int = Form(...),
    CategoryId: int = Form(...),
    ProductURL: str = Form(...),
    ImageIdsToDelete: Optional[str] = Form(None),
    Images: Optional[List[UploadFile]] = File(None)
):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    delete_ids = [int(x) for x in ImageIdsToDelete.split(",")] if ImageIdsToDelete else []
    image_paths = []
    if Images:
        for img in Images:
            path = await upload_file(img)
            image_paths.append(path)
    data = {"Id": Id, "Name": Name, "Description": Description, "Price": Price,
            "StockQuantity": StockQuantity, "CategoryId": CategoryId, "ProductURL": ProductURL}
    await repo.update_product(data, image_paths, delete_ids)
    return response(message="Product updated successfully")

@router.get("/GetProductsWithPaging")
async def get_products(pageNumber: int = 1, pageSize: int = 10):
    data = await repo.get_products_paged(pageNumber, pageSize)
    return response(data, "Products fetched" if data else "Not found")

@router.get("/FilterProductsByCategory/{parentId}")
async def filter_by_category(parentId: int):
    data = await repo.filter_by_category(parentId)
    return response(data, "Products fetched" if data else "Not found")

@router.get("/SearchProductsByName")
async def search(query: str):
    data = await repo.search_by_name(query)
    return response(data, "Products fetched" if data else "Not found")

@router.get("/GetProductById")
async def get_by_id(productId: int, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    data = await repo.get_by_id(productId)
    return response(data, "Product fetched" if data else "Not found")

@router.get("/GetProductByURL/url")
async def get_by_url(url: str):
    data = await repo.get_by_url(url)
    return response(data, "Product fetched" if data else "Not found")

@router.put("/UpdateProductStock")
async def update_stock(req: UpdateProductStockRequest, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    if await repo.update_stock(req.ProductId, req.NewStockQuantity):
        return response(message="Stock updated successfully")
    raise HTTPException(404, "Product not found")

@router.delete("/DeleteProduct/{id}")
async def delete_product(id: int, request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    if await repo.delete_product(id):
        return response(message="Product deleted successfully")
    raise HTTPException(400, "Delete failed")

@router.get("/GetAllProductIdAndNames")
async def get_all_names(request: Request):
    role = get_current_role(request)
    if role != "Admin":
        raise HTTPException(401, "Unauthorized")
    data = await repo.get_all_id_names()
    return response([dict(d) for d in data], "Products fetched")

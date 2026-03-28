import os, uuid, aiofiles
from fastapi import UploadFile
from dotenv import load_dotenv

load_dotenv()
UPLOAD_DIR = os.getenv("UPLOAD_DIR", "uploads")

async def upload_file(file: UploadFile) -> str:
    if not file:
        return None
    os.makedirs(UPLOAD_DIR, exist_ok=True)
    ext = file.filename.split(".")[-1]
    filename = f"{uuid.uuid4()}.{ext}"
    path = os.path.join(UPLOAD_DIR, filename)
    async with aiofiles.open(path, "wb") as f:
        content = await file.read()
        await f.write(content)
    return f"/{UPLOAD_DIR}/{filename}"

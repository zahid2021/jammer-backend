from datetime import datetime, timedelta
from jose import jwt, JWTError
from fastapi import HTTPException, Request
import os
from dotenv import load_dotenv

load_dotenv()

SECRET_KEY = os.getenv("JWT_SECRET", "your_secret_key")
ALGORITHM = "HS256"
ISSUER = os.getenv("JWT_ISSUER", "jammer")

def create_token(user_id: str, role: str) -> str:
    payload = {
        "UserId": user_id,
        "Role": role,
        "iss": ISSUER,
        "aud": ISSUER,
    }
    return jwt.encode(payload, SECRET_KEY, algorithm=ALGORITHM)

def decode_token(token: str) -> dict:
    try:
        return jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM], audience=ISSUER)
    except JWTError:
        raise HTTPException(status_code=401, detail="Invalid token")

def get_current_user(request: Request) -> str:
    auth = request.headers.get("Authorization", "")
    token = auth.replace("Bearer ", "")
    payload = decode_token(token)
    return payload.get("UserId")

def get_current_role(request: Request) -> str:
    auth = request.headers.get("Authorization", "")
    token = auth.replace("Bearer ", "")
    payload = decode_token(token)
    return payload.get("Role")

from cryptography.fernet import Fernet
import base64
import hashlib

def get_key():
    secret = "jammer_secret_key_32_bytes_long!!"
    key = base64.urlsafe_b64encode(secret.encode()[:32])
    return key

def encrypt(password: str) -> str:
    f = Fernet(get_key())
    return f.encrypt(password.encode()).decode()

def match(plain: str, encrypted: str) -> bool:
    try:
        f = Fernet(get_key())
        decrypted = f.decrypt(encrypted.encode()).decode()
        return plain == decrypted
    except:
        return False

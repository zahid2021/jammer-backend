# Jammer Backend API

A full-featured e-commerce REST API built with **Python FastAPI** and **MySQL**.

---

## 📋 Modules

- **Users** — Signup, Login, JWT Auth, Roles (Admin, Customer, Manager, DeliveryBoy)
- **Products** — CRUD, Images, Search, Filter by Category, Pagination
- **Cart** — Add, Update, Delete items
- **Orders** — Create, Track, Cancel, Reports
- **Coupons** — Create, Apply, Discount filters
- **Banners** — Add, Get, Delete

---

## 🛠️ Tech Stack

Python · FastAPI · MySQL · JWT · Uvicorn · Pydantic

---

## ⚙️ Installation
```bash
pip install -r requirements.txt
```

## 🚀 Usage
```bash
uvicorn main:app --reload --port 8000
```

Swagger docs: `http://localhost:8000/docs`

---

## 🔐 Auth

JWT Bearer token — include in header:
```
Authorization: Bearer <token>
```

### Roles
Admin · Customer · Manager · DeliveryBoy

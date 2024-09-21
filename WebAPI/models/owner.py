from sqlalchemy import Column, Integer, String, DateTime
from models.base import BaseDBModel

class OwnerDB(BaseDBModel):
    __tablename__ = "owner"

    id = Column(Integer, primary_key=True)
    email = Column(String, nullable=False)
    code = Column(String, nullable=True)
    code_updated = Column(DateTime, nullable=True)

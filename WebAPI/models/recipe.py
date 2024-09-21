from sqlalchemy import Column, Integer, String
from models.base import BaseDBModel

class IngredientDB(BaseDBModel):
    __tablename__ = "ingredient"

    id = Column(Integer, primary_key=True)
    owner_id = Column(Integer)
    name = Column(String, nullable=False)

class RecipeDB(BaseDBModel):
    __tablename__ = "recipe"

    id = Column(Integer, primary_key=True)
    owner_id = Column(Integer)
    name = Column(String, nullable=False)
    description = Column(String, nullable=True)
    content = Column(String, nullable=False)

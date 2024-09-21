from sqlalchemy.orm import Session

from models.recipe import IngredientDB, RecipeDB
from schemas.recipe import RecipeCreate, RecipeUpdate, RecipeInDb


def get_ingredients(db: Session):
    return db.query(IngredientDB).all()

def get_recipes(db: Session, owner_id: int) -> list[RecipeDB] | None:
    return db.query(RecipeDB).filter(RecipeDB.owner_id == owner_id).all()

def get_recipe(db: Session, id: int, owner_id: int) -> RecipeDB | None:
    return db.query(RecipeDB).filter(RecipeDB.id == id).first()

def create_recipe(db: Session, owner_id: int, recipe: RecipeCreate) -> RecipeDB:
    recipe_dict = recipe.model_dump(exclude_unset=True)
    recipe_new = RecipeDB(**recipe_dict)
    recipe_new.owner_id = owner_id
    db.add(recipe_new)
    db.commit()
    db.refresh(recipe_new)
    return recipe_new

def update_recipe(db: Session, recipe_model: RecipeDB, recipe: RecipeUpdate) -> RecipeDB:
    recipe_dict = recipe.model_dump()
    for field, value in recipe_dict.items():
        setattr(recipe_model, field, value)
    db.add(recipe_model)
    db.commit()
    db.refresh(recipe_model)
    return recipe_model
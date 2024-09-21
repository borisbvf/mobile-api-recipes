from fastapi import APIRouter, Body, Depends
from typing import Annotated
from sqlalchemy.orm import Session

from dependencies import get_local_session, get_current_owner
from crud import recipe as crud_recipe
from models.recipe import RecipeDB
from models.owner import OwnerDB
from schemas.recipe import RecipeCreate, RecipeUpdate, RecipeInDb


recipe_router = APIRouter()

@recipe_router.get("/ingredients")
async def get_ingredients():
    return crud_recipe.get_ingredients()

@recipe_router.get("/list")
async def get_recipes(
        db: Session = Depends(get_local_session), 
        owner: OwnerDB = Depends(get_current_owner)
        ):
    recipes = crud_recipe.get_recipes(db, owner.id)
    return recipes

@recipe_router.get("/{recipe_id}")
async def get_recipe(
        recipe_id: int, 
        db: Session = Depends(get_local_session), 
        owner: OwnerDB = Depends(get_current_owner)):
    recipe = crud_recipe.get_recipe(db, recipe_id, owner.id)
    return recipe

@recipe_router.post("/")
async def create_recipe(
        recipe: RecipeCreate,
        db: Session = Depends(get_local_session),
        owner: OwnerDB = Depends(get_current_owner)
        ):
    recipe_new = crud_recipe.create_recipe(db, owner.id, recipe)
    return recipe_new

@recipe_router.put("/{recipe_id}")
async def update_recipe(
        recipe_id: int,
        recipe: RecipeUpdate, 
        db: Session = Depends(get_local_session),
        owner: OwnerDB = Depends(get_current_owner)):
    recipe_model = crud_recipe.get_recipe(db, recipe_id, owner.id)
    recipe_upd = crud_recipe.update_recipe(db, recipe_model, recipe)
    return recipe_upd

@recipe_router.delete("/{recipe_id}")
async def delete_recipe():
    pass

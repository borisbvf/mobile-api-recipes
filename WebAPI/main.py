from fastapi import FastAPI
from routes import recipe, owner

app = FastAPI(title="Recipes")

app.include_router(
    router=recipe.recipe_router, 
    prefix="/recipes", 
    tags=["recipes"]
    )
app.include_router(
    router=owner.owner_router,
    prefix="/owners",
    tags=["owners"]
)

@app.get("/")
async def root():
    return {"Hello": "World"}
